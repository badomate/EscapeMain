using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PanopticToIK : MonoBehaviour
{
    private Socket_toHl2 TcpScript;

    public GameObject[] keypointBones = null; //for keypoints that are to be set specifically and not with IK
    protected Animator animator;
    public bool Looping = true; //TODO: fix the False setting, perhaps by introducing a new bool to check for the animation finishing.

    float[] angles;
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

    private string basePath = "assets/PanopticAnimations/";
    public string selectedAnimName = "coverFace";

    //used to not re-read  the same data multiple times if FPS is high
    private int lastProcessedFrame;


    //used to indicate that we are resetting to a starting pose
    public bool smoothing = true;
    private bool fadingOut = false;
    private bool fadingIn = false;


    //get data from hololens
    public bool usingHololensTcp = false;
    public bool usingPanoptic = false;

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
        GameObject rightHand = GameObject.Find("mixamorig:RightHand");
        GameObject leftHand = GameObject.Find("mixamorig:LeftHand");
        rightHand.transform.rotation = rightHand.transform.parent.rotation;
        leftHand.transform.rotation = leftHand.transform.parent.rotation;
    }

    private Vector3 goalFromIndex(int index)
    {
        int offset = 3; //how many coordinates per keypoint, hololens has 3, panoptic has 4
        float neckX = angles[index * offset] * scaleFactor; // x-axis pos
        float neckY = angles[index * offset + 1] * scaleFactor; // y-axis pos
        float neckZ = angles[index * offset + 2] * scaleFactor; // z-axis pos - positive values in Z is going to the right
        return origin + new Vector3(-neckX, neckY, neckZ); //Y is inverted, but how about the others?
    }

    private void SetIKPosition(int index, AvatarIKGoal limb) //, AvatarIKHint hintlimb
    {

        Vector3 goal = goalFromIndex(index);
        animator.SetIKPositionWeight(limb, 1);
        animator.SetIKRotationWeight(limb, 1);
        animator.SetIKPosition(limb, goal);
    }

    private void SetIKPosition(int index, AvatarIKHint limb) //, AvatarIKHint hintlimb
    {
        Vector3 goal = goalFromIndex(index);
        animator.SetIKHintPositionWeight(limb, 1);
        animator.SetIKHintPosition(limb, goal);
    }

    private void SetJointAngles()
    {
        //Endpoints:
        SetIKPosition(0, AvatarIKGoal.LeftHand);
        //SetIKPosition(11, AvatarIKGoal.RightHand);

        if (!usingHololensTcp && usingPanoptic)
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
        if (!usingHololensTcp && usingPanoptic)
        {
            Vector3 goal = goalFromIndex(index);
            //GameObject hips = GameObject.Find("mixamorig:Hips");
            //hips.transform.position = goal; //this would be better but it gets reset by the animation controller
            transform.position = goal + centerpointOffset; //lets just set the whole character for now, easier
        }
        else
        {
            //TODO: maybe we could set the center point based on the Head position from our hololens?
        }
    }

    private void fadeInStart()
    {
        if (smoothing)
        {
            fadeStartTime = Time.time;
            LoadJSONData(basePath + selectedAnimName + "/body3DScene_" + (1 + startFrame).ToString("D8") + ".json");
            fadingIn = true;
        }
    }

    //this should rewrite our angles[] array
    private void getDataFromHololens()
    {
        TcpScript = GetComponent<Socket_toHl2>();
        //Debug.Log(TcpScript.position);
        float[] test  = { TcpScript.position.x, TcpScript.position.y, TcpScript.position.z };
        angles = test;
    }

    public LevelManager LevelManagerScript;
    public void loadHololensRecording()
    {
        LevelManagerScript = GetComponent<LevelManager>();
        frameCount = 0;
        endFrame = LevelManagerScript.goalGesture.GetLength(0); //TODO: make this more dynamic, use switchToAnimation or fadeIn
        startFrame = 0;


        angles = new float[LevelManagerScript.goalGesture.GetLength(1)];

        //copy goalGesture into current angles so we can do the animation
        for (int j = 0; j < LevelManagerScript.goalGesture.GetLength(1); j++)
        {
            angles[j] = LevelManagerScript.goalGesture[frameCount, j];
        }
    }

    void OnAnimatorIK()
    {
        if (fadingIn && smoothing) //each step of fading in
        {
            fadeReset(false);
        }

        //do animation
        if ((frameCount <= endFrame || Looping) && !fadingOut && !fadingIn) 
        {
            if(lastProcessedFrame != frameCount + startFrame)
            {
                if (usingHololensTcp)
                {
                    getDataFromHololens();
                }
                else if(usingPanoptic){
                    LoadJSONData(basePath + selectedAnimName + "/body3DScene_" + (frameCount + startFrame).ToString("D8") + ".json");
                }
                else
                {
                    loadHololensRecording();
                }
                lastProcessedFrame = frameCount;
            }
            if(angles != null)
            {
                SetCenterPosition(2); //move the center to the correct position
                SetJointAngles();
            }
            else
            {
                Debug.Log("No data was found by IK animator");
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
        SetJointAngles();

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
        /*
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, newWeight);

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, newWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, newWeight);


        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, newWeight);
        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, newWeight);
        */
    }

    private void LoadJSONData(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);

        // Parse the JSON data into a data structure
        BodyData bodyData = JsonUtility.FromJson<BodyData>(jsonString);

        // Extract the joint angles from the body data
        angles = bodyData.bodies[0].joints19;

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
        DirectoryInfo dir = new DirectoryInfo(basePath + selectedAnimName);
        FileInfo[] info = dir.GetFiles("*.json");

        if(endFrame > info.Length || endFrame < 0)
        {
            endFrame = info.Length; //cull endFrame if set over frame limit
        }
        if (startFrame > endFrame || startFrame < 0)
        {
            startFrame = 0; //cull startFrame if set over frame limit
        }

        animator = GetComponent<Animator>();

        fadeInStart();
    }

}
