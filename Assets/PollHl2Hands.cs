using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;

public class PollHl2Hands : MonoBehaviour
{
    private HandsAggregatorSubsystem subsystem;

    Dictionary<Pose.Landmark, Vector3> poseDictionary;

    Dictionary<TrackedHandJoint, Pose.Landmark> jointToLandmarkMapping = new Dictionary<TrackedHandJoint, Pose.Landmark>()
{
    { TrackedHandJoint.IndexTip, Pose.Landmark.LEFT_INDEX },
    { TrackedHandJoint.IndexMetacarpal, Pose.Landmark.LEFT_INDEX_BASE },
    { TrackedHandJoint.IndexProximal, Pose.Landmark.LEFT_INDEX_KNUCKLE },

    { TrackedHandJoint.LittleTip, Pose.Landmark.LEFT_PINKY },
    { TrackedHandJoint.LittleMetacarpal, Pose.Landmark.LEFT_PINKY_BASE },
    { TrackedHandJoint.LittleProximal, Pose.Landmark.LEFT_PINKY_KNUCKLE },

    { TrackedHandJoint.ThumbTip, Pose.Landmark.LEFT_THUMB },
    { TrackedHandJoint.ThumbMetacarpal, Pose.Landmark.LEFT_THUMB_BASE },
    { TrackedHandJoint.ThumbProximal, Pose.Landmark.LEFT_THUMB_KNUCKLE },

    { TrackedHandJoint.RingTip, Pose.Landmark.LEFT_RING },
    { TrackedHandJoint.RingMetacarpal, Pose.Landmark.LEFT_RING_BASE },
    { TrackedHandJoint.RingProximal, Pose.Landmark.LEFT_RING_KNUCKLE },

    { TrackedHandJoint.MiddleTip, Pose.Landmark.LEFT_MIDDLE },
    { TrackedHandJoint.MiddleMetacarpal, Pose.Landmark.LEFT_MIDDLE_BASE },
    { TrackedHandJoint.MiddleProximal, Pose.Landmark.LEFT_MIDDLE_KNUCKLE },

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
            if (subsystem.TryGetEntireHand(XRNode.LeftHand, out IReadOnlyList<HandJointPose> leftHand))
            {
                doLeftHand(leftHand);
            }

            if (subsystem.TryGetEntireHand(XRNode.RightHand, out IReadOnlyList<HandJointPose> rightHand))
            {
                doRightHand(rightHand);
            }
            RecognizeGesture.playerMovementRecord[0] = poseDictionary;
        }
    }

    void doLeftHand(IReadOnlyList<HandJointPose> leftHand)
    {
        foreach (int i in Enum.GetValues(typeof(TrackedHandJoint)))
        {
            poseDictionary[jointToLandmarkMapping[(TrackedHandJoint)i]] = leftHand[i].Position;
        }
    }
    void doRightHand(IReadOnlyList<HandJointPose> rightHand)
    {
        foreach (int i in Enum.GetValues(typeof(TrackedHandJoint)))
        {
            poseDictionary[jointToLandmarkMapping[(TrackedHandJoint)i]] = rightHand[i].Position;
        }
    }

}