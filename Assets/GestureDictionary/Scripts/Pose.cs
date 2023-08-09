using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary> A pose corresponds to a specific (and static) set of landmark positions. </summary>
public class Pose
{
    /// <summary>
    /// Valid landmarks. Each correspond to a point in the player's body.
    /// </summary>
    public enum Landmark
    {
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_FOOT,
        RIGHT_FOOT
    }

    public static List<Landmark> LandmarkIds = new List<Landmark>() {
        Landmark.LEFT_WRIST
    };

    /// <summary> 
    /// Match between a landmark and its position on the pose, relative to the player's location.
    /// Not all landmarks are relevant for a given pose. 
    /// </summary>
    public Dictionary<Landmark, Vector3> _landmarkArrangement;

    public Pose() {
        _landmarkArrangement = new Dictionary<Landmark, Vector3>();
    }

    public Pose(Dictionary<Landmark, Vector3> arranjement) {
        _landmarkArrangement = arranjement;
    }

    public float MatchVariance(Pose otherPose) {
        float matchVarianceSquared = 0f;
        foreach(KeyValuePair<Landmark, Vector3> landmarkPos in _landmarkArrangement) {
            /*
            Vector3 refArranjement = landmarkPos.Value;
            Vector3 otherArranjement = otherPose.landmarkArrangement[landmarkPos.Key]; 
            Debug.Log("LM: landmarkPos.Key" + ". Ref ": + refArranjement + ". Other: " + otherArranjement + ".");
            */

            float landmarkVarianceSquared =  GetLandmarkVarianceSquared(
                landmarkPos.Value,
                otherPose._landmarkArrangement[landmarkPos.Key]
            );     
            // matchVarianceSquared += Mathf.Pow(landmarkVariance, 2); // sqrt(a)**2 = a
            matchVarianceSquared += landmarkVarianceSquared;
        }

        return Mathf.Sqrt(matchVarianceSquared);
    }

    /// <summary> Returns the variance between a landmark's position and its reference value </summary>
    public float GetLandmarkVariance(Vector3 reference, Vector3 toCompare) {
        float varianceSquared = GetLandmarkVarianceSquared(reference, toCompare);
        return Mathf.Sqrt(varianceSquared);
    }

    /// <summary> 
    /// Returns the squared variance between 
    /// a landmark's position and its reference value 
    /// </summary>
    public float GetLandmarkVarianceSquared(Vector3 reference, Vector3 toCompare) {
        float varianceSquared = 0f;

        for (int i = 0; i < 3; i++) {
            varianceSquared += Mathf.Pow(
                reference[i]-toCompare[i], 
                2);
        }

        return varianceSquared;
    }

    /// <summary> Format: "LM0 Position=[lm00Pos_x, lm00Pos_y,  lm00Pos_z]. LM1 Position=[lm01Pos_x, lm01Pos_y,  lm01Pos_z]" </summary>
    public override string ToString()
    {
        StringBuilder poseStringBuilder = new StringBuilder();

        for (int j = 0; j < LandmarkIds.Count; j++) {
            Landmark landmark = LandmarkIds[j];
            Vector3 landmarkPos = _landmarkArrangement[landmark];
            string landmarkPosString = 
                $" {landmark}: Position=[{landmarkPos[0]}, {landmarkPos[1]},  {landmarkPos[2]}].";
            poseStringBuilder.Append(landmarkPosString);
        }

        string poseString = poseStringBuilder.ToString();
        return poseString;
    }

    /// <summary> Format: " LM0 Position=[ lm00Pos_x, lm00Pos_y,  lm00Pos_z]. LM1 Position=[ lm01Pos_x, lm01Pos_y,  lm01Pos_z]." </summary>
    public static Pose GetPoseFromString(string poseString, Regex poseRegex)
    {
        Pose pose = new Pose();

        MatchCollection landmarkPositions = poseRegex.Matches(poseString);
        int nrLandmarksRegistered = 0;
        Debug.Log("Pose str received:" + poseString);

        foreach (Match landmarkPosition in landmarkPositions.Cast<Match>())
        {
            GroupCollection landmarkCoordinate = landmarkPosition.Groups;
            Landmark currentLandmarkId = LandmarkIds[nrLandmarksRegistered];
            pose._landmarkArrangement[currentLandmarkId] = 
                new Vector3(
                    float.Parse(landmarkCoordinate["x"].Value),
                    float.Parse(landmarkCoordinate["y"].Value),
                    float.Parse(landmarkCoordinate["z"].Value))
                ;

            Debug.Log("Pose[" + currentLandmarkId + "]: " + pose._landmarkArrangement[currentLandmarkId]);
            nrLandmarksRegistered++;
        }

        return pose;
    }

    /// <summary> Format: " LM0 Position=[ lm00Pos_x, lm00Pos_y,  lm00Pos_z]. LM1 Position=[ lm01Pos_x, lm01Pos_y,  lm01Pos_z]." </summary>
    public static Vector3[] GetPoseVectorFromString(string poseString, Regex poseRegex)
    {
        MatchCollection landmarkPositions = poseRegex.Matches(poseString);
        int nrLandmarksRegistered = 0;
        
        Debug.Log("Pose str received:" + poseString);
        Debug.Log("Regex= " + poseRegex);

        int totalLandmarks = landmarkPositions.Count;

        Vector3[] poseVector = new Vector3[totalLandmarks];

        foreach (Match landmarkPosition in landmarkPositions.Cast<Match>())
        {
            GroupCollection landmarkCoordinate = landmarkPosition.Groups;
            Landmark currentLandmarkId = LandmarkIds[nrLandmarksRegistered];
            Vector3 landmarkVector = new Vector3(
                    float.Parse(landmarkCoordinate["x"].Value),
                    float.Parse(landmarkCoordinate["y"].Value),
                    float.Parse(landmarkCoordinate["z"].Value))
                ;

            Debug.Log("Pose[" + currentLandmarkId + "]: " + landmarkVector);
            
            poseVector[nrLandmarksRegistered] = landmarkVector;
            nrLandmarksRegistered++;
        }

        return poseVector;
    }
}
