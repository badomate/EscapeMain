using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> A list of sequential poses, with a specific frame (time) interval and (pose) match threshold. </summary>
public class Gesture : MonoBehaviour
{
    /// <summary>
    /// Pose within a gesture,
    /// with information about the pose,
    /// the time between transition to the next pose,
    /// and how closely these need to match another to be called equal
    /// </summary>
    public class PoseInGesture {
        public Pose poseToMatch;
        public float matchThreshold; 
        public float frameInterval;
        public float frameIntervalThreshold;

        public PoseInGesture() {
            poseToMatch = new Pose();
            matchThreshold = 0.1f;
            frameInterval = Mathf.Pow(30f, -1); // 30Hz / 30fps
            frameIntervalThreshold = 0.2f;
        }
    }

    public List<PoseInGesture> poseSequence;
    public float matchThresholdPerPoseNr;

    public Gesture() {
        poseSequence = new List<PoseInGesture>();
        matchThresholdPerPoseNr = 0.5f;
    }

    /// <summary>
    /// Adds poses to a gesture's pose sequence
    /// </summary>
    public void AddPoses(List<Pose> poses, 
                                    List<float> matchThresholds = null, 
                                    List<float> frameIntervals = null,
                                    List<float> frameIntervalThresholds = null) {
        for (int i = 0; i < poses.Count; i++) {
            PoseInGesture poseInGesture = new PoseInGesture();
            poseInGesture.poseToMatch = poses[i];
            poseInGesture.matchThreshold = matchThresholds == null? 
                                poseInGesture.matchThreshold : matchThresholds[i];
            poseInGesture.frameInterval = frameIntervals == null? 
                                poseInGesture.frameInterval : frameIntervals[i];
            poseInGesture.frameIntervalThreshold = frameIntervalThresholds == null? 
                                poseInGesture.frameIntervalThreshold : frameIntervalThresholds[i];

            poseSequence.Add(poseInGesture);
        }
    } 

    /// <summary>
    /// Converts a gesture to a matrix of coordinates, where each row represents a pose
    /// and each column represents a landmark.
    /// matrix[i, j] represents the position of landmark "j" in pose "i"
    /// </summary>
    public Vector3[,] GestureToMatrix() {
        Vector3[,] gestureMatrix = new Vector3 [poseSequence.Count, Pose.landmarkIds.Count];
        for (int i = 0; i < poseSequence.Count; i++) {
            for (int j = 0; j < Pose.landmarkIds.Count; j++) {
                Pose.Landmark landmark = Pose.landmarkIds[j];
                Dictionary<Pose.Landmark, Vector3> landmarkArrangement = poseSequence[i].
                                                                                                    poseToMatch.
                                                                                                    landmarkArrangement;

                Vector3 pos = landmarkArrangement.ContainsKey(landmark) ?
                                        landmarkArrangement[landmark]: Vector3.zero;
                
                gestureMatrix[i,j] = pos;
            }
        }
        return gestureMatrix;
    }

    /// <summary>
    /// Returns true if the none of the poses in the 
    /// other gesture's pose sequence vary more than the 
    /// match threshold from the current gesture.
    /// </summary>
    public bool GestureMatches(Gesture otherGesture) {
        for (int i = 0; i < poseSequence.Count; i++) {
            Pose poseRef = poseSequence[i].poseToMatch;
            Pose poseOther = otherGesture.poseSequence[i].poseToMatch;
            float poseMatchVariance = poseRef.MatchVariance(poseOther);

            if (poseMatchVariance > poseSequence[i].matchThreshold)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns a value representing how much the poses in the 
    /// other gesture's pose sequence vary from the current gestures'.
    /// </summary>
    public float GetMatchVariance(Gesture otherGesture) {
        float matchVarianceSquared = 0f;
        for (int i = 0; i < poseSequence.Count; i++) {
            Pose poseRef = poseSequence[i].poseToMatch;
            Pose poseOther = otherGesture.poseSequence[i].poseToMatch;
            float poseMatchVariance = poseRef.MatchVariance(poseOther);
 
            matchVarianceSquared += Mathf.Pow(poseMatchVariance, 2);
        }

        return Mathf.Sqrt(matchVarianceSquared);
    }
}