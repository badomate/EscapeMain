using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///This class should serve some of the same functions as EstimationToIk 
///but will perform those actions using the Animation Rigging system
///It does not inherit from it since EstimationToIk is heavily based on OnAnimatorIK which is unused here
/// </summary>
public class RiggingIK : MonoBehaviour
{
    Pose currentPose; 
    public static Dictionary<Pose.Landmark, GameObject> landmarkToTarget = new Dictionary<Pose.Landmark, GameObject>();
    public GameObject RightHandTarget;
    public GameObject LeftHandTarget;
    public GameObject RightFootTarget;
    public GameObject LeftFootTarget;

    private void SetIKPosition(Pose playingPose)
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
                landmarkTarget.transform.position = position;
            }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
