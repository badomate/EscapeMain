using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AuxiliarContent;
using System.Linq;
using System;
using UnityEngine.Windows;

public class RecognizeGesture : MonoBehaviour
{
    public int recordingLength = 2; //how many frames do we save for comparison? should match the dictionary
    public int stillnessFramesRequired = 2;


    public bool recording = false;

    public Dictionary<Pose.Landmark, Vector3>[] playerMovementRecord;
    private int recordingProgress = 0; //how many samples of the currently playing gesture have we saved so far
    public float matchThreshold = 0.01f; //0 would mean an absolute perfect match across all samples
    public float stillnessThreshold = 0.1f; //used to "lock in" a pose

    public static UnityEvent StillnessEvent = new UnityEvent();
    //public UnityEvent MimicEvent = new UnityEvent(); 

    private LevelManager LevelManagerScript;

    public GameObject InfoBox;
    // Start is called before the first frame update

    public RiggingIK playerRig;
    void Start()
    {
        LevelManagerScript = GetComponent<LevelManager>();
        playerMovementRecord = new Dictionary<Pose.Landmark, Vector3>[recordingLength];
        StillnessEvent.AddListener(handlleStillness);
    }

    void handlleStillness()
    {
        if(!fingerDown(Pose.Landmark.LEFT_INDEX) && !fingerDown(Pose.Landmark.LEFT_MIDDLE) && fingerDown(Pose.Landmark.LEFT_RING) && fingerDown(Pose.Landmark.LEFT_PINKY))
        {
            InfoBox.SetActive(true);
            Debug.Log("Victory sign detected!!");
        }
        else
        {
            InfoBox.SetActive(false);

        }

    }


    private float frameInterval = 1f / 30f; // 30hz
    private float timeSinceLastFrame = 0f;
    // Update is called once per frame
    void Update()
    {
        timeSinceLastFrame += Time.deltaTime;

        // Check if the desired time interval has passed (30fps)
        if (timeSinceLastFrame >= frameInterval && recording)
        {
            saveGestureFrame();
            detectStillness();  //TODO: only run this function if needed
            /*if (goalGestureCompleted(characterGesture)) //we could use this to detect other gestures too, not just the solution
            {
                if(LevelManagerScript.currentPlayer == 0)
                {
                    recording = false; recordingProgress = 0;
                    LevelManagerScript.Success();
                }
            }*/
            timeSinceLastFrame = 0f; // Reset the time counter
        }
    }

    public void detectStillness()
    {
        // Check if there are enough rows to check for stillness
        if (recordingLength < stillnessFramesRequired)
        {
           Debug.LogWarning("Stillness frames required are greater than the set total recording memory!");
           return;
        }

        // the difference in each element of the last stillnessFramesRequired rows must be under threshold
        for (int recordingIndex = recordingLength - stillnessFramesRequired; recordingIndex < recordingLength -1; recordingIndex++)
        {
            if (playerMovementRecord[recordingIndex] == null)
            {
                return; //recording is incomplete
            }

            //Debug.Log(playerMovementRecord[recordingIndex]);
            foreach (var landmark in playerMovementRecord[recordingIndex].Keys.ToList()) //adjust hand origin
            {
                if (Vector3.Distance(playerMovementRecord[recordingIndex][landmark], playerMovementRecord[recordingIndex + 1][landmark]) > stillnessThreshold)
                {
                    //Debug.Log("This landmark broke the stillness: " + landmark);
                    return; // Difference exceeded the threshold
                }

            }
        }
        if (StillnessEvent != null)
        {
            //Debug.LogWarning("Still!");
            StillnessEvent.Invoke();
        }
        else
        {
            StillnessEvent = new UnityEvent();
            StillnessEvent.Invoke();
        }

    }

    public bool goalGestureCompleted(Vector3[,] gestureToCompareMatrix)
    {
       // Debug.Log("CURRENT GESTURE MATRIX:\n" + gestureToCompareMatrix + "\n\nGOAL GESTURE MATRIX:\n" + LevelManagerScript.goalGesture);

        Gesture gestureToCompare = Gesture.MatrixToGesture(gestureToCompareMatrix);
        Gesture goalGesture = LevelManagerScript.goalGesture;
        //Debug.Log("CURRENT GESTURE:\n" + gestureToCompare + "\n\nGOAL GESTURE:\n" + goalGesture);
        return recording && recordingProgress == recordingLength && goalGesture.GestureMatches(gestureToCompare);
    }

    //We save the gesture's samples received through the mediapipe stream as a matrix and keep comparing it to the goal until they match. Every row is a sample (at 30hz)
    //If the matrix is full, we will throw away the oldest sample so we can keep matrix size the same
    //TODO: we may be taking more samples than we are receiving from mediapipe if the stream rate is low. It would be good to make sure we only call this function when the player's pose has definitely been updated.
    public void saveGestureFrame()
    {
        if (recording)
        {
            Dictionary<Pose.Landmark, Vector3> landmarksCopy = new Dictionary<Pose.Landmark, Vector3>(CameraStream.playerPose._landmarkArrangement);


            if (recordingProgress < recordingLength) //building up the matrix
            {
                foreach (var landmark in landmarksCopy.Keys.ToList()) //adjust hand origin
                {
                    playerMovementRecord[recordingProgress][landmark] = landmarksCopy[landmark];
                }
                recordingProgress++;
            }
            else //updating the matrix
            {
                for (int i = 0; i < playerMovementRecord.GetLength(0) -1; i++)
                {
                    playerMovementRecord[i] = playerMovementRecord[i + 1];
                }

                playerMovementRecord[playerMovementRecord.GetLength(0) - 1] = landmarksCopy;

            }
        }
    }

    //TODO: this function parses a lot of enums as strings and vice-versa, there might be a better apporoach, perhaps by pre-defining the hierarchy of which landmarks belong to which finger or hand
    bool fingerDown(Pose.Landmark fingerTip)
    {
        if (playerMovementRecord[playerMovementRecord.GetLength(0) - 1] == null)
        {
            return false;
        }
        Dictionary<Pose.Landmark, Vector3> poseToExamine = playerMovementRecord[playerMovementRecord.GetLength(0) - 1];


        Pose.Landmark wrist;
        Pose.Landmark fingerMiddle;
        Enum.TryParse(fingerTip + "_KNUCKLE", out fingerMiddle);
        Enum.TryParse(fingerTip.ToString().Substring(fingerTip.ToString().IndexOf('_')) + "WRIST", out wrist);

        if (!poseToExamine.ContainsKey(fingerMiddle) || !poseToExamine.ContainsKey(wrist) || !poseToExamine.ContainsKey(fingerTip))
        {
            return false;
        }

        if (!fingerTip.ToString().Contains("THUMB"))
        {
            if (Vector3.Distance(poseToExamine[fingerMiddle], poseToExamine[wrist]) < Vector3.Distance(poseToExamine[fingerTip], poseToExamine[wrist])) //I'm just using distances here but this could be done with angles
            {
                //Finger is down
                return true;
            }
            else
            {
                return false;
            }
        }
        else //calculate the thumb, it is an exception
        {
            Pose.Landmark elbow;
            Enum.TryParse(fingerTip.ToString().Substring(fingerTip.ToString().IndexOf('_')) + "ELBOW", out elbow);

            Vector3 wristDirection = poseToExamine[wrist] - poseToExamine[elbow];
            Vector3 thumbDirection = poseToExamine[fingerTip] - poseToExamine[fingerMiddle];
            float thumbDirectionDot = Vector3.Dot(wristDirection.normalized, thumbDirection.normalized);
            return thumbDirectionDot < 0;
        }
    }

    //Check if the wrist's rotation is close enough to a given rotation for hand sign recognition
    bool checkWristRot(Quaternion requiredRotation, float threshold, bool leftHand)
    {
        Quaternion currentRotation;
        if (leftHand)
        {
            currentRotation = playerRig.LeftWristTarget.transform.rotation;
        }
        else
        {
            currentRotation = playerRig.RightWristTarget.transform.rotation;
        }

        float diffX = Mathf.Abs(Mathf.DeltaAngle(requiredRotation.eulerAngles.x, currentRotation.eulerAngles.x));
        float diffY = Mathf.Abs(Mathf.DeltaAngle(requiredRotation.eulerAngles.y, currentRotation.eulerAngles.y));
        float diffZ = Mathf.Abs(Mathf.DeltaAngle(requiredRotation.eulerAngles.z, currentRotation.eulerAngles.z));

        return diffX < threshold && diffY < threshold && diffZ < threshold;
    }

    /*
    public float MeanSquaredError(float[,] matrix1, float[,] matrix2)
    {
        int rows = matrix1.GetLength(0);
        int cols = matrix1.GetLength(1);
        float sumOfSquaredDifferences = 0.0f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float diff = matrix1[i, j] - matrix2[i, j];
                sumOfSquaredDifferences += diff * diff;
            }
        }

        return sumOfSquaredDifferences / (rows * cols);
    }*/
}
