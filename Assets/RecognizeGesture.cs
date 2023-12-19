using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AuxiliarContent;
using System.Linq;
using System;
using UnityEngine.Windows;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Unity.VisualScripting;

public enum Actions
{
    GO_LEFT,
    GO_RIGHT,
    GO_FORWARD,
    GO_BACKWARD,

    TURN_LEFT,
    TURN_RIGHT,

    VICTORY,
    SUPERMAN,
    UNRECOGNIZED,
    AMBIGUOUS,

    CAMERA_LEFT,
    CAMERA_RIGHT,
}

public class RecognizeGesture : MonoBehaviour
{
    public int recordingLength = 2; // how many frames do we save for comparison? should match the dictionary
    public int stillnessFramesRequired = 2;


    public bool recording = false;

    public Dictionary<Pose.Landmark, Vector3>[] playerMovementRecord;
    private int recordingProgress = 0; // how many samples of the currently playing gesture have we saved so far
    public float matchThreshold = 0.01f; // 0 would mean an absolute perfect match across all samples
    public float stillnessThreshold = 0.1f; // used to "lock in" a pose

    public static UnityEvent StillnessEvent = new UnityEvent();
    // public UnityEvent MimicEvent = new UnityEvent(); 

    public delegate void RecognitionEventDel(Actions action);
    public static RecognitionEventDel RecognitionEvent;

    private LevelManager LevelManagerScript;

    public GameObject InfoBox;
    // Start is called before the first frame update

    public RiggingIK playerRig;
    public GameObject wristRotTargetLeft;
    public GameObject wristRotTargetRight;
    void Start()
    {
        LevelManagerScript = GetComponent<LevelManager>();
        playerMovementRecord = new Dictionary<Pose.Landmark, Vector3>[recordingLength];
        StillnessEvent.AddListener(handleStillness);
    }

    void handleStillness()
    {
        bool isLeftHandStraight = isJointStraight(Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_ELBOW, Pose.Landmark.LEFT_WRIST, 45f);
        bool isRightHandStraight = isJointStraight(Pose.Landmark.RIGHT_SHOULDER, Pose.Landmark.RIGHT_ELBOW, Pose.Landmark.RIGHT_WRIST, 45f);

        bool isLeftHandLeveled = isJointLeveled(Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_WRIST, 0.3f);
        bool isRightHandLeveled = isJointLeveled(Pose.Landmark.RIGHT_SHOULDER, Pose.Landmark.RIGHT_WRIST, 0.3f);

        //Debug.Log(wristRotTargetLeft.transform.eulerAngles);
        bool isHello =    isWristRotation(true, Quaternion.Euler(0, 240, 180), 45f) &&
                          !fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          !fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.LEFT_RING) &&
                          !fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          isJointAbove(Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_WRIST, 0.3f);

        bool isCircle = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB);

        /*
        bool isRed = (fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          !fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB))||
                          fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          !fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          fingerDown(Pose.Landmark.RIGHT_THUMB);



        bool isThumbsUp = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          !fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(180, 240, 90), 45f);


        bool isDirectionLeft = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(180, 240, -90), 45f) &&
                          fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          fingerDown(Pose.Landmark.RIGHT_THUMB) &&
                          isWristRotation(false, Quaternion.Euler(180, 240, -90), 45f);
        */
        if (isCircle)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.CAMERA_RIGHT);
        }
        /*
        if (turnCameraLeft)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.CAMERA_LEFT);
        }
        else if (turnCameraRight)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.CAMERA_RIGHT);
        }

        if (Truth(isVictory, isGoBackward, isGoForward, isSuperman, isTurnLeft, isGoLeft, isTurnRight, isGoRight) >= 2)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.AMBIGUOUS);
        }
        else if (isVictory)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.VICTORY);
        }
        else if (isSuperman)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.SUPERMAN);
        }
        else if (isGoForward)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_FORWARD);
        }
        else if (isGoBackward)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_BACKWARD);
        }
        else if (isGoRight)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_RIGHT);

        }
        else if (isTurnRight)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.TURN_RIGHT);
        }
        else if (isGoLeft)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_LEFT);
        }
        else if (isTurnLeft)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.TURN_LEFT);
        }
        else
        {
            InfoBox.SetActive(false);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.UNRECOGNIZED);

        }*/

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
            if (detectStillness())
            {
                StillnessEvent.Invoke();
            }
            else
            {
                RecognizeGesture.RecognitionEvent.Invoke(Actions.UNRECOGNIZED);
            }
            timeSinceLastFrame = 0f; // Reset the time counter
        }
    }
    public static int Truth(params bool[] booleans)
    {
        return booleans.Count(b => b);
    }
    public bool detectStillness()
    {
        // Check if there are enough rows to check for stillness
        if (recordingLength < stillnessFramesRequired)
        {
            Debug.LogWarning("Stillness frames required are greater than the set total recording memory!");
            return false;
        }

        // the difference in each element of the last stillnessFramesRequired rows must be under threshold
        for (int recordingIndex = recordingLength - stillnessFramesRequired; recordingIndex < recordingLength - 1; recordingIndex++)
        {
            if (playerMovementRecord[recordingIndex] == null)
            {
                return false; //recording is incomplete
            }

            //Debug.Log(playerMovementRecord[recordingIndex]);
            foreach (var landmark in playerMovementRecord[recordingIndex].Keys.ToList()) //adjust hand origin
            {
                if (Vector3.Distance(playerMovementRecord[recordingIndex][landmark], playerMovementRecord[recordingIndex + 1][landmark]) > stillnessThreshold)
                {
                    //Debug.Log("This landmark broke the stillness: " + landmark);
                    return false; // Difference exceeded the threshold
                }

            }
        }
        if (StillnessEvent != null)
        {
            //Debug.LogWarning("Still!");
            return true;
        }
        else
        {
            StillnessEvent = new UnityEvent();
            return true;
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
                for (int i = 0; i < playerMovementRecord.GetLength(0) - 1; i++)
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
        Enum.TryParse(fingerTip.ToString().Substring(0, fingerTip.ToString().IndexOf('_') + 1) + "WRIST", out wrist);

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
            Pose.Landmark wristMiddle;
            Enum.TryParse(fingerTip.ToString().Substring(0, fingerTip.ToString().IndexOf('_') + 1) + "MIDDLE_BASE", out wristMiddle);

            float toleranceThreshold = 0.02f; // Adjust this threshold as needed
            Vector2 wristDirection = poseToExamine[wristMiddle] - poseToExamine[wrist];
            Vector2 thumbDirection = poseToExamine[fingerTip] - poseToExamine[fingerMiddle];

            bool thumbInsidePalm = (thumbDirection.x * wristDirection.y - thumbDirection.y * wristDirection.x) > -toleranceThreshold;
            Debug.Log((thumbDirection.x * wristDirection.y - thumbDirection.y * wristDirection.x));
            return thumbInsidePalm;
        }
    }

    bool isJointLeveled(Pose.Landmark start, Pose.Landmark end, float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][start];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][end];

            Vector3 v = endVect - startVect;

            v = Vector3.Normalize(v);

            return Math.Abs(v.y) < margin && Math.Abs(v.z) < margin;
        }
        catch
        {
            return false;
        }
    }
    bool isJointAbove(Pose.Landmark start, Pose.Landmark end, float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][start];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][end];

            Vector3 v = endVect - startVect;

            v = Vector3.Normalize(v);

            return Math.Abs(v.z) < margin && Math.Abs(v.x) < margin && 0 < v.y;
        }
        catch
        {
            return false;
        }
    }

    bool isJoint90Degrees(Pose.Landmark start, Pose.Landmark middle, Pose.Landmark end, float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][start];
            Vector3 middleVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][middle];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][end];

            Vector3 V1 = middleVect - startVect;
            Vector3 V2 = endVect - middleVect;

            V1 = Vector3.Normalize(V1);
            V2 = Vector3.Normalize(V2);

            Vector3 rotationAxis = Vector3.Cross(V1, V2);

            float cosTheta = Vector3.Dot(V1, V2);

            return cosTheta < margin;
        }
        catch
        {
            return false;
        }
    }

    bool isJointStraight(Pose.Landmark start, Pose.Landmark middle, Pose.Landmark end, float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][start];
            Vector3 middleVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][middle];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][end];

            Vector3 V1 = middleVect - startVect;
            Vector3 V2 = endVect - middleVect;

            V1 = Vector3.Normalize(V1);
            V2 = Vector3.Normalize(V2);

            Vector3 rotationAxis = Vector3.Cross(V1, V2);

            float cosTheta = Vector3.Dot(V1, V2);
            float theta = (float)Math.Acos(cosTheta) * 180 / (float)Math.PI;

            return theta < margin;
        }
        catch
        {
            return false;
        }
    }

    bool isRightLegRaised(float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][Pose.Landmark.LEFT_FOOT];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][Pose.Landmark.RIGHT_FOOT];

            Vector3 v = endVect - startVect;

            v = Vector3.Normalize(v);

            return margin < v.y;
        }
        catch
        {
            return false;
        }
    }

    bool isLeftLegRaised(float margin)
    {
        try
        {
            Vector3 startVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][Pose.Landmark.LEFT_FOOT];
            Vector3 endVect = playerMovementRecord[playerMovementRecord.GetLength(0) - 1][Pose.Landmark.RIGHT_FOOT];

            Vector3 v = startVect - endVect;

            v = Vector3.Normalize(v);

            return margin < v.y;
        }
        catch
        {
            return false;
        }
    }


    bool isWristRotation(bool leftHand, Quaternion targetRotation, float threshold)
    {
        GameObject wristRotator;
        if (leftHand)
        {
            wristRotator = wristRotTargetLeft;
        }
        else
        {
            wristRotator = wristRotTargetRight;
        }
        float angleDifference = Quaternion.Angle(wristRotator.transform.rotation, targetRotation);
        //Debug.Log(angleDifference);
        return angleDifference <= threshold;

    }



}
