using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary.ContentGenerators;

namespace GestureDictionary {
    public class DictionaryManager
    {
        private Dictionary<Gesture, string> _gestureToMeaning;
        private Dictionary<Gesture, string> _metaGestureToMeaning;
        private Dictionary<string, Gesture> _meaningToGesture;

        private Dictionary<string, Pose> _knownPoses;

        public DictionaryManager() {
            _gestureToMeaning = new Dictionary<Gesture, string>();
            _metaGestureToMeaning = new Dictionary<Gesture, string>();
            _meaningToGesture = new Dictionary<string, Gesture>();
            _knownPoses = new Dictionary<string, Pose>();

            PoseGenerator.GenerateStarterPoses(this);
            GestureGenerator.GenerateMetaGestures(this);
            GestureGenerator.GenerateStarterGestures(this);

            Debug.Log("GTM:" + _gestureToMeaning.Keys.Count);
            Debug.Log("MGTM:" + _meaningToGesture.Keys.Count);
        }

        public Dictionary<string, Pose> GetKnownPoses()
        {
            return _knownPoses;
        }

        public Dictionary<Gesture, string> GetGestureRegistry()
        {
            return _gestureToMeaning;
        }

        public Dictionary<Gesture, string> GetMetaGestureRegistry()
        {
            return _metaGestureToMeaning;
        }

        public Dictionary<string, Gesture> GetMeaningRegistry()
        {
            return _meaningToGesture;
        }

        /// <summary> Adds a gesture to registries of known gestures </summary>
        public void AddGesture(Gesture gesture, string meaning, bool isMeta = false) {
            if (isMeta) {
                _metaGestureToMeaning.Add(gesture, meaning);
            }
            else
            {
                _gestureToMeaning.Add(gesture, meaning);
            }
            
            _meaningToGesture.Add(meaning, gesture);
        }

        /// <summary> Updates a gesture's meaning </summary>
        public void UpdateGestureMeaning(Gesture gesture, string meaning) {
            if (!_gestureToMeaning.ContainsKey(gesture) || !_meaningToGesture.ContainsKey(meaning)) {
                AddGesture(gesture, meaning);
            }
            else {
                _gestureToMeaning[gesture] = meaning;
                _meaningToGesture[meaning] = gesture;
            }
        }

        /// <summary> Returns the gesture with the requested meaning (if known). </summary>
        public Gesture GetGestureFromMeaning(string meaning) {
            if (_meaningToGesture.ContainsKey(meaning)) {
                return _meaningToGesture[meaning];
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
            if (_metaGestureToMeaning.ContainsKey(gesture)) return _metaGestureToMeaning[gesture];
            else if (_gestureToMeaning.ContainsKey(gesture)) return _gestureToMeaning[gesture];
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
            foreach (KeyValuePair<Gesture, string> dictGesture in _metaGestureToMeaning)
            {
                Gesture metaGesture = dictGesture.Key;
                float matchVariance = gesture.GetMatchVariance(metaGesture);

                if (
                    (matchVariance <= (metaGesture._matchThresholdPerPoseNr * metaGesture._poseSequence.Count)) 
                    && bestMatchVariance > matchVariance)
                {
                    bestMatchVariance = matchVariance;
                    bestMatch = dictGesture.Value;
                }
            }

            if (!bestMatch.Equals(""))
                return bestMatch;

            foreach (KeyValuePair<Gesture, string> dictGesture in _gestureToMeaning) {
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

