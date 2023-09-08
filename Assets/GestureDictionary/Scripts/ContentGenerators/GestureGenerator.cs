using GestureDictionary.ContentGenerators.StarterGestures;
using System.Collections.Generic;
using UnityEngine;
using AuxiliarContent;

namespace GestureDictionary.ContentGenerators {

    public enum GestureID
    {
        TEST_A,
        TEST_B,
        HAND_RISES,
        HAND_FALLS,
        POSITIVE,
        NEGATIVE
    }

    public static class GestureGenerator
    {

        public static void GenerateStarterGestures(DictionaryManager dictionaryManager) {
            GenerateMetaGestures(dictionaryManager);
            GenerateBasicGestures(dictionaryManager);
        }

        /// <summary> Generates known metagestures </summary>
        public static void GenerateMetaGestures(
            DictionaryManager dictionaryManager) {

            Dictionary<string, Pose> poseRegistry = dictionaryManager.GetKnownPoses();

            GenerateTestStarterGestures(poseRegistry, dictionaryManager, true);
            //            GenerateTrueStarterGestures(poseRegistry, dictionaryManager, true);

            CustomDebug.LogGen("Added metagestures.");
        }

        /// <summary> Generates basic known gestures </summary>
        public static void GenerateBasicGestures(DictionaryManager dictionaryManager) {
            Dictionary<string, Pose> poseRegistry = dictionaryManager.GetKnownPoses();

            GenerateTestStarterGestures(poseRegistry, dictionaryManager, false);
            //    GenerateTrueStarterGestures(poseRegistry, dictionaryManager, false);

            CustomDebug.LogGen("Added basic gestures.");
        }

        public static void AddStarterGesture
            (StarterGesture starterGesture, 
            DictionaryManager dictionaryManager, bool isMetaGesture = false)
        {
            CustomDebug.LogGen("Added starter gesture: " + starterGesture.gestureId);
            dictionaryManager.AddGesture(starterGesture, starterGesture.gestureId, isMetaGesture);
        }

        public static void GenerateTrueStarterGestures(Dictionary<string, Pose> poseRegistry,
            DictionaryManager dictionaryManager, bool isMetaGesture = false) {

                if (isMetaGesture) {
                    AddStarterGesture(new GestureNegative(poseRegistry), dictionaryManager, true);
                    AddStarterGesture(new GesturePositive(poseRegistry), dictionaryManager, true);
                    
                }
                else {
                    AddStarterGesture(new GestureHandRises(poseRegistry), dictionaryManager);
                    AddStarterGesture(new GestureHandFalls(poseRegistry), dictionaryManager);
                }

            CustomDebug.LogGen("Added TRUE gestures.");
        }

        public static void GenerateTestStarterGestures(Dictionary<string, Pose> poseRegistry, 
            DictionaryManager dictionaryManager, bool isMetaGesture=false) {

                if (isMetaGesture) {
                    // TODO
                }
                else {
                    AddStarterGesture(new GestureTestA(poseRegistry), dictionaryManager);
                    AddStarterGesture(new GestureTestB(poseRegistry), dictionaryManager);
                }

            CustomDebug.LogGen("Added TEST gestures.");
        }
    }

}
