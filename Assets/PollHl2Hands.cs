using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Linq;

public class PollHl2Hands : MonoBehaviour
{
    private HandsAggregatorSubsystem subsystem;

    public static Dictionary<Pose.Landmark, Vector3> poseDictionary = new Dictionary<Pose.Landmark, Vector3>();

    Dictionary<TrackedHandJoint, Pose.Landmark> jointToLandmarkMapping = new Dictionary<TrackedHandJoint, Pose.Landmark>()
{
    { TrackedHandJoint.Palm, Pose.Landmark.LEFT_WRIST },//To-do: may want a seperate palm landmark instead? this could cause issues later
    { TrackedHandJoint.Wrist, Pose.Landmark.LEFT_WRIST_ROOT }, 

    { TrackedHandJoint.IndexTip, Pose.Landmark.LEFT_INDEX },
    { TrackedHandJoint.IndexProximal, Pose.Landmark.LEFT_INDEX_BASE },
    { TrackedHandJoint.IndexIntermediate, Pose.Landmark.LEFT_INDEX_KNUCKLE },

    { TrackedHandJoint.LittleTip, Pose.Landmark.LEFT_PINKY },
    { TrackedHandJoint.LittleProximal, Pose.Landmark.LEFT_PINKY_BASE },
    { TrackedHandJoint.LittleIntermediate, Pose.Landmark.LEFT_PINKY_KNUCKLE },

    { TrackedHandJoint.RingTip, Pose.Landmark.LEFT_RING },
    { TrackedHandJoint.RingProximal, Pose.Landmark.LEFT_RING_BASE },
    { TrackedHandJoint.RingIntermediate, Pose.Landmark.LEFT_RING_KNUCKLE },

    { TrackedHandJoint.MiddleTip, Pose.Landmark.LEFT_MIDDLE },
    { TrackedHandJoint.MiddleProximal, Pose.Landmark.LEFT_MIDDLE_BASE },
    { TrackedHandJoint.MiddleIntermediate, Pose.Landmark.LEFT_MIDDLE_KNUCKLE },

    { TrackedHandJoint.ThumbTip, Pose.Landmark.LEFT_THUMB },
    { TrackedHandJoint.ThumbProximal, Pose.Landmark.LEFT_THUMB_BASE },
    { TrackedHandJoint.ThumbDistal, Pose.Landmark.LEFT_THUMB_KNUCKLE },


};

    void Start()
    {
        new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>() != null);
        subsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
    }
    void Update()
    {
        if (subsystem != null)
        {
            poseDictionary.Clear();
            if (subsystem.TryGetEntireHand(XRNode.LeftHand, out IReadOnlyList<HandJointPose> leftHand))
            {
                processHand(leftHand, true);
            }

            if (subsystem.TryGetEntireHand(XRNode.RightHand, out IReadOnlyList<HandJointPose> rightHand))
            {
                processHand(rightHand, false);
            }

            RecognizeGesture.playerMovementRecord[0] = new Dictionary<Pose.Landmark, Vector3>(PollHl2Hands.poseDictionary);
        }
    }
    public static string ReplaceLeftWithRight(string enumValue)
    {
        return enumValue.Replace("LEFT", "RIGHT");
    }

    void processHand(IReadOnlyList<HandJointPose> hand, bool left)
    {
        if (hand != null)
        {
            foreach (TrackedHandJoint i in Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if((int)i < hand.Count && (int)i >= 0 && hand[(int)i] != null && jointToLandmarkMapping.ContainsKey(i))
                {
                    if (left)
                    {
                        poseDictionary.Add(jointToLandmarkMapping[i], hand[(int)i].Position);
                        //Debug.Log(hand[(int)i].Position);
                    }
                    else
                    {
                        Enum.TryParse(ReplaceLeftWithRight(jointToLandmarkMapping[i].ToString()), out Pose.Landmark landmarkToPut);
                        poseDictionary.Add(landmarkToPut, hand[(int)i].Position);
                        //Debug.Log(hand[(int)i].Position);
                    }
                }
            }
        }
    }

}