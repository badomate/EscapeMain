//using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.OpenXR;

public class PollHl2Hands : MonoBehaviour
    //IMixedRealitySourceStateHandler, // Handle source detected and lost
    //IMixedRealityHandJointHandler // handle joint position updates for hands
{
    /*private void OnEnable()
    {
        // Instruct Input System that we would like to receive all input events of type
        // IMixedRealitySourceStateHandler and IMixedRealityHandJointHandler
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

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