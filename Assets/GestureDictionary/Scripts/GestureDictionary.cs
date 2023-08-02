using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary.ContentGenerators;

namespace GestureDictionary {
    public class GestureDictionary : MonoBehaviour
    {
        private Dictionary<Gesture, string> gestureToMeaning;
        private Dictionary<Gesture, string> metaGestureToMeaning;
        private Dictionary<string, Gesture> meaningToGesture;

        private Dictionary<string, Pose> knownPoses;

        public GestureDictionary() {
            gestureToMeaning = new Dictionary<Gesture, string>();
            meaningToGesture = new Dictionary<string, Gesture>();
            knownPoses = new Dictionary<string, Pose>();
            PoseGenerator.GenerateStarterPoses(this);
            GestureGenerator.GenerateMetaGestures(this);
            GestureGenerator.GenerateStarterGestures(this);
        }

        public Dictionary<string, Pose> GetKnownPoses()
        {
            return knownPoses;
        }

        public Dictionary<Gesture, string> GetGestureRegistry()
        {
            return gestureToMeaning;
        }

        public Dictionary<Gesture, string> GetMetaGestureRegistry()
        {
            return metaGestureToMeaning;
        }

        public Dictionary<string, Gesture> GetMeaningRegistry()
        {
            return meaningToGesture;
        }

        /// <summary> Adds a gesture to registries of known gestures </summary>
        public void AddGesture(Gesture gesture, string meaning, bool isMeta = false) {
            if (isMeta) {
                metaGestureToMeaning.Add(gesture, meaning);
            }
            else
            {
                gestureToMeaning.Add(gesture, meaning);
            }
            
            meaningToGesture.Add(meaning, gesture);
        }

        /// <summary> Updates a gesture's meaning </summary>
        public void UpdateGestureMeaning(Gesture gesture, string meaning) {
            if (!gestureToMeaning.ContainsKey(gesture) || !meaningToGesture.ContainsKey(meaning)) {
                AddGesture(gesture, meaning);
            }
            else {
                gestureToMeaning[gesture] = meaning;
                meaningToGesture[meaning] = gesture;
            }
        }

        /// <summary> Returns the gesture with the requested meaning (if known). </summary>
        public Gesture GetGestureFromMeaning(string meaning) {
            if (meaningToGesture.ContainsKey(meaning)) {
                return meaningToGesture[meaning];
            }
            return null;
        }

        /// <summary> Returns the most likely meaning for a given gesture, out of the ones known </summary>
        public string GetMeaningFromGesture(Gesture gesture, bool isBasic=false) {
            if (isBasic) return GetMeaningFromGestureBasic(gesture);
            else return GetMeaningFromGestureComplex(gesture);
        }

        /// <summary> Returns the meaning of the exact match to the given gesture (if recognized) </summary>
        public string GetMeaningFromGestureBasic(Gesture gesture) {
            if (metaGestureToMeaning.ContainsKey(gesture)) return metaGestureToMeaning[gesture];
            else if (gestureToMeaning.ContainsKey(gesture)) return gestureToMeaning[gesture];
            else return "";
        }

        /// <summary> 
        /// Looks through all known gestures to find the closest match to the one supplied,
        /// then returns its meaning.
        /// </summary> 
        public string GetMeaningFromGestureComplex(Gesture gesture) {
            float bestMatchVariance= Mathf.Infinity;
            string bestMatch = "";

            // Always check metagestures first
            foreach (KeyValuePair<Gesture, string> dictGesture in metaGestureToMeaning)
            {
                Gesture metaGesture = dictGesture.Key;
                float matchVariance = gesture.GetMatchVariance(metaGesture);

                if (
                    (matchVariance <= (metaGesture.matchThresholdPerPoseNr * metaGesture.poseSequence.Count)) 
                    && bestMatchVariance > matchVariance)
                {
                    bestMatchVariance = matchVariance;
                    bestMatch = dictGesture.Value;
                }
            }

            if (!bestMatch.Equals(""))
                return bestMatch;

            foreach (KeyValuePair<Gesture, string> dictGesture in gestureToMeaning) {
                float matchVariance = gesture.GetMatchVariance(dictGesture.Key);
                if (bestMatchVariance > matchVariance) {
                    bestMatchVariance = matchVariance;
                    bestMatch = dictGesture.Value;
                }
            }

            return bestMatch;
        }
    }
}

