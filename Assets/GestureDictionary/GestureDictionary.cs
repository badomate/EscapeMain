using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureDictionary : MonoBehaviour
{
    private Dictionary<Gesture, string> gestureToMeaning;
    private Dictionary<string, Gesture> meaningToGesture;

    public GestureDictionary() {
        gestureToMeaning = new Dictionary<Gesture, string>();
        meaningToGesture = new Dictionary<string, Gesture>();
    }

    // Here, compare every gesture in the dictionary
    public string GetMeaningFromGesture(Gesture gesture, bool isBasic=false) {
        if (isBasic) return GetMeaningFromGestureBasic(gesture);
        else return GetMeaningFromGestureComplex(gesture);
    }

    public string GetMeaningFromGestureBasic(Gesture gesture) {
        if (gestureToMeaning.ContainsKey(gesture)) {
            return gestureToMeaning[gesture];
        }

        return "";
    }

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

    public Gesture GetGestureFromMeaning(string meaning) {
        if (meaningToGesture.ContainsKey(meaning)) {
            return meaningToGesture[meaning];
        }
        return null;
    }
}
