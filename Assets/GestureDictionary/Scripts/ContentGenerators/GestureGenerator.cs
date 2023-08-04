using GestureDictionary.ContentGenerators.StarterGestures;
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators {

    public enum GestureID
    {
        HAND_RISES,
        HAND_FALLS
    }

    public static class GestureGenerator
    {
        /// <summary> Generates known metagestures </summary>
        public static void GenerateMetaGestures(
            DictionaryManager dictionaryManager) {

            Dictionary<string, Pose> poseRegistry = dictionaryManager.GetKnownPoses();
            Dictionary<string, Gesture> meaningToGesture = dictionaryManager.GetMeaningRegistry();
            Dictionary<Gesture, string> metaGestureToMeaning = dictionaryManager.GetMetaGestureRegistry();

            Debug.Log("Added metagestures.");
        }

        /// <summary> Generates basic known gestures </summary>
        public static void GenerateBasicGestures(DictionaryManager dictionaryManager) {
            Dictionary<string, Pose> poseRegistry = dictionaryManager.GetKnownPoses();

            AddStarterGesture(new GestureHandRises(poseRegistry), dictionaryManager);
            AddStarterGesture(new GestureHandFalls(poseRegistry), dictionaryManager);
            
            Debug.Log("Added basic gestures.");
        }

        public static void AddStarterGesture
            (StarterGesture starterGesture, 
            DictionaryManager dictionaryManager, bool isMetaGesture = false)
        {
            Debug.Log("Added starter gesture: " + starterGesture.gestureId);
            dictionaryManager.AddGesture(starterGesture, starterGesture.gestureId, isMetaGesture);
        }
    }

}
