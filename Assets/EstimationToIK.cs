using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using AuxiliarContent;
using UnityEngine.Animations.Rigging;
using static EstimationToIK;

public class EstimationToIK : MonoBehaviour
{
    public Socket_toHl2 TcpScript;
    public CameraStream CameraStreamScript;

    public GameObject[] keypointBones; //for keypoints that are to be set specifically and not with 
    public GameObject leftHand; //used for aesthetic adjustments after IK
    public GameObject rightHand;
    protected Animator animator;
    public bool Looping = true; //TODO: fix the False setting, perhaps by introducing a new bool to check for the animation finishing.

    public Vector3[] landmarks;
    public float scaleFactor = 0.02f; //0.005f; // Adjust the scaling factor as needed
    private int frameCount;
    private int smoothingFrameCount = 0;

    private Vector3 origin;

    public Vector3 centerpointOffset = new Vector3(0,0,0); //how far the center is from the Unity pivot. This depends on the model we are currently using.
    public int startFrame = 0;
    public int endFrame = 100; //might be overwritten by max number of frame files

    public int smoothingFrames = 30; //frames to interpolate back to default pose or initial pose. Set to 1 to disable this feature.
    private float fadeStartTime = 0.0f;
    private float animStartTime = 0.0f;

    private string basePath = "assets/GestureDictionary/";
    public string selectedAnimName = "coverFace";

    //used to not re-read  the same data multiple times if FPS is high
    private int lastProcessedFrame;


    //used to indicate that we are resetting to a starting pose
    public bool smoothing = true;
    private bool fadingOut = false;
    private bool fadingIn = false;


    //Where to receive data. LevelManager might overwrite these if active.
    public enum estimationSource {None, HololensTcp, Panoptic, Recording, PointedDircetions, MediaPipe}; //later we could combine mediapipe and hololens
    public estimationSource currentEstimationSource;

    [System.Serializable]
    public class BodyData
    {
        public float version;
        public float univTime;
        public string fpsType;
        public Body[] bodies;
    }

    [System.Serializable]
    public class Body
    {
        public int id;
        public float[] joints19;
    }

    private void switchToAnimation(string newSelectedAnimName, int newStartFrame, int newEndFrame)
    {
        origin = transform.position; //save the origin position of where we started the animation
        selectedAnimName = newSelectedAnimName;
        startFrame = newStartFrame;
        endFrame = newEndFrame;
        animStartTime = Time.time;
        fadeInStart();
    }
    private void adjustHands()
    {
        if(rightHand != null && leftHand != null)
        {
            rightHand.transform.rotation = rightHand.transform.parent.rotation;
            leftHand.transform.rotation = leftHand.transform.parent.rotation;
        }
    }

    private Vector3 goalFromIndex(int index)
    {
        if(index  < landmarks.Length)
        {
            Vector3 currentPos = landmarks[index] * scaleFactor; // x-axis pos

            if (currentEstimationSource == estimationSource.MediaPipe)
            {
                return transform.position + new Vector3(currentPos.z, -currentPos.y, -currentPos.x);
            }
            return origin + new Vector3(-currentPos.x, currentPos.y, currentPos.z); //Y is inverted, but how about the others?
        }
        else
        {
            //Debug.LogWarning("goalFromIndex is trying to index landmarks that were not received.");
            return new Vector3(0, 0, 0);
        }
    }

    private void SetIKPosition(int index, AvatarIKGoal limb)
    {

        Vector3 goal = goalFromIndex(index);
        animator.SetIKPositionWeight(limb, 1);
        //animator.SetIKRotationWeight(limb, 1);
        animator.SetIKPosition(limb, goal);
    }

    private void SetIKPosition(int index, AvatarIKHint limb)
    {
        Vector3 goal = goalFromIndex(index);
        animator.SetIKHintPositionWeight(limb, 1);
        animator.SetIKHintPosition(limb, goal);
    }


    public TwoBoneIKConstraint[] constraints; 
    GameObject[] ikPointerObjects = new GameObject[10];
    private void SetIKPosition(int index, string fingerName)
    {
        Vector3 goal = goalFromIndex(index);

        for(int i = 0; i < constraints.Length; i++)
        {
            if (constraints[i].data.tip != null && constraints[i].data.tip.name.Contains(fingerName))
            {
                if (ikPointerObjects[i] == null)
                {
                    ikPointerObjects[i] = new GameObject("TemporaryTargetObject" + i);
                }

                ikPointerObjects[i].transform.position = goal;
                Transform convertedTransform = ikPointerObjects[i].transform;
                constraints[i].data.target = convertedTransform;

                RigBuilder rigbuilder = GetComponent<RigBuilder>();
                rigbuilder.Build(); //only build if the object is new. Actually, better to just premake all these objects and move them around
            }
        }
    }

    private void SetJointLandmarks()
    {
        if(currentEstimationSource == estimationSource.MediaPipe)
        {
            SetIKPosition(15, AvatarIKGoal.LeftHand);
            SetIKPosition(16, AvatarIKGoal.RightHand);

            SetIKPosition(27, AvatarIKGoal.LeftFoot);
            SetIKPosition(28, AvatarIKGoal.RightFoot);
             
            //Hints:
            SetIKPosition(13, AvatarIKHint.LeftElbow);
            SetIKPosition(25, AvatarIKHint.LeftKnee);
            SetIKPosition(14, AvatarIKHint.RightElbow);
            SetIKPosition(26, AvatarIKHint.RightKnee);

            //Fingers:
            //SetIKPosition(19, "mixamorig:RightHandIndex4");
        }
        else //currently everything except mediapipe and panoptic has these indices for hands
        {
            //Endpoints:
            SetIKPosition(0, AvatarIKGoal.LeftHand);
            SetIKPosition(1, AvatarIKGoal.RightHand);

        }

        if (currentEstimationSource == estimationSource.Panoptic)
        {
            SetIKPosition(8, AvatarIKGoal.LeftFoot);
            SetIKPosition(14, AvatarIKGoal.RightFoot);

            //Hints:
            SetIKPosition(4, AvatarIKHint.LeftElbow);
            SetIKPosition(7, AvatarIKHint.LeftKnee);
            SetIKPosition(10, AvatarIKHint.RightElbow);
            SetIKPosition(13, AvatarIKHint.RightKnee);
        }
    }

    //index is index in the json
    private void SetCenterPosition(int index)
    {
        if (currentEstimationSource == estimationSource.Panoptic)
        {
            Vector3 goal = goalFromIndex(index);
            //GameObject hips = GameObject.Find("mixamorig:Hips");
            //hips.transform.position = goal; //this would be better but it gets reset by the animation controller
            transform.position = goal + centerpointOffset; //lets just set the whole character for now, easier
        }
        else
        {
            //TODO: maybe we could set the center point based on the Head position from our hololens?
            if (currentEstimationSource == estimationSource.MediaPipe)
            {
                //Vector3 midpoint = (goalFromIndex(23) + goalFromIndex(24)) / 2; 
                //Vector3 midpoint = CameraStreamScript.centerLandmarkOffset; //+origin?
                //Debug.Log(midpoint);
                transform.position = origin + 0.5f * (new Vector3(0, -CameraStreamScript.centerLandmarkOffset.y, CameraStreamScript.centerLandmarkOffset.x) - new Vector3(0.0f,0.5f,0.5f)); //the helper's rotation might influence this!
            }
        }
    }

    private void fadeInStart()
    {
        if (smoothing)
        {
            fadeStartTime = Time.time;
            LoadPanopticJSONData(basePath + "Panoptic/" + selectedAnimName + "/body3DScene_" + (frameCount + startFrame).ToString("D8") + ".json");
            fadingIn = true;
        }
    }

    //this should rewrite our landmarks[] array
    private void getDataFromHololens()
    {
        if (TcpScript == null)
        {
            TcpScript = GetComponent<Socket_toHl2>();
        }
        //Debug.Log(TcpScript.position);
        landmarks = new Vector3[Pose.LandmarkIds.Count];
        landmarks[0] = TcpScript.position[0];
    }

    private Vector3[,] savedRecording;
    public void saveRecording(Vector3[,] recording)
    {
        savedRecording = new Vector3[recording.GetLength(0), recording.GetLength(1)];
        CustomDebug.LogAlex("#2 Len saved recording = [" + savedRecording.GetLength(0) + ", " + savedRecording.GetLength(1) + "]");
        Array.Copy(recording, savedRecording, recording.GetLength(0) * recording.GetLength(1));
    }

    private LevelManager LevelManagerScript;
    public void getDataFromRecording()
    {
        LevelManagerScript = GetComponent<LevelManager>();
        frameCount = 0; //Is this needed?
        endFrame = savedRecording.GetLength(0); //TODO: make this more dynamic, use switchToAnimation or fadeIn
        startFrame = 0;


        landmarks = new Vector3[savedRecording.GetLength(1)];
        CustomDebug.LogAlex("Len saved recording = [" + savedRecording.GetLength(0) + ", " + savedRecording.GetLength(1) + "]");

        //copy goalGesture into current landmarks so we can do the animation
        for (int j = 0; j < savedRecording.GetLength(1); j++)
        {
            landmarks[j] = savedRecording[frameCount, j];
        }
    }


    public InteractByPointing PointingScript;
    private void getDataFromPointing()
    {
        if (PointingScript) //TODO: this block is very similar to getDataFromRecording. We could probably move the redundant part to a seperate function
        {
            Vector3[,] builtDirectionMatrix = Gesture.GestureToMatrix(PointingScript.GestureBeingBuilt);
            CustomDebug.LogAlex("BDM len =" + builtDirectionMatrix.GetLength(0) + builtDirectionMatrix.GetLength(1));
            if(builtDirectionMatrix.GetLength(0) > 0)
            {
                landmarks = new Vector3[builtDirectionMatrix.GetLength(1)];
                for (int j = 0; j < builtDirectionMatrix.GetLength(1); j++)
                {
                    landmarks[j] = builtDirectionMatrix[0, j]; //TODO: rotation issue. Character is moving his hand backwards if the pointing hit was in front of it.
                }
            }
        }
        else
        {
            Debug.LogWarning("Trying to follow pointing directions, but can't find the pointing script. Check script parameters.");
        }
    }

    private void getDataFromMediapipeStream()
    {
        if (CameraStreamScript)
        {

            if (CameraStreamScript.vector3List.Count > 0)
            {
                landmarks = new Vector3[CameraStreamScript.vector3List.Count];
                landmarks = CameraStreamScript.vector3List.ToArray();
            }
        }

    }

    public bool moveCenter = false;
    void OnAnimatorIK()
    {
        if (fadingIn && smoothing) //each step of fading in
        {
            fadeReset(false);
        }


        if (landmarks != null && currentEstimationSource != estimationSource.None) //lets be one frame behind
        {
            if (moveCenter)
            {
                SetCenterPosition(2); //move the center to the correct position
            }
            else
            {
                transform.position = origin;
            }
            SetJointLandmarks();
        }

        //do animation
        if ((frameCount <= endFrame || Looping) && !fadingOut && !fadingIn) 
        {
            if (lastProcessedFrame != frameCount + startFrame)
            {
                switch (currentEstimationSource)
                {

                    case (estimationSource.HololensTcp):
                        getDataFromHololens();
                        break;
                    case (estimationSource.Panoptic):
                        LoadPanopticJSONData(basePath + "Panoptic/" + selectedAnimName + "/body3DScene_" + (frameCount + startFrame).ToString("D8") + ".json");
                        break;
                    case (estimationSource.Recording):
                        getDataFromRecording();
                        break;
                    case (estimationSource.PointedDircetions): //TODO: test if this messes up resetting on player's turn to demonstrate
                        getDataFromPointing();
                        break;
                    case (estimationSource.MediaPipe):
                        //LoadMediaPipeJSONData(basePath + "Recordings/" + selectedAnimName + ".json");
                        getDataFromMediapipeStream();
                        break;
                    default: //no data to gather, so reset weights
                        if (landmarks != null)
                        {
                            for (int i = 0; i < landmarks.Length; i++)
                            {
                                landmarks[i] = new Vector3(0, 0, 0);
                            }
                        }
                        editLimbWeights(0);
                        break;
                }
                lastProcessedFrame = frameCount;

            }
        }

        //start fading out to reset
        if (frameCount + startFrame == endFrame - 1 && Looping && !fadingOut && !fadingIn && smoothing) 
        {
            fadingOut = true;
            fadeStartTime = Time.time;
        }
        if (fadingOut) //each step of fading out
        {
            fadeReset();
        }

    }  


    void fadeReset(bool fadingOutWards = true)
    {
        //we still need to set the IK position to smooth out
        SetCenterPosition(2); 
        SetJointLandmarks();

        //set current weight
        float smoothWeight;
        if (fadingOutWards)
        {
            smoothWeight = 1.0f - smoothingFrameCount / (float)smoothingFrames;
        }
        else
        {
            smoothWeight = (float)smoothingFrameCount / smoothingFrames;
        }
        editLimbWeights(smoothWeight);

        //stop resetting if we reached required frames
        if(smoothingFrameCount == smoothingFrames - 1)
        {
            fadingOut = false;
            fadingIn = false;
            animStartTime = Time.time;
            smoothingFrameCount = 0;
            frameCount = 1;
            if (Looping && fadingOutWards && smoothing)
            {
                fadeInStart();
            }
        }
    }

    void editLimbWeights(float newWeight = 1.0f)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, newWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, newWeight);
        
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, newWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, newWeight);


        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, newWeight);
        
    }

    private void LoadPanopticJSONData(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);

        // Parse the JSON data into a data structure
        BodyData bodyData = JsonUtility.FromJson<BodyData>(jsonString);

        // Extract the joint landmarks from the body data

        int vectorCount = bodyData.bodies[0].joints19.Length / 4;
        landmarks = new Vector3[vectorCount];

        for (int i = 0; i < vectorCount; i++)
        {
            int index = i * 4;
            float x = bodyData.bodies[0].joints19[index];
            float y = bodyData.bodies[0].joints19[index + 1];
            float z = bodyData.bodies[0].joints19[index + 2];
            Vector3 vector = new Vector3(x, y, z);
            landmarks[i] = vector;
        }

    }



    //TODO: There must be an easier way to do this, without having to define multiple new classes
    [System.Serializable]
    public class MediaPipeData
    {
        public List<JSONVector3> bodies;
    }

    [System.Serializable]
    public class JSONVector3
    {
        public float x;
        public float y;
        public float z;
    }

    private void LoadMediaPipeJSONData(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);

        MediaPipeData mediaPipeData = JsonUtility.FromJson<MediaPipeData>(jsonString);
        List<JSONVector3> bodyData = mediaPipeData.bodies;

        if (bodyData != null)
        {
            landmarks = new Vector3[bodyData.Count];

            for (int i = 0; i < bodyData.Count; i++)
            {
                JSONVector3 jsonVector = bodyData[i];
                Vector3 vector = new Vector3(jsonVector.x, jsonVector.y, jsonVector.z);
                landmarks[i] = vector;
            }
        }

    }

    void LateUpdate()
    {
        adjustHands();
    }


    void Update()
    {
        float frameRate = 30f; // Desired frame rate (30fps)
        float frameTime = 1f / frameRate; // Time per frame
        if (fadingOut || fadingIn)
        {
           smoothingFrameCount = (Mathf.FloorToInt((Time.time - fadeStartTime) / frameTime) + 1) % smoothingFrames; // Add 1 to start from frame 1, % by all frames of animation
        }
        else{ //TODO: dont have multiple frame counters
            frameCount = (Mathf.FloorToInt((Time.time - animStartTime) / frameTime) + 1) % (endFrame - startFrame); // Add 1 to start from frame 1, % by all frames of animation
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        if (currentEstimationSource == estimationSource.Panoptic)
        {
            DirectoryInfo dir = new DirectoryInfo(basePath + selectedAnimName);
            FileInfo[] info = dir.GetFiles("*.json");

            if (endFrame > info.Length || endFrame < 0)
            {
                endFrame = info.Length; //cull endFrame if set over frame limit
            }
            if (startFrame > endFrame || startFrame < 0)
            {
                startFrame = 0; //cull startFrame if set over frame limit
            }
        }else if (currentEstimationSource == estimationSource.MediaPipe)
        {
            //LoadMediaPipeJSONData(basePath + "Recordings/" + selectedAnimName + ".json");
        }

        animator = GetComponent<Animator>();


        fadeInStart();
    }

}
