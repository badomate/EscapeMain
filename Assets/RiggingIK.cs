using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using GestureDictionary.ContentGenerators.StarterGestures;

/// <summary>
///This class should serve some of the same functions as EstimationToIk 
///but will perform those actions using the Animation Rigging system
///It does not inherit from it since EstimationToIk is heavily based on OnAnimatorIK which is unused here
/// </summary>
public class RiggingIK : MonoBehaviour
{
    Pose currentPose;
    public Dictionary<Pose.Landmark, GameObject> landmarkToTarget = new Dictionary<Pose.Landmark, GameObject>();

    //main landmarks
    public GameObject RightHandTarget;
    public GameObject LeftHandTarget;
    public GameObject RightFootTarget;
    public GameObject LeftFootTarget;

    //hint landmarks
    public GameObject RightElbowHintTarget;
    public GameObject LeftElbowHintTarget;

    //hand landmarks
    public GameObject RightIndexTarget;
    public GameObject RightMiddleTarget;
    public GameObject RightRingTarget;
    public GameObject RightPinkyTarget;
    public GameObject RightThumbTarget;

    public GameObject LeftIndexTarget;
    public GameObject LeftMiddleTarget;
    public GameObject LeftRingTarget;
    public GameObject LeftPinkyTarget;
    public GameObject LeftThumbTarget;

    public GameObject ShoulderTarget;

    public ChainIKConstraint pointingConstraint;

    public bool mirroring = false;
    public bool gesturePlaySmoothing = true;
    public float shoulderOffsetScale = 0.1f;
    public Vector3 coordinateScale = new Vector3(1, 1, 1); //every landmark vector is multiplied by this

    List<Pose.Landmark> rightFingers = new List<Pose.Landmark> {
        Pose.Landmark.RIGHT_INDEX,
        Pose.Landmark.RIGHT_THUMB,
        Pose.Landmark.RIGHT_RING,
        Pose.Landmark.RIGHT_PINKY,
        Pose.Landmark.RIGHT_MIDDLE};

    List<Pose.Landmark> leftFingers = new List<Pose.Landmark> {
        Pose.Landmark.LEFT_INDEX,
        Pose.Landmark.LEFT_THUMB,
        Pose.Landmark.LEFT_RING,
        Pose.Landmark.LEFT_PINKY,
        Pose.Landmark.LEFT_MIDDLE };

    //this is just to simplify
    public void SetIKPositions(Pose poseToPlay, bool relative = false)
    {
        SetIKPositions(poseToPlay._landmarkArrangement, relative);
    }

    //Changes every IK target to match up with the given pose
    public void SetIKPositions(Dictionary<Pose.Landmark, Vector3> landmarkArrangement, bool relative = false)
    {
        Dictionary<Pose.Landmark, Vector3> landmarksCopy = new Dictionary<Pose.Landmark, Vector3>(landmarkArrangement); //Dictoinary must to be copied before we do the iteration, or we get errors for having it changed by the animation thread in the middle of it.


        foreach (var landmark in landmarksCopy.Keys.ToList()) //adjust hand origin
        {
            Debug.Log(landmarksCopy[Pose.Landmark.LEFT_INDEX]);
            Debug.Log(landmarksCopy[Pose.Landmark.RIGHT_INDEX]);
            if (leftFingers.Contains(landmark))
            {
                landmarksCopy[landmark] -= landmarksCopy[Pose.Landmark.LEFT_WRIST_ROOT];
                landmarksCopy[landmark] *= 0.75f;
                landmarksCopy[landmark] += landmarksCopy[Pose.Landmark.LEFT_WRIST];
            }

            if (rightFingers.Contains(landmark))
            {
                landmarksCopy[landmark] -= landmarksCopy[Pose.Landmark.RIGHT_WRIST_ROOT];
                landmarksCopy[landmark] *= 0.75f;
                landmarksCopy[landmark] += landmarksCopy[Pose.Landmark.RIGHT_WRIST];
            }

        }

        foreach (var landmark in landmarksCopy.Keys.ToList()) //TODO: use the built-in Pose version of this instead for clarity, but it's a bit tricky since we are copying it over
        {
            Vector3 originalPosition = Vector3.Scale(landmarksCopy[landmark], coordinateScale); //always scale first otherwise the positional relativity would break

            Vector3 rotatedPosition; 
            rotatedPosition = originalPosition;
            if (relative)
            {
                rotatedPosition = gameObject.transform.rotation * originalPosition;
            }

            landmarksCopy[landmark] = rotatedPosition;
        }



            foreach (var kvp in landmarksCopy)
        {
            Pose.Landmark landmark = kvp.Key;
            Vector3 position = kvp.Value;

            //Check if we have a target to adjust for this landmark
            if (landmarkToTarget.ContainsKey(landmark) && landmarkToTarget[landmark] != null)
            {
                GameObject landmarkTarget = landmarkToTarget[landmark];

                //Move the target gameobject to the position our Pose specified
                if (relative)
                {
                    landmarkTarget.transform.position = position + gameObject.transform.position;
                }
                else
                {
                    landmarkTarget.transform.position = position;
                }
            }
        }
        shoulderSetMirror(landmarksCopy);
    }

    //on the mirror, shoulder is not set automatically, instead it can be calculated
    public void shoulderSetMirror(Dictionary<Pose.Landmark, Vector3> landmarks)
    {
        if (landmarks.ContainsKey(Pose.Landmark.RIGHT_SHOULDER) && landmarks.ContainsKey(Pose.Landmark.LEFT_SHOULDER) && ShoulderTarget != null)
        {
            Vector3 leftShoulder = Vector3.Scale(landmarks[Pose.Landmark.LEFT_SHOULDER], coordinateScale);
            Vector3 rightShoulder = Vector3.Scale(landmarks[Pose.Landmark.RIGHT_SHOULDER], coordinateScale);
            //Calculate the center position
            Vector3 centerPosition = (leftShoulder + rightShoulder) / 2;
            centerPosition -= Vector3.up * shoulderOffsetScale; // Slightly lower it to match with Mixamo rig

            //Calculate what the rotation between the two shoulders might be
            Vector3 directionVector = rightShoulder - leftShoulder;
            Quaternion rotation = Quaternion.LookRotation(directionVector);

            //Move the target
            ShoulderTarget.transform.position = centerPosition + gameObject.transform.position; //change if not relative
            ShoulderTarget.transform.rotation = gameObject.transform.rotation * rotation;
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
        //landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget); //TODO: Add proper left hand functionality to the rig. Currently it keeps trying to override the shoulders completely instead of adjusting them
        landmarkToTarget.Add(Pose.Landmark.RIGHT_WRIST, RightHandTarget);
        landmarkToTarget.Add(Pose.Landmark.LEFT_FOOT, LeftFootTarget);
        landmarkToTarget.Add(Pose.Landmark.RIGHT_FOOT, RightFootTarget);
        if (mirroring) //some landmarks are only used by the mirror character for now. Later the A.I might need more to copy gestures.
        {
            landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget); 
            landmarkToTarget.Add(Pose.Landmark.RIGHT_ELBOW, RightElbowHintTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_ELBOW, LeftElbowHintTarget);


            landmarkToTarget.Add(Pose.Landmark.RIGHT_INDEX, RightIndexTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_THUMB, RightThumbTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_RING, RightRingTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_PINKY, RightPinkyTarget);
            landmarkToTarget.Add(Pose.Landmark.RIGHT_MIDDLE, RightMiddleTarget);

            landmarkToTarget.Add(Pose.Landmark.LEFT_INDEX, LeftIndexTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_THUMB, LeftThumbTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_RING, LeftRingTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_PINKY, LeftPinkyTarget);
            landmarkToTarget.Add(Pose.Landmark.LEFT_MIDDLE, LeftMiddleTarget);
        }

        //for testing the "play gesture" function
        //Gesture exampleGesture = new GestureHandRises(LevelManager.dictionary.GetKnownPoses());
        //StartCoroutine(playGesture(exampleGesture));
    }

    // Have the A.I correctly take a Twister turn. In normal gameplay, this would be cheating.
    void SolveTwister()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mirroring && CameraStream.playerPose._landmarkArrangement.Count > 0)
        {
            SetIKPositions(CameraStream.playerPose, true);
        }
    }

    void LateUpdate()
    {
        /*
        if (!mirroring) //mirror has hand landmarks so he doesnt need adjustment
        {
            adjustHands();
        }*/
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
        Animator animator = GetComponent<Animator>();
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand.transform.rotation = rightHand.transform.parent.rotation;
        leftHand.transform.rotation = leftHand.transform.parent.rotation;

    }
}
