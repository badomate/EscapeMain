using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A list of sequential poses, with a specific frame (time) interval and (pose) match threshold. 
// Each frame and pose allow for an error margin/threshold.
public class Gesture : MonoBehaviour
{
    public class PoseInGesture {
        public Pose poseToMatch;
        public float matchThreshold; 
        public float frameInterval;
        public float frameIntervalThreshold;

        public PoseInGesture() {
            poseToMatch = new Pose();
            matchThreshold = 0.1f;
            frameInterval = Mathf.Pow(30f, -1);
            frameIntervalThreshold = 0.2f;
        }
    }

    public List<PoseInGesture> poseSequence;
    public Gesture() {
        poseSequence = new List<PoseInGesture>();
    }

    public Vector3[,] GestureToMatrix() {
        Vector3[,] gestureArray = new Vector3 [poseSequence.Count, Pose.landmarkIds.Count];
        for (int i = 0; i < poseSequence.Count; i++) {
            for (int j = 0; j < Pose.landmarkIds.Count; j++) {
                Pose.Landmark landmark = Pose.landmarkIds[j];
                Vector3 pos = poseSequence[i].poseToMatch.landmarkArrangement[landmark]; 
                gestureArray[i,j] = pos;
            }
        }
        return gestureArray;
    }

    public bool GestureMatches(Gesture otherGesture) {
        for (int i = 0; i < poseSequence.Count; i++) {
            PoseInGesture poseGestRef = poseSequence[i];
            PoseInGesture poseGestOther = otherGesture.poseSequence[i];
            float matchVariance = poseGestRef.poseToMatch.MatchVariance(poseGestOther.poseToMatch);

            if (matchVariance > poseGestRef.matchThreshold)
                return false;
        }

        return true;
    }

    public float GetMatchVariance(Gesture otherGesture) {
        float matchVarianceSquared = 0f;
        for (int i = 0; i < poseSequence.Count; i++) {
            PoseInGesture poseGestRef = poseSequence[i];
            PoseInGesture poseGestOther = otherGesture.poseSequence[i];
            matchVarianceSquared += Mathf.Pow(poseGestRef.poseToMatch.MatchVariance(poseGestOther.poseToMatch), 2);
        }

        return Mathf.Sqrt(matchVarianceSquared);
    }
}