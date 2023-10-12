using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject RightHandTarget;
    public GameObject LeftHandTarget;
    public GameObject RightFootTarget;
    public GameObject LeftFootTarget;

    public GameObject RightElbowHintTarget;
    public GameObject LeftElbowHintTarget;

    public ChainIKConstraint pointingConstraint;

    public bool mirroring = false;

    //Changes every IK target to match up with the given pose
    public void SetIKPositions(Pose playingPose, bool relative = false)
    {
        Dictionary<Pose.Landmark, Vector3> landmarksCopy = new Dictionary<Pose.Landmark, Vector3>(playingPose._landmarkArrangement); //Dictoinary must to be copied before we do the iteration, or we get errors for having it changed by the animation thread in the middle of it.

        foreach (var landmark in landmarksCopy.Keys.ToList()) //TODO: use the built-in Pose version of this instead for clarity, but it's a bit tricky since we are copying it over
        {
            Vector3 originalPosition = landmarksCopy[landmark];
            Vector3 rotatedPosition = gameObject.transform.rotation * originalPosition;
            landmarksCopy[landmark] = rotatedPosition;
        }


        foreach (var kvp in landmarksCopy)
        {
            Pose.Landmark landmark = kvp.Key;
            Vector3 position = kvp.Value;

            //Check if we have a target to adjust for this landmark
            if (landmarkToTarget.ContainsKey(landmark))
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
        if (mirroring)
        {
            landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget); //only add it to the mirror for now
            landmarkToTarget.Add(Pose.Landmark.RIGHT_ELBOW, RightElbowHintTarget); //only add it to the mirror for now
            landmarkToTarget.Add(Pose.Landmark.LEFT_ELBOW, LeftElbowHintTarget); //only add it to the mirror for now
        }

    }

    // Have the A.I correctly take a Twister turn. In normal gameplay, this would be cheating.
    void SolveTwister()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mirroring && CameraStream.vector3List.Count > 0)
        {
            SetIKPositions(CameraStream.playerPose, true);
        }
    }
}
