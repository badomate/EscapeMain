using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators {

    public enum PoseID {
        HAND_LEFT_UP,
        HAND_LEFT_MIDDLE,
        HAND_LEFT_DOWN
    }
    
    public static class PoseGenerator
    {
        /// <summary> Generates basic known poses </summary>
        public static void GenerateStarterPoses(GestureDictionary gestureDictionary) {
            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();

            Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandUp = 
                new() {
                    {Pose.Landmark.LEFT_WRIST, new Vector3(2, 5, 4)}
                };
            poseRegistry.Add(PoseID.HAND_LEFT_UP.ToString(), new Pose(dictPoseLeftHandUp));

            Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandMiddle = 
                new() {
                    {Pose.Landmark.LEFT_WRIST, new Vector3(2, 3, 4)}
                };
            poseRegistry.Add(PoseID.HAND_LEFT_MIDDLE.ToString(), new Pose(dictPoseLeftHandMiddle));

            Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandDown = 
                new() {
                    {Pose.Landmark.LEFT_WRIST, new Vector3(2, 0, 4)}
                };
            poseRegistry.Add(PoseID.HAND_LEFT_DOWN.ToString(), new Pose(dictPoseLeftHandDown));
        }
    }
}
