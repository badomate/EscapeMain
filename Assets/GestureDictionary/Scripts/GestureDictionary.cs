using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureDictionary : MonoBehaviour
{
    private Dictionary<Gesture, string> gestureToMeaning;
    private Dictionary<string, Gesture> meaningToGesture;

    private Dictionary<string, Pose> knownPoses;

    public GestureDictionary() {
        gestureToMeaning = new Dictionary<Gesture, string>();
        meaningToGesture = new Dictionary<string, Gesture>();
        knownPoses = new Dictionary<string, Pose>();
        GeneratePoses();
        GenerateGestures();
    }

    /// <summary> Generates basic known poses </summary>
    private void GeneratePoses() {
        Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandUp = 
            new Dictionary<Pose.Landmark, Vector3>() {
                {Pose.Landmark.LEFT_WRIST, new Vector3(2, 5, 4)}
            };

        Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandMiddle = 
            new Dictionary<Pose.Landmark, Vector3>() {
                {Pose.Landmark.LEFT_WRIST, new Vector3(2, 3, 4)}
            };

        Dictionary<Pose.Landmark, Vector3> dictPoseLeftHandDown = 
            new Dictionary<Pose.Landmark, Vector3>() {
                {Pose.Landmark.LEFT_WRIST, new Vector3(2, 0, 4)}
            };

        knownPoses.Add("leftHandUp", new Pose(dictPoseLeftHandUp));
        knownPoses.Add("leftHandMiddle", new Pose(dictPoseLeftHandMiddle));
        knownPoses.Add("leftHandDown", new Pose(dictPoseLeftHandDown));
    }

    /// <summary> Generates basic known gestures </summary>
    private void GenerateGestures() {
        Gesture gestureHandRises = new Gesture();
        List<Pose> gestureHandRisesPoses = new List<Pose>() {
            knownPoses["leftHandUp"],
            knownPoses["leftHandMiddle"],
            knownPoses["leftHandDown"]
        };
        gestureHandRises.AddPoses(gestureHandRisesPoses);

        Gesture gestureHandFalls = new Gesture();
        List<Pose> gestureHandFallsPoses = new List<Pose>() {
            knownPoses["leftHandDown"],
            knownPoses["leftHandMiddle"],
            knownPoses["leftHandUp"]
        };
        gestureHandRises.AddPoses(gestureHandFallsPoses);

        this.AddGesture(gestureHandRises, "handRises");
        this.AddGesture(gestureHandFalls, "handFalls");
    }

    /// <summary> Adds a gesture to the dictionary of known gestures </summary>
    public void AddGesture(Gesture gesture, string meaning) {
        gestureToMeaning.Add(gesture, meaning);
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
        if (gestureToMeaning.ContainsKey(gesture)) return gestureToMeaning[gesture];
        else return "";
    }

    /// <summary> 
    /// Looks through all known gestures to find the closest match to the one supplied,
    /// then returns its meaning.
    /// </summary> 
    public string GetMeaningFromGestureComplex(Gesture gesture) {
        float bestMatchVariance= Mathf.Infinity;
        string bestMatch = "";

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
