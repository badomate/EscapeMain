using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary.ContentGenerators.StarterPoses;

namespace GestureDictionary.ContentGenerators {

    public enum PoseID {
        TEST_A,
        TEST_B,
        HAND_LEFT_UP,
        HAND_LEFT_MIDDLE,
        HAND_LEFT_DOWN
    }
    
    public static class PoseGenerator
    {
        /// <summary> Generates basic known poses </summary>
        public static void GenerateStarterPoses(DictionaryManager gestureDictionary) {
            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();

            GenerateTestStarterPoses(poseRegistry);
//            GenerateTrueStarterPoses(poseRegistry);

            Debug.Log("Added all starter poses defined.");
        }

        public static void AddStarterPose(StarterPose starterPose, Dictionary<string, Pose> poseRegistry)
        {
            Debug.Log("Added StarterPose: " + starterPose.poseId);
            poseRegistry.Add(starterPose.poseId, starterPose);
        }

        public static void GenerateTrueStarterPoses(Dictionary<string, Pose> poseRegistry) {
            AddStarterPose(new PoseLeftHandUp(), poseRegistry);
            AddStarterPose(new PoseLeftHandDown(), poseRegistry);
            AddStarterPose(new PoseLeftHandMiddle(), poseRegistry);

            Debug.Log("Added TRUE starter poses");
        }

        public static void GenerateTestStarterPoses(Dictionary<string, Pose> poseRegistry) {
            AddStarterPose(new PoseTestA(), poseRegistry);
            AddStarterPose(new PoseTestB(), poseRegistry);

            Debug.Log("Added TEST starter poses");
        }
    }
}
