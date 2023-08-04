using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary.ContentGenerators.StarterPoses;

namespace GestureDictionary.ContentGenerators {

    public enum PoseID {
        HAND_LEFT_UP,
        HAND_LEFT_MIDDLE,
        HAND_LEFT_DOWN
    }
    
    public static class PoseGenerator
    {
        /// <summary> Generates basic known poses </summary>
        public static void GenerateStarterPoses(DictionaryManager gestureDictionary) {
            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();

            AddStarterPose(new PoseLeftHandUp(), poseRegistry);
            AddStarterPose(new PoseLeftHandDown(), poseRegistry);
            AddStarterPose(new PoseLeftHandMiddle(), poseRegistry);
        }

        public static void AddStarterPose(StarterPose starterPose, Dictionary<string, Pose> poseRegistry)
        {
            Debug.Log("Added StarterPose: " + starterPose.poseId);
            poseRegistry.Add(starterPose.poseId, starterPose);
        }
    }
}
