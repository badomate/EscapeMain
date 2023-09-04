using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LandmarkIndicesDictionary
{
    public static Dictionary<Pose.Landmark, int> mediapipeIndices = new Dictionary<Pose.Landmark, int>();
    static LandmarkIndicesDictionary()
    {
        // Manually add elements to the dictionary
        mediapipeIndices.Add(Pose.Landmark.LEFT_WRIST, 15);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_WRIST, 16);
        mediapipeIndices.Add(Pose.Landmark.LEFT_FOOT, 27);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_FOOT, 28);
    }

}
