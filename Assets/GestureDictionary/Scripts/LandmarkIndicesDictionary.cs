using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LandmarkIndicesDictionary
{
    public static Dictionary<Pose.Landmark, int> mediapipeIndices = new Dictionary<Pose.Landmark, int>();
    public static Dictionary<Pose.Landmark, List<int>> mediapipeIndicesToLimbs = new Dictionary<Pose.Landmark, List<int>>();


    public static Dictionary<Pose.Landmark, int> cocoIndices = new Dictionary<Pose.Landmark, int>();

    static LandmarkIndicesDictionary()
    {
        // Manually add elements to the dictionary
        mediapipeIndices.Add(Pose.Landmark.LEFT_WRIST, 15);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_WRIST, 16);
        mediapipeIndices.Add(Pose.Landmark.LEFT_FOOT, 27);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_FOOT, 28);


        mediapipeIndices.Add(Pose.Landmark.LEFT_KNEE, 25);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_KNEE, 26);
        mediapipeIndices.Add(Pose.Landmark.LEFT_ELBOW, 13);
        mediapipeIndices.Add(Pose.Landmark.RIGHT_ELBOW, 14);

        //mediapipeIndices.Add(Pose.Landmark.RIGHT_INDEX, 20);

        mediapipeIndicesToLimbs.Add(Pose.Landmark.LEFT_WRIST, new List<int>{ 11,13,15,17,19,21});
        mediapipeIndicesToLimbs.Add(Pose.Landmark.RIGHT_WRIST, new List<int> { 12,14,16,18,20,22});
        mediapipeIndicesToLimbs.Add(Pose.Landmark.LEFT_FOOT, new List<int> { 25,27,29,31});
        mediapipeIndicesToLimbs.Add(Pose.Landmark.RIGHT_FOOT, new List<int> { 26,28,30,32});



        cocoIndices.Add(Pose.Landmark.NOSE, 0);
        cocoIndices.Add(Pose.Landmark.LEFT_EAR, 3);
        cocoIndices.Add(Pose.Landmark.RIGHT_EAR, 4);

        cocoIndices.Add(Pose.Landmark.LEFT_SHOULDER, 5);
        cocoIndices.Add(Pose.Landmark.RIGHT_SHOULDER, 6);

        cocoIndices.Add(Pose.Landmark.LEFT_ELBOW, 7);
        cocoIndices.Add(Pose.Landmark.RIGHT_ELBOW, 8);
        cocoIndices.Add(Pose.Landmark.LEFT_WRIST, 9);
        cocoIndices.Add(Pose.Landmark.RIGHT_WRIST, 10);

        cocoIndices.Add(Pose.Landmark.LEFT_HIP, 11);
        cocoIndices.Add(Pose.Landmark.RIGHT_HIP, 12);

        cocoIndices.Add(Pose.Landmark.LEFT_KNEE, 13);
        cocoIndices.Add(Pose.Landmark.RIGHT_KNEE, 14);
        cocoIndices.Add(Pose.Landmark.LEFT_FOOT, 15);
        cocoIndices.Add(Pose.Landmark.RIGHT_FOOT, 16);

    }
}
