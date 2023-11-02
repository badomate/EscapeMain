using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AuxiliarContent;
using Unity.VisualScripting;
using UnityEngine;

/// <summary> A pose corresponds to a specific (and static) set of landmark positions. </summary>
public class Pose
{
    /// <summary>
    /// Valid landmarks. Each correspond to a point in the player's body.
    /// </summary>
    public enum Landmark
    {
        //base landmarks (endpoints)
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_FOOT,
        RIGHT_FOOT,

        //hint landmarks
        RIGHT_ELBOW,
        LEFT_ELBOW,
        RIGHT_KNEE,
        LEFT_KNEE,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,

        LEFT_WRIST_PIVOTRIGHT,
        LEFT_WRIST_PIVOTLEFT,
        RIGHT_WRIST_PIVOTRIGHT,
        RIGHT_WRIST_PIVOTLEFT,

        //hand landmarks
        LEFT_WRIST_ROOT,
        RIGHT_WRIST_ROOT,

        RIGHT_INDEX,
        RIGHT_THUMB,
        RIGHT_RING,
        RIGHT_PINKY,
        RIGHT_MIDDLE,

        LEFT_INDEX,
        LEFT_THUMB,
        LEFT_RING,
        LEFT_PINKY,
        LEFT_MIDDLE

    }

    private const float DEFAULT_VARIANCE = 3;

    public static List<Landmark> LandmarkIds = new List<Landmark>() {
        Landmark.LEFT_WRIST,
        Landmark.RIGHT_WRIST
    };

    /// <summary> 
    /// Match between a landmark and its position on the pose, relative to the player's location.
    /// Not all landmarks are relevant for a given pose. 
    /// </summary>
    public Dictionary<Landmark, Vector3> _landmarkArrangement;

    private static Vector3 DefaultLandmarkPosition = Vector3.zero;

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

            float landmarkVarianceSquared =  otherPose._landmarkArrangement.ContainsKey(landmarkPos.Key)?
            GetLandmarkVarianceSquared(
                landmarkPos.Value,
                otherPose._landmarkArrangement[landmarkPos.Key]
            ) : DEFAULT_VARIANCE;     
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
    public string ToString(bool allLandmarks = false)
    {
        StringBuilder poseStringBuilder = new StringBuilder();

        for (int i = 0; i < LandmarkIds.Count; i++) {
            Landmark landmark = LandmarkIds[i];
            bool isLandmarkRelevant = _landmarkArrangement.
                                                    TryGetValue(landmark, out Vector3 landmarkPos);
            if (!isLandmarkRelevant)
                landmarkPos = DefaultLandmarkPosition;

            if (allLandmarks) {
                string landmarkPosString = 
                    $" {landmark}: Position=[{landmarkPos[0]}, {landmarkPos[1]},  {landmarkPos[2]}].";
                poseStringBuilder.Append(landmarkPosString);
            }
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
        CustomDebug.LogAlex("Pose str received:" + poseString);

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

            CustomDebug.LogAlex("Pose[" + currentLandmarkId + "]: " + pose._landmarkArrangement[currentLandmarkId]);
            nrLandmarksRegistered++;
        }

        return pose;
    }

    public static Pose GetPoseFromArray(Vector3[] poseArray)
    {
        Pose pose = new Pose();

        int totalLandmarks = LandmarkIds.Count;

        for (int i = 0; i < totalLandmarks; i++)
        {
            Landmark landmark = LandmarkIds[i];
            Vector3 landmarkPosition = poseArray[i];
            bool isLandmarkRelevant = !landmarkPosition.Equals(Vector3.zero);

            if (isLandmarkRelevant)
                pose._landmarkArrangement.Add(landmark, landmarkPosition);
        }
        return pose;
    }

    /// <summary> Format: " LM0 Position=[ lm00Pos_x, lm00Pos_y,  lm00Pos_z]. LM1 Position=[ lm01Pos_x, lm01Pos_y,  lm01Pos_z]." </summary>
    public static Vector3[] GetPoseVectorFromString(string poseString, Regex poseRegex)
    {
        MatchCollection landmarkPositions = poseRegex.Matches(poseString);
        int nrLandmarksRegistered = 0;

        int totalLandmarks = landmarkPositions.Count;
        CustomDebug.LogAlex("Received pose. #LM=" + totalLandmarks);

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


    public static Vector3[] GetPoseVectors(Pose pose)
    {
        int totalLandmarks = LandmarkIds.Count;
        Vector3[] poseVectors = new Vector3[totalLandmarks];

        for (int i = 0; i < totalLandmarks; i++)
        {
            Landmark landmark = LandmarkIds[i];
            bool isLandmarkRelevant = pose._landmarkArrangement.
                                                    TryGetValue(landmark, out Vector3 landmarkPosition);
            if (!isLandmarkRelevant)
                landmarkPosition = DefaultLandmarkPosition;

            poseVectors[i] = landmarkPosition;
        }
        return poseVectors;
    }

    public void RotatePose(Quaternion rotation)
    {
        foreach (var landmark in _landmarkArrangement.Keys.ToList())
        {
            Vector3 originalPosition = _landmarkArrangement[landmark];
            Vector3 rotatedPosition = rotation * originalPosition;
            _landmarkArrangement[landmark] = rotatedPosition;
        }
    }
}
