using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
///This class should serve some of the same functions as EstimationToIk 
///but will perform those actions using the Animation Rigging system
///It does not inherit from it since EstimationToIk is heavily based on OnAnimatorIK which is unused here
/// </summary>
public class RiggingIK : MonoBehaviour
{
    Pose currentPose;
    public Dictionary<Pose.Landmark, GameObject> landmarkToTarget = new Dictionary<Pose.Landmark, GameObject>();

    public static Dictionary<GameObject, Vector3> targetToPlayerBasePosition = new Dictionary<GameObject, Vector3>();
    public static Dictionary<Pose.Landmark, Vector3> landmarkToModelBasePosition = new Dictionary<Pose.Landmark, Vector3>();

    Animator animator;

    //main landmarks
    public GameObject RightHandTarget;
    public GameObject LeftHandTarget;
    public GameObject RightFootTarget;
    public GameObject LeftFootTarget;

    //hint landmarks
    public GameObject RightElbowHintTarget;
    public GameObject LeftElbowHintTarget;

    public GameObject RightShoulderTarget;
    public GameObject LeftShoulderTarget;

    //hand landmarks
    public GameObject RightIndexTarget;
    public GameObject RightIndexHintTarget;
    public GameObject RightMiddleTarget;
    public GameObject RightMiddleHintTarget;
    public GameObject RightRingTarget;
    public GameObject RightRingHintTarget;
    public GameObject RightPinkyTarget;
    public GameObject RightPinkyHintTarget;
    public GameObject RightThumbTarget;
    public GameObject RightThumbHintTarget;

    public GameObject LeftIndexTarget;
    public GameObject LeftIndexHintTarget;
    public GameObject LeftMiddleTarget;
    public GameObject LeftMiddleHintTarget;
    public GameObject LeftRingTarget;
    public GameObject LeftRingHintTarget;
    public GameObject LeftPinkyTarget;
    public GameObject LeftPinkyHintTarget;
    public GameObject LeftThumbTarget;
    public GameObject LeftThumbHintTarget;

    public GameObject ShoulderTarget;
    public GameObject LeftWristTarget;
    public GameObject RightWristTarget;

    public GameObject LeftEarTarget;
    public GameObject RightEarTarget;
    public GameObject NoseTarget;
    public GameObject HeadTarget;
    public GameObject RightHipTarget;
    public GameObject LeftHipTarget;
    public GameObject HipTarget;
    public GameObject LeftKneeHintTarget;
    public GameObject RightKneeHintTarget;
    public ChainIKConstraint pointingConstraint;

    public bool mirroring = false;
    public bool gesturePlaySmoothing = true;
    public bool crouch = true;
    public float shoulderOffsetScale = 0.1f;

    bool lockedInCalibration = false;
    public bool useCalibration = false;
    public bool reshapeModelForCalibration = false;
    public bool calibrateFingers = true; //turn off if model doesnt have fingers anyway
    public bool useWorldCoordinates = false;

    public GameObject skeletonRoot;

    public GameObject LeftUpperArmBone;
    public GameObject LeftLowerArmBone;

    public GameObject RightUpperArmBone;
    public GameObject RightLowerArmBone;

    public GameObject LeftUpperLegBone;
    public GameObject RightUpperLegBone;


    public GameObject LeftIndexBase;
    public GameObject LeftThumbBase;
    public GameObject LeftRingBase;
    public GameObject LeftPinkyBase;
    public GameObject LeftMiddleBase;

    public GameObject RightIndexBase;
    public GameObject RightThumbBase;
    public GameObject RightRingBase;
    public GameObject RightPinkyBase;
    public GameObject RightMiddleBase;


    public RigBuilder rigBuilder;

    List<Pose.Landmark> rightFingers = new List<Pose.Landmark> {
        Pose.Landmark.RIGHT_INDEX,
        Pose.Landmark.RIGHT_THUMB,
        Pose.Landmark.RIGHT_RING,
        Pose.Landmark.RIGHT_PINKY,
        Pose.Landmark.RIGHT_MIDDLE,

        Pose.Landmark.RIGHT_INDEX_KNUCKLE,
        Pose.Landmark.RIGHT_THUMB_KNUCKLE,
        Pose.Landmark.RIGHT_RING_KNUCKLE,
        Pose.Landmark.RIGHT_PINKY_KNUCKLE,
        Pose.Landmark.RIGHT_MIDDLE_KNUCKLE,

        Pose.Landmark.RIGHT_INDEX_BASE,
        Pose.Landmark.RIGHT_THUMB_BASE,
        Pose.Landmark.RIGHT_RING_BASE,
        Pose.Landmark.RIGHT_PINKY_BASE,
        Pose.Landmark.RIGHT_MIDDLE_BASE};

    List<Pose.Landmark> leftFingers = new List<Pose.Landmark> {
        Pose.Landmark.LEFT_INDEX,
        Pose.Landmark.LEFT_THUMB,
        Pose.Landmark.LEFT_RING,
        Pose.Landmark.LEFT_PINKY,
        Pose.Landmark.LEFT_MIDDLE,

        Pose.Landmark.LEFT_INDEX_KNUCKLE,
        Pose.Landmark.LEFT_THUMB_KNUCKLE,
        Pose.Landmark.LEFT_RING_KNUCKLE,
        Pose.Landmark.LEFT_PINKY_KNUCKLE,
        Pose.Landmark.LEFT_MIDDLE_KNUCKLE,

        Pose.Landmark.LEFT_INDEX_BASE,
        Pose.Landmark.LEFT_THUMB_BASE,
        Pose.Landmark.LEFT_RING_BASE,
        Pose.Landmark.LEFT_PINKY_BASE,
        Pose.Landmark.LEFT_MIDDLE_BASE};

    //this is just to simplify
    public void SetIKPositions(Pose poseToPlay, bool relative = false)
    {
        SetIKPositions(poseToPlay._landmarkArrangement, relative);
    }

    //Changes every IK target to match up with the given pose
    public void SetIKPositions(Dictionary<Pose.Landmark, Vector3> landmarkArrangement, bool relative = false)
    {
        Dictionary<Pose.Landmark, Vector3> landmarksCopy = new Dictionary<Pose.Landmark, Vector3>(landmarkArrangement); //Dictoinary must to be copied before we do the iteration, or we get errors for having it changed by the animation thread in the middle of it.


        //ROTATE LANDMARKS BY INITIAL GAMEOBJECT ROTATION
        foreach (var landmark in landmarksCopy.Keys.ToList()) //TODO: use the built-in Pose version of this instead for clarity, but it's a bit tricky since we are copying it over
        {
            if (relative)
            {
                landmarksCopy[landmark] = gameObject.transform.rotation * landmarksCopy[landmark];
            }
        }


        //MAKE FINGERS RELATIVE TO WRIST POSITION
        foreach (var landmark in landmarksCopy.Keys.ToList()) //adjust hand origin
        {
            if (leftFingers.Contains(landmark))
            {
                landmarksCopy[landmark] -= landmarksCopy[Pose.Landmark.LEFT_WRIST_ROOT];
                landmarksCopy[landmark] += landmarksCopy[Pose.Landmark.LEFT_WRIST];
            }

            if (rightFingers.Contains(landmark))
            {
                landmarksCopy[landmark] -= landmarksCopy[Pose.Landmark.RIGHT_WRIST_ROOT];
                landmarksCopy[landmark] += landmarksCopy[Pose.Landmark.RIGHT_WRIST];
            }

        }

        //RETARGET REAL TO MODEL - if we are not reshaping the model, start by reshaping the received coordinates to match us
        if (useCalibration && !reshapeModelForCalibration)
        {
            //Calibrate limbs
            calibrateLimb(landmarksCopy, RightUpperArmBone, Pose.Landmark.RIGHT_SHOULDER, Pose.Landmark.RIGHT_ELBOW, Pose.Landmark.RIGHT_WRIST);
            calibrateLimb(landmarksCopy, LeftUpperArmBone, Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.LEFT_ELBOW, Pose.Landmark.LEFT_WRIST);

            calibrateLimb(landmarksCopy, LeftUpperLegBone, Pose.Landmark.LEFT_HIP, Pose.Landmark.LEFT_KNEE, Pose.Landmark.LEFT_FOOT);
            calibrateLimb(landmarksCopy, RightUpperLegBone, Pose.Landmark.RIGHT_HIP, Pose.Landmark.RIGHT_KNEE, Pose.Landmark.RIGHT_FOOT);


            //Calibrate fingers
            if (landmarksCopy.ContainsKey(Pose.Landmark.RIGHT_INDEX_BASE) && calibrateFingers)
            {
                calibrateLimb(landmarksCopy, RightIndexBase, Pose.Landmark.RIGHT_INDEX_BASE, Pose.Landmark.RIGHT_INDEX_KNUCKLE, Pose.Landmark.RIGHT_INDEX);
                calibrateLimb(landmarksCopy, RightThumbBase, Pose.Landmark.RIGHT_THUMB_BASE, Pose.Landmark.RIGHT_THUMB_KNUCKLE, Pose.Landmark.RIGHT_THUMB);
                calibrateLimb(landmarksCopy, RightMiddleBase, Pose.Landmark.RIGHT_MIDDLE_BASE, Pose.Landmark.RIGHT_MIDDLE_KNUCKLE, Pose.Landmark.RIGHT_MIDDLE);
                calibrateLimb(landmarksCopy, RightRingBase, Pose.Landmark.RIGHT_RING_BASE, Pose.Landmark.RIGHT_RING_KNUCKLE, Pose.Landmark.RIGHT_RING);
                calibrateLimb(landmarksCopy, RightPinkyBase, Pose.Landmark.RIGHT_PINKY_BASE, Pose.Landmark.RIGHT_PINKY_KNUCKLE, Pose.Landmark.RIGHT_PINKY);
            }
            if (landmarksCopy.ContainsKey(Pose.Landmark.LEFT_INDEX_BASE) && calibrateFingers)
            {
                calibrateLimb(landmarksCopy, LeftIndexBase, Pose.Landmark.LEFT_INDEX_BASE, Pose.Landmark.LEFT_INDEX_KNUCKLE, Pose.Landmark.LEFT_INDEX);
                calibrateLimb(landmarksCopy, LeftThumbBase, Pose.Landmark.LEFT_THUMB_BASE, Pose.Landmark.LEFT_THUMB_KNUCKLE, Pose.Landmark.LEFT_THUMB);
                calibrateLimb(landmarksCopy, LeftMiddleBase, Pose.Landmark.LEFT_MIDDLE_BASE, Pose.Landmark.LEFT_MIDDLE_KNUCKLE, Pose.Landmark.LEFT_MIDDLE);
                calibrateLimb(landmarksCopy, LeftRingBase, Pose.Landmark.LEFT_RING_BASE, Pose.Landmark.LEFT_RING_KNUCKLE, Pose.Landmark.LEFT_RING);
                calibrateLimb(landmarksCopy, LeftPinkyBase, Pose.Landmark.LEFT_PINKY_BASE, Pose.Landmark.LEFT_PINKY_KNUCKLE, Pose.Landmark.LEFT_PINKY);
            }

        }
        

        //SET TARGET POSITIONS
        foreach (var kvp in landmarksCopy)
        {
            Pose.Landmark landmark = kvp.Key;
            Vector3 position = kvp.Value;

            //Check if we have a target to adjust for this landmark
            if (landmarkToTarget.ContainsKey(landmark) && landmarkToTarget[landmark] != null)
            {
                GameObject landmarkTarget = landmarkToTarget[landmark];

                //Move the target gameobject to the position our Pose specified
                if (relative && !useWorldCoordinates)
                {
                    landmarkTarget.transform.position = position + gameObject.transform.position;
                }
                else
                {
                    landmarkTarget.transform.position = position;
                }
            }
        }
        setTargetBetweenlandmarks(landmarksCopy, Pose.Landmark.LEFT_SHOULDER, Pose.Landmark.RIGHT_SHOULDER, ShoulderTarget, shoulderOffsetScale);
        
        setRotationFromTriangle(landmarksCopy, Pose.Landmark.LEFT_PINKY_BASE, Pose.Landmark.LEFT_INDEX_BASE, Pose.Landmark.LEFT_WRIST, LeftWristTarget, Quaternion.Euler(0, 0, 0));
        setRotationFromTriangle(landmarksCopy, Pose.Landmark.RIGHT_INDEX_BASE, Pose.Landmark.RIGHT_PINKY_BASE, Pose.Landmark.RIGHT_WRIST, RightWristTarget, Quaternion.Euler(0, 0, 0));

        setRotationFromTriangle(landmarksCopy, Pose.Landmark.LEFT_EAR, Pose.Landmark.RIGHT_EAR, Pose.Landmark.NOSE, HeadTarget, Quaternion.Euler(-90,0,0));

        setTargetBetweenlandmarks(landmarksCopy, Pose.Landmark.LEFT_HIP, Pose.Landmark.RIGHT_HIP, HipTarget);

        
        if (useWorldCoordinates)
        {
           skeletonRoot.transform.position = HipTarget.transform.position;
        }

        if (crouch) //TODO: check why this wasn't working properly last demo
        {
            float distFromFloor = 0;
            if (landmarkToTarget.ContainsKey(Pose.Landmark.LEFT_FOOT) && landmarkToTarget.ContainsKey(Pose.Landmark.RIGHT_FOOT) && LeftFootTarget && RightFootTarget)
            {
                if (landmarkToTarget[Pose.Landmark.LEFT_FOOT].transform.position.y != 0 || landmarkToTarget[Pose.Landmark.RIGHT_FOOT].transform.position.y != 0)
                    distFromFloor = -Math.Min(landmarkToTarget[Pose.Landmark.LEFT_FOOT].transform.position.y, landmarkToTarget[Pose.Landmark.RIGHT_FOOT].transform.position.y);

            }

            if (distFromFloor != 0)
            {
                foreach (var target in landmarkToTarget)
                {
                    if (target.Value != null)
                    {
                        float x = target.Value.transform.position.x;
                        float y = target.Value.transform.position.y;
                        float z = target.Value.transform.position.z;
                        target.Value.transform.position = new Vector3(x, y + 1 - 0.938785f + distFromFloor, z);
                    }
                }
            }
        }

    }

    void calibrateLimb(Dictionary<Pose.Landmark, Vector3> landmarksCopy, GameObject rootBone, Pose.Landmark rootLandmark, Pose.Landmark midLandmark, Pose.Landmark endLandmark)
    {
        Vector3 rootShift = rootBone.transform.position - (transform.position + landmarksCopy[rootLandmark]);
        
        Vector3 extraShift = ElongateLimb(landmarksCopy, midLandmark, rootLandmark);
        ElongateLimb(landmarksCopy, endLandmark, midLandmark, extraShift);
        landmarksCopy[midLandmark] += rootShift;
        landmarksCopy[endLandmark] += rootShift;
    }

    /* //Function for reshaping the model to resemble the person
    void ElongateModelLimb(GameObject goalTarget, GameObject sourceTarget, Pose.Landmark goalLandmark, Pose.Landmark sourceLandmark, GameObject boneToScale)
    {
        Vector3 realPose = targetToPlayerBasePosition[goalTarget] - targetToPlayerBasePosition[sourceTarget];
        Vector3 modelPose = targetToModelBasePosition[goalTarget] - targetToModelBasePosition[sourceTarget];
        float realMagnitude = realPose.magnitude;
        float modelMagnitude = modelPose.magnitude;

        float scaleVar = realMagnitude / modelMagnitude;

        boneToScale.transform.localScale = new Vector3(scaleVar, scaleVar, scaleVar);
    }*/

    //retargets the REAL estimation onto the model and returns the vector so that it may be applied to its children
    Vector3 ElongateLimb(Dictionary<Pose.Landmark, Vector3> landmarksCopy, Pose.Landmark goalLandmark, Pose.Landmark sourceLandmark, Vector3 extraShift = default(Vector3))
    {
        landmarksCopy[goalLandmark] += extraShift;
        Vector3 realPose = landmarksCopy[goalLandmark] - landmarksCopy[sourceLandmark];
        Vector3 modelPose = landmarkToModelBasePosition[goalLandmark] - landmarkToModelBasePosition[sourceLandmark];
        float realMagnitude = realPose.magnitude;
        float modelMagnitude = modelPose.magnitude;

        //float scaleVar = (modelMagnitude - realMagnitude)/ realMagnitude;
        float scaleVar = (modelMagnitude- realMagnitude) / realMagnitude;

        Vector3 shiftVector = realPose * scaleVar;

        if((realMagnitude + shiftVector.magnitude) -modelMagnitude > 0.001 && scaleVar > 0){
            Debug.LogWarning("Limb length did not match even after calibration for: " + goalLandmark);
        }

        landmarksCopy[goalLandmark] += shiftVector; //+ extraShift;
        return shiftVector;
    }

    //on the mirror, shoulder is not set automatically, instead it can be calculated
    public void setTargetBetweenlandmarks(Dictionary<Pose.Landmark, Vector3> landmarks, Pose.Landmark leftLandmark, Pose.Landmark rightLandmark, GameObject centerTarget, float offsetScale = 0.0f)
    {
        if (landmarks.ContainsKey(rightLandmark) && landmarks.ContainsKey(leftLandmark) && centerTarget != null)
        {
            Vector3 leftPivot = landmarks[leftLandmark];
            Vector3 rightPivot = landmarks[rightLandmark];
            //Calculate the center position
            Vector3 centerPosition = (leftPivot + rightPivot) / 2;
            centerPosition -= Vector3.up * offsetScale; // Slightly lower it to match with Mixamo rig

            //Calculate what the rotation between the two shoulders might be
            Vector3 directionVector = rightPivot - leftPivot;
            Quaternion rotation = Quaternion.LookRotation(directionVector);

            //Move the target
            centerTarget.transform.position = centerPosition + gameObject.transform.position; //change if not relative
            centerTarget.transform.rotation = gameObject.transform.rotation * rotation;
        }
    }

    void setRotationFromTriangle(Dictionary<Pose.Landmark, Vector3> landmarks, Pose.Landmark leftLandmark, Pose.Landmark rightLandmark, Pose.Landmark baseLandmark, GameObject centerTarget, Quaternion rotationOffset)
    {
        if (landmarks.ContainsKey(rightLandmark) && landmarks.ContainsKey(leftLandmark) && landmarks.ContainsKey(baseLandmark) && centerTarget != null)
        {
            Vector3 rightDirection = (landmarks[rightLandmark] - landmarks[leftLandmark]).normalized;
            Vector3 forwardDirection = Vector3.Cross(rightDirection, (landmarks[baseLandmark] - landmarks[leftLandmark]).normalized).normalized;
            Vector3 upDirection = Vector3.Cross(forwardDirection, rightDirection).normalized;

            Quaternion orientation = Quaternion.LookRotation(forwardDirection, upDirection);

            centerTarget.transform.rotation = orientation * rotationOffset;
        }
    }

    public void SetPointPosition(Vector3 pointAt)
    {
        if (pointingConstraint != null)
        {
            pointingConstraint.weight = 1.0f;
            LeftHandTarget.transform.position = pointAt;
        }
    }

    public void StopPointing()
    {
        if (pointingConstraint != null)
        {
            pointingConstraint.weight = 0.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget); //TODO: Add proper left hand functionality to the rig. Currently it keeps trying to override the shoulders completely instead of adjusting them
        landmarkToTarget.Add(Pose.Landmark.RIGHT_WRIST, RightHandTarget);
        landmarkToTarget.Add(Pose.Landmark.LEFT_FOOT, LeftFootTarget);
        landmarkToTarget.Add(Pose.Landmark.RIGHT_FOOT, RightFootTarget);
        if (mirroring) //some landmarks are only used by the mirror character for now. Later the A.I might need more to copy gestures.
        {
            landmarkToTarget.Add(Pose.Landmark.LEFT_HIP, LeftHipTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_HIP, RightHipTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_ELBOW, RightElbowHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_ELBOW, LeftElbowHintTarget);


            landmarkToTarget.Add(Pose.Landmark.RIGHT_SHOULDER, RightShoulderTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_SHOULDER, LeftShoulderTarget);


            landmarkToTarget.Add(Pose.Landmark.RIGHT_INDEX, RightIndexTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_THUMB, RightThumbTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_RING, RightRingTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_PINKY, RightPinkyTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_MIDDLE, RightMiddleTarget);

            landmarkToTarget.Add(Pose.Landmark.RIGHT_INDEX_KNUCKLE, RightIndexHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_THUMB_KNUCKLE, RightThumbHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_RING_KNUCKLE, RightRingHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_PINKY_KNUCKLE, RightPinkyHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_MIDDLE_KNUCKLE, RightMiddleHintTarget);
            /*
            landmarkToTarget.Add(Pose.Landmark.RIGHT_INDEX_BASE, RightIndexHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_THUMB_BASE, RightThumbHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_RING_BASE, RightRingHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_PINKY_BASE, RightPinkyHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_MIDDLE_BASE, RightMiddleHintTarget);*/

            landmarkToTarget.Add(Pose.Landmark.LEFT_INDEX, LeftIndexTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_THUMB, LeftThumbTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_RING, LeftRingTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_PINKY, LeftPinkyTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_MIDDLE, LeftMiddleTarget);

            landmarkToTarget.Add(Pose.Landmark.LEFT_INDEX_KNUCKLE, LeftIndexHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_THUMB_KNUCKLE, LeftThumbHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_RING_KNUCKLE, LeftRingHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_PINKY_KNUCKLE, LeftPinkyHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_MIDDLE_KNUCKLE, LeftMiddleHintTarget);
            /*
            landmarkToTarget.Add(Pose.Landmark.LEFT_INDEX_BASE, LeftIndexHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_THUMB_BASE, LeftThumbHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_RING_BASE, LeftRingHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_PINKY_BASE, LeftPinkyHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_MIDDLE_BASE, LeftMiddleHintTarget);*/

            landmarkToTarget.Add(Pose.Landmark.LEFT_KNEE, LeftKneeHintTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_KNEE, RightKneeHintTarget);


            landmarkToTarget.Add(Pose.Landmark.LEFT_EAR, LeftEarTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_EAR, RightEarTarget);
            landmarkToTarget.Add(Pose.Landmark.NOSE, NoseTarget);
        }


        if (mirroring && useCalibration)
        {
            saveCurrentPositions(landmarkToModelBasePosition);
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        if (mirroring && CameraStream.playerPose._landmarkArrangement.Count > 0)
        {
            SetIKPositions(CameraStream.playerPose, !useWorldCoordinates);
            if (!lockedInCalibration && useCalibration)
            {
                lockedInCalibration = true;
                StartCoroutine(calibrationTimer());


            }
        }
    }
    IEnumerator calibrationTimer()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Using calibration!");
        //ELONGATE ARMS
        if (mirroring && lockedInCalibration && rigBuilder)
        {
            if (reshapeModelForCalibration)
            {
                /*
                saveCurrentPositions(targetToPlayerBasePosition);
                ElongateModelLimb(RightElbowHintTarget, RightShoulderTarget, Pose.Landmark.RIGHT_ELBOW, Pose.Landmark.RIGHT_SHOULDER, RightUpperArmBone);
                ElongateModelLimb(RightHandTarget, RightElbowHintTarget, Pose.Landmark.RIGHT_WRIST, Pose.Landmark.RIGHT_ELBOW, RightLowerArmBone);

                ElongateModelLimb(LeftElbowHintTarget, LeftShoulderTarget, Pose.Landmark.LEFT_ELBOW, Pose.Landmark.LEFT_SHOULDER, LeftUpperArmBone);
                ElongateModelLimb(LeftHandTarget, LeftElbowHintTarget, Pose.Landmark.LEFT_WRIST, Pose.Landmark.LEFT_ELBOW, LeftLowerArmBone);
                rigBuilder.Build(); //I should not have to do this, but it will not work otherwise
                Debug.Log("Reshaped limbs!");
                */
            }
        }

    }

    Pose makeIntermediateArrangement(Pose posePrev, Pose poseNext, float interpParam)
    {
        Dictionary<Pose.Landmark, Vector3> dictPrev = posePrev._landmarkArrangement;
        Dictionary<Pose.Landmark, Vector3> dictNext = poseNext._landmarkArrangement;
        Dictionary<Pose.Landmark, Vector3> dictIntermediate = new Dictionary<Pose.Landmark, Vector3>();

        foreach (var kvp in dictPrev)
        {
            Pose.Landmark landmark = kvp.Key;
            Vector3 position = kvp.Value;

            //Check if we have a target to adjust for this landmark
            if (dictNext.ContainsKey(landmark) && dictNext[landmark] != null)
            {
                dictIntermediate.Add(landmark, Vector3.Lerp(dictPrev[landmark], dictNext[landmark], interpParam));
            }
        }

        Pose intermediatePose = new Pose(dictIntermediate);
        return intermediatePose;
    }

    Vector3 initialLeftPosition;
    //public Dictionary<Pose.Landmark, Vector3> targetToInitialPosition = new Dictionary<GameObject, Vector3>();
    public void saveCurrentPositions(Dictionary<Pose.Landmark, Vector3> dictionaryToFill)
    {
        dictionaryToFill.Clear();
        dictionaryToFill.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_ELBOW, LeftElbowHintTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.RIGHT_WRIST, RightHandTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_ELBOW, RightElbowHintTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.RIGHT_SHOULDER, RightShoulderTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_SHOULDER, LeftShoulderTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.RIGHT_KNEE, RightKneeHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_KNEE, LeftKneeHintTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.LEFT_HIP, LeftHipTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_HIP, RightHipTarget.transform.position);


        dictionaryToFill.Add(Pose.Landmark.LEFT_FOOT, LeftFootTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_FOOT, RightFootTarget.transform.position);


        dictionaryToFill.Add(Pose.Landmark.LEFT_INDEX, LeftIndexTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_MIDDLE, LeftMiddleTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_RING, LeftRingTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_PINKY, LeftPinkyTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_THUMB, LeftThumbTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.LEFT_INDEX_KNUCKLE, LeftIndexHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_MIDDLE_KNUCKLE, LeftMiddleHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_RING_KNUCKLE, LeftRingHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_PINKY_KNUCKLE, LeftPinkyHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_THUMB_KNUCKLE, LeftThumbHintTarget.transform.position);



        dictionaryToFill.Add(Pose.Landmark.RIGHT_INDEX, RightIndexTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_MIDDLE, RightMiddleTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_RING, RightRingTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_PINKY, RightPinkyTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_THUMB, RightThumbTarget.transform.position);

        dictionaryToFill.Add(Pose.Landmark.RIGHT_INDEX_KNUCKLE, RightIndexHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_MIDDLE_KNUCKLE, RightMiddleHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_RING_KNUCKLE, RightRingHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_PINKY_KNUCKLE, RightPinkyHintTarget.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_THUMB_KNUCKLE, RightThumbHintTarget.transform.position);

        if(calibrateFingers)
        {

        dictionaryToFill.Add(Pose.Landmark.LEFT_INDEX_BASE, LeftIndexBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_MIDDLE_BASE, LeftMiddleBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_RING_BASE, LeftRingBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_PINKY_BASE, LeftPinkyBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.LEFT_THUMB_BASE, LeftThumbBase.transform.position);

        dictionaryToFill.Add(Pose.Landmark.RIGHT_INDEX_BASE, RightIndexBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_MIDDLE_BASE, RightMiddleBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_RING_BASE, RightRingBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_PINKY_BASE, RightPinkyBase.transform.position);
        dictionaryToFill.Add(Pose.Landmark.RIGHT_THUMB_BASE, RightThumbBase.transform.position);
        }
    }


    //you can quickly test the "playGesture" function as such:
    //Gesture exampleGesture = new GestureHandRises(LevelManager.dictionary.GetKnownPoses());
    //StartCoroutine(playGesture(exampleGesture));
    public IEnumerator playGesture(Gesture gestureToPlay)
    {
        for (int i = 0; i < gestureToPlay._poseSequence.Count; i++)
        {
            SetIKPositions(gestureToPlay._poseSequence[i]._poseToMatch, true);
            if (gesturePlaySmoothing)
            {
                if (i < gestureToPlay._poseSequence.Count - 1)
                {
                    float interpLength = gestureToPlay._poseSequence[i]._frameInterval * 60;
                    for (int j = 0; j < interpLength; j++)//for how far along we are between poses
                    {
                        Pose intermediatePose = makeIntermediateArrangement(gestureToPlay._poseSequence[i]._poseToMatch, gestureToPlay._poseSequence[i + 1]._poseToMatch, (float)j / interpLength);
                        SetIKPositions(intermediatePose, true);

                        yield return new WaitForSeconds((float)1 / 60);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(gestureToPlay._poseSequence[i]._frameInterval);
            }
        }
    }
    private void adjustHands()
    {
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand.transform.rotation = rightHand.transform.parent.rotation;
        leftHand.transform.rotation = leftHand.transform.parent.rotation;

    }
}
