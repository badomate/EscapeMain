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

    public static Dictionary<Pose.Landmark, Vector3>[] playerMovementRecord;
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
    public GameObject playerRoot;
    void Start()
    {
        LevelManagerScript = GetComponent<LevelManager>();
        playerMovementRecord = new Dictionary<Pose.Landmark, Vector3>[recordingLength];
        StillnessEvent.AddListener(handleStillness);
    }

    void handleStillness()
    {
        //Watch out: Depending on mediapipe configuration, left and right hand indicators may be flipped 
        bool isLeftHandStraight = isJointStraight(Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_ELBOW, Pose.Landmark.LEFT_WRIST, 45f);
        bool isRightHandStraight = isJointStraight(Pose.Landmark.RIGHT_SHOULDER, Pose.Landmark.RIGHT_ELBOW, Pose.Landmark.RIGHT_WRIST, 45f);

        bool isLeftHandLeveled = isJointLeveled(Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_WRIST, 0.3f);
        bool isRightHandLeveled = isJointLeveled(Pose.Landmark.RIGHT_SHOULDER, Pose.Landmark.RIGHT_WRIST, 0.3f);

        //Debug.Log(" LEFT_THUMB." + fingerDown(Pose.Landmark.LEFT_THUMB) + "LEFT_MIDDLE:" + fingerDown(Pose.Landmark.LEFT_MIDDLE) + "LEFT_RING: " + fingerDown(Pose.Landmark.LEFT_RING) + "LEFT_PINKY: " + fingerDown(Pose.Landmark.LEFT_PINKY));
        Debug.Log(wristRotTargetRight.transform.eulerAngles);

        //Shapes
        bool isCircle = fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY);


        bool isSquare = fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          isWristRotation(true, Quaternion.Euler(300, 0, 50), 45);

        //Colors
        bool isBlue = (!fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          !fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.RIGHT_RING) &&
                          !fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          isWristRotation(false, Quaternion.Euler(300, 60, 300), 45))
        && (fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          isWristRotation(true, Quaternion.Euler(0, 300, 150), 45));

        bool isRed = (fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          !fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY))
        || (fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          !fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY));
        //Directions

        bool isDirectionForward = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(0, 330, 70), 45) &&
                          fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          fingerDown(Pose.Landmark.RIGHT_THUMB) &&
                          isWristRotation(false, Quaternion.Euler(0, 120, 260), 45);


        bool isDirectionRight = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(0, 250, 100), 45) &&
                          fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          fingerDown(Pose.Landmark.RIGHT_THUMB) &&
                          isWristRotation(false, Quaternion.Euler(0, 75, 280), 45);


        bool isDirectionLeft = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(0, 0, 90), 45) &&
                          fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          fingerDown(Pose.Landmark.RIGHT_RING) &&
                          fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          fingerDown(Pose.Landmark.RIGHT_THUMB) &&
                          isWristRotation(false, Quaternion.Euler(0, 200, 260), 45);

        //Feedback
        bool isYes = fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          fingerDown(Pose.Landmark.LEFT_RING) &&
                          fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          !fingerDown(Pose.Landmark.LEFT_THUMB) &&
                          isWristRotation(true, Quaternion.Euler(0, 60, 90), 45);

        bool isHello = isWristRotation(true, Quaternion.Euler(330, 230, 170), 30) &&
                          !fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          !fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.LEFT_RING) &&
                          !fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          !fingerDown(Pose.Landmark.LEFT_THUMB) ||
                          isWristRotation(false, Quaternion.Euler(0, 180, 180), 30) &&
                          !fingerDown(Pose.Landmark.RIGHT_INDEX) &&
                          !fingerDown(Pose.Landmark.RIGHT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.RIGHT_RING) &&
                          !fingerDown(Pose.Landmark.RIGHT_PINKY) &&
                          !fingerDown(Pose.Landmark.RIGHT_THUMB);



        bool isNewWord = isWristRotation(true, Quaternion.Euler(45, 250, 0), 30) &&
                          !fingerDown(Pose.Landmark.LEFT_INDEX) &&
                          !fingerDown(Pose.Landmark.LEFT_MIDDLE) &&
                          !fingerDown(Pose.Landmark.LEFT_RING) &&
                          !fingerDown(Pose.Landmark.LEFT_PINKY) &&
                          !fingerDown(Pose.Landmark.LEFT_THUMB);



        if (isCircle)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.CAMERA_RIGHT);
        }else if (isHello)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.CAMERA_LEFT);
        }
        else if (isYes)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.VICTORY);
        }
        else if (isRed)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.SUPERMAN);
        }
        else if (isDirectionForward)
        {
            InfoBox.SetActive(true);
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_FORWARD);
        }

    }

    bool isWristRotation(bool leftHand, Quaternion targetRotation, int threshold)
    {
        GameObject wristRotator = leftHand ? wristRotTargetLeft : wristRotTargetRight;

        Quaternion playerRootRotation = playerRoot.transform.rotation;

        //TODO - The following lines is an attempt to make up for body rotation when using absolute coordinates. I have not had the chance to test it so it is disabled for now.
        //Quaternion relativeWristRotation = Quaternion.Inverse(playerRootRotation) * wristRotator.transform.rotation;
        float angleDifference = Quaternion.Angle(wristRotator.transform.rotation, targetRotation);

        /*if (!leftHand)
        {
            Debug.Log("Original: " + relativeWristRotation.eulerAngles + "; Inverted: " + wristRotator.transform.eulerAngles);
        }
        */
        return angleDifference <= threshold;
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

            foreach (var landmark in playerMovementRecord[recordingIndex].Keys.ToList()) //adjust hand origin
            {
                if (Vector3.Distance(playerMovementRecord[recordingIndex][landmark], playerMovementRecord[recordingIndex + 1][landmark]) > stillnessThreshold)
                {
                    return false; // Difference exceeded the threshold
                }

            }
        }
        if (StillnessEvent != null)
        {
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
        Gesture gestureToCompare = Gesture.MatrixToGesture(gestureToCompareMatrix);
        Gesture goalGesture = LevelManagerScript.goalGesture;
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

        Enum.TryParse(fingerTip.ToString().Substring(0, fingerTip.ToString().IndexOf('_') + 1) + "WRIST_ROOT", out wrist);

            if (!poseToExamine.ContainsKey(fingerMiddle) || !poseToExamine.ContainsKey(wrist) || !poseToExamine.ContainsKey(fingerTip))
        {
            return false;
        }

        if (!fingerTip.ToString().Contains("THUMB"))
        {
            if (Vector3.Distance(poseToExamine[fingerMiddle], poseToExamine[wrist]) < Vector3.Distance(poseToExamine[fingerTip], poseToExamine[wrist])) //I'm just using distances here but this could be done with angles
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else //calculate the thumb, it is an exception
        {
            Pose.Landmark wristLeft;
            Pose.Landmark wristRight;
            Enum.TryParse(fingerTip.ToString().Substring(0, fingerTip.ToString().IndexOf('_') + 1) + "INDEX_BASE", out wristLeft);
            Enum.TryParse(fingerTip.ToString().Substring(0, fingerTip.ToString().IndexOf('_') + 1) + "PINKY_BASE", out wristRight);

            float toleranceThreshold = 90f;
            Vector3 wristDirection = poseToExamine[wristRight] - poseToExamine[wristLeft];
            Vector3 thumbDirection = poseToExamine[fingerTip] - poseToExamine[fingerMiddle];

            float thumbDirectionDot = Vector3.Angle(wristDirection.normalized, thumbDirection.normalized);

            bool thumbInsidePalm = thumbDirectionDot < toleranceThreshold;
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



}
