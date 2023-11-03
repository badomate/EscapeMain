using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary.ContentGenerators.StarterPoses;
using AuxiliarContent;

namespace GestureDictionary.ContentGenerators {

    public enum PoseID {
        TEST_A,
        TEST_B,
        HAND_LEFT_UP,
        HAND_LEFT_MIDDLE,
        HAND_LEFT_DOWN,
        COME_CLOSER1,
        COME_CLOSER2,
        GO_FURTHER1,
        GO_FURTHER2,
        GO_LEFT1,
        GO_LEFT2,
        GO_RIGHT1,
        GO_RIGHT2,
        DONT_UNDERSTAND1,
        DONT_UNDERSTAND2
    }
    
    public static class PoseGenerator
    {
        /// <summary> Generates basic known poses </summary>
        public static void GenerateStarterPoses(DictionaryManager gestureDictionary) {
            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();

            GenerateTestStarterPoses(poseRegistry);
            //            GenerateTrueStarterPoses(poseRegistry);

            CustomDebug.LogGen("Added all starter poses defined.");
        }

        public static void AddStarterPose(StarterPose starterPose, Dictionary<string, Pose> poseRegistry)
        {
            CustomDebug.LogGen("Added StarterPose: " + starterPose.poseId + starterPose.ToString(false));
            poseRegistry.Add(starterPose.poseId, starterPose);
        }

        public static void GenerateTrueStarterPoses(Dictionary<string, Pose> poseRegistry) {
            AddStarterPose(new PoseLeftHandUp(), poseRegistry);
            AddStarterPose(new PoseLeftHandDown(), poseRegistry);
            AddStarterPose(new PoseLeftHandMiddle(), poseRegistry);

            CustomDebug.LogGen("Added TRUE starter poses");
        }

        public static void GenerateTestStarterPoses(Dictionary<string, Pose> poseRegistry) {
            AddStarterPose(new PoseTestA(), poseRegistry);
            AddStarterPose(new PoseTestB(), poseRegistry);
            AddStarterPose(new PoseLeftHandUp(), poseRegistry);
            AddStarterPose(new PoseLeftHandDown(), poseRegistry);
            AddStarterPose(new PoseLeftHandMiddle(), poseRegistry);
            AddStarterPose(new PoseComeCloser1(), poseRegistry);
            AddStarterPose(new PoseComeCloser2(), poseRegistry);
            AddStarterPose(new PoseGoFurther1(), poseRegistry);
            AddStarterPose(new PoseGoFurther2(), poseRegistry);
            AddStarterPose(new PoseGoLeft1(), poseRegistry);
            AddStarterPose(new PoseGoLeft2(), poseRegistry);
            AddStarterPose(new PoseGoRight1(), poseRegistry);
            AddStarterPose(new PoseGoRight2(), poseRegistry);
            AddStarterPose(new PoseDontUnderstand1(), poseRegistry);
            AddStarterPose(new PoseDontUnderstand2(), poseRegistry);

            CustomDebug.LogGen("Added TEST starter poses");
        }
    }
}
