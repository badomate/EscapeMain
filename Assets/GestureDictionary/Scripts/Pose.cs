using System.Collections.Generic;
using System.Text;
using UnityEngine;

// A pose corresponds to a specific (and static) set oflandmark positions.
public class Pose : MonoBehaviour
{
    // Valid landmarks. Each correspond to a point in the player's body.
    public enum Landmark
    {
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_FOOT,
        RIGHT_FOOT
    }

    public static List<Landmark> landmarkIds = new List<Landmark>() {
        Landmark.LEFT_WRIST
    };

    // Match between a landmark and its position on the pose, relative to the player's location.
    // Not all landmarks are relevant for a given pose.
    public Dictionary<Landmark, Vector3> landmarkArrangement = new Dictionary<Landmark, Vector3>();
    
    public Pose() {}

    public Pose(Dictionary<Landmark, Vector3> arranjement) {
        landmarkArrangement = arranjement;
    }


    public float MatchVariance(Pose otherPose) {
        float matchVariance = 0f;
        float matchVarianceSquared = 0f;
        foreach(KeyValuePair<Landmark, Vector3> landmarkPos in landmarkArrangement) {
            /*
            Vector3 refArranjement = landmarkPos.Value;
            Vector3 otherArranjement = otherPose.landmarkArrangement[landmarkPos.Key]; 
            Debug.Log("LM: landmarkPos.Key" + ". Ref ": + refArranjement + ". Other: " + otherArranjement + ".");
            */

            float landmarkVarianceSquared =  GetLandmarkVarianceSquared(
                landmarkPos.Value,
                otherPose.landmarkArrangement[landmarkPos.Key]
            );     
            // matchVarianceSquared += Mathf.Pow(landmarkVariance, 2); // sqrt(a)**2 = a
            matchVarianceSquared += landmarkVarianceSquared;
        }

        matchVariance = Mathf.Sqrt(matchVarianceSquared);
        return matchVariance;
    }

    // Returns the variance between a landmark's position and its reference value
    public float GetLandmarkVariance(Vector3 reference, Vector3 toCompare) {
        float varianceSquared = GetLandmarkVarianceSquared(reference, toCompare);
        return Mathf.Sqrt(varianceSquared);
    }

    // Returns the squared variance between a landmark's position and its reference value
    public float GetLandmarkVarianceSquared(Vector3 reference, Vector3 toCompare) {
        float varianceSquared = 0f;

        for (int i = 0; i < 3; i++) {
            varianceSquared += Mathf.Pow(
                (reference[i]-toCompare[i]), 
                2);
        }

        return varianceSquared;
    }

    // Format: "[[lm00Pos_x, lm00Pos_y,  lm00Pos_z] [lm01Pos_x, lm01Pos_y,  lm01Pos_z]]"
    public override string ToString()
    {
        string poseString = "[";

        for (int j = 0; j < Pose.landmarkIds.Count; j++) {
            Pose.Landmark landmark = Pose.landmarkIds[j];
            Vector3 pos = landmarkArrangement[landmark];
            poseString += "[ " + pos[0] + ", " + pos[1] + ",  " + pos[2] + "]  ";
        }

        if (Pose.landmarkIds.Count > 0)
            poseString = poseString.Remove(poseString.Length-1);
        poseString += "]";

        return poseString;
    }
}
