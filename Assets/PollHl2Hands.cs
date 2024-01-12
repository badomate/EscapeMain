//using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using MixedReality.Toolkit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System;
//using Microsoft.MixedReality.OpenXR;

public class PollHl2Hands : MonoBehaviour
{
    private HandsAggregatorSubsystem subsystem;

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
        }
    }

    void doLeftHand(IReadOnlyList<HandJointPose> leftHand)
    {
        Debug.Log(leftHand[0].ToString());
    }
    void doRightHand(IReadOnlyList<HandJointPose> rightHand)
    {
        Debug.Log(rightHand[0].ToString());
    }


    /*
    private void OnDisable()
    {
        // This component is being destroyed
        // Instruct the Input System to disregard us for input event handling
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    // IMixedRealitySourceStateHandler interface
    public void OnSourceDetected(SourceStateEventData eventData)
    {
        var hand = eventData.Controller as IMixedRealityHand;

        // Only react to articulated hand input sources
        if (hand != null)
        {
            Debug.Log("Source detected: " + hand.ControllerHandedness);
        }
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        var hand = eventData.Controller as IMixedRealityHand;

        // Only react to articulated hand input sources
        if (hand != null)
        {
            Debug.Log("Source lost: " + hand.ControllerHandedness);
        }
    }

    Dictionary<Pose.Landmark, Vector3> poseDictionary;

    public void OnHandJointsUpdated(
                InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        MixedRealityPose handJoint;
        if (eventData.InputData.TryGetValue(TrackedHandJoint.IndexTip, out handJoint))
        {
            Debug.Log("Hand Joint Palm Updated: " + handJoint.Position);
            poseDictionary.Add(Pose.Landmark.LEFT_INDEX, handJoint.Position);
        }

        RecognizeGesture.playerMovementRecord[0] = poseDictionary;
    }*/
}