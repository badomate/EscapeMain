using System.Collections;
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
            GestureDictionary gestureDictionary) {

            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();
            Dictionary<string, Gesture> meaningToGesture = gestureDictionary.GetMeaningRegistry();
            Dictionary<Gesture, string> metaGestureToMeaning = gestureDictionary.GetMetaGestureRegistry();

            Debug.Log("Added metagestures.");
        }

        /// <summary> Generates basic known gestures </summary>
        public static void GenerateStarterGestures(GestureDictionary gestureDictionary) {
            Dictionary<string, Pose> poseRegistry = gestureDictionary.GetKnownPoses();


            Gesture gestureHandRises = new Gesture();
            List<Pose> gestureHandRisesPoses = new List<Pose>() {
                poseRegistry[PoseID.HAND_LEFT_UP.ToString()],
                poseRegistry[PoseID.HAND_LEFT_MIDDLE.ToString()],
                poseRegistry[PoseID.HAND_LEFT_DOWN.ToString()]
            };
            gestureHandRises.AddPoses(gestureHandRisesPoses);

            Gesture gestureHandFalls = new Gesture();
            List<Pose> gestureHandFallsPoses = new List<Pose>() {
                poseRegistry[PoseID.HAND_LEFT_DOWN.ToString()],
                poseRegistry[PoseID.HAND_LEFT_MIDDLE.ToString()],
                poseRegistry[PoseID.HAND_LEFT_UP.ToString()]
            };
            gestureHandRises.AddPoses(gestureHandFallsPoses);

            gestureDictionary.AddGesture(gestureHandRises, GestureID.HAND_RISES.ToString());
            gestureDictionary.AddGesture(gestureHandFalls, GestureID.HAND_FALLS.ToString());
        }
    }

}
