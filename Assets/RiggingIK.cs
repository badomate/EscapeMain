using System.Collections;
using System.Collections.Generic;
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

    public ChainIKConstraint pointingConstraint;

    public bool mirroring = false;

    //Changes every IK target to match up with the given pose
    public void SetIKPositions(Pose playingPose, bool relative = false)
    {
        foreach (var kvp in playingPose._landmarkArrangement)
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
        //landmarkToTarget.Add(Pose.Landmark.LEFT_WRIST, LeftHandTarget); //TODO: Add left hand functionality to the rig. Currently it keeps trying to override the shoulders completely instead of adjusting them
        landmarkToTarget.Add(Pose.Landmark.RIGHT_WRIST, RightHandTarget);
        landmarkToTarget.Add(Pose.Landmark.LEFT_FOOT, LeftFootTarget);
        landmarkToTarget.Add(Pose.Landmark.RIGHT_FOOT, RightFootTarget);

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
            SetIKPositions(Pose.GetPoseFromArray(CameraStream.vector3List.ToArray()), true);
        }
    }
}
