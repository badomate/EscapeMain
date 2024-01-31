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

    //Define what landmarks are part of what limb. Useful for self-pointing
    public static Dictionary<Pose.Landmark, List<Pose.Landmark>> landmarkToLimb = new Dictionary<Pose.Landmark, List<Pose.Landmark>>
    {
        {
            Pose.Landmark.LEFT_WRIST, new List<Pose.Landmark>
            {
                Pose.Landmark.LEFT_WRIST_ROOT,
                Pose.Landmark.LEFT_THUMB,
                Pose.Landmark.LEFT_INDEX,
                Pose.Landmark.LEFT_MIDDLE,
                Pose.Landmark.LEFT_RING,
                Pose.Landmark.LEFT_PINKY,
                Pose.Landmark.LEFT_THUMB_BASE,
                Pose.Landmark.LEFT_INDEX_BASE,
                Pose.Landmark.LEFT_MIDDLE_BASE,
                Pose.Landmark.LEFT_RING_BASE,
                Pose.Landmark.LEFT_PINKY_BASE,
                Pose.Landmark.LEFT_THUMB_KNUCKLE,
                Pose.Landmark.LEFT_INDEX_KNUCKLE,
                Pose.Landmark.LEFT_MIDDLE_KNUCKLE,
                Pose.Landmark.LEFT_RING_KNUCKLE,
                Pose.Landmark.LEFT_PINKY_KNUCKLE
            }
        },
        {
            Pose.Landmark.RIGHT_WRIST, new List<Pose.Landmark>
            {
                Pose.Landmark.RIGHT_WRIST_ROOT,
                Pose.Landmark.RIGHT_THUMB,
                Pose.Landmark.RIGHT_INDEX,
                Pose.Landmark.RIGHT_MIDDLE,
                Pose.Landmark.RIGHT_RING,
                Pose.Landmark.RIGHT_PINKY,
                Pose.Landmark.RIGHT_THUMB_BASE,
                Pose.Landmark.RIGHT_INDEX_BASE,
                Pose.Landmark.RIGHT_MIDDLE_BASE,
                Pose.Landmark.RIGHT_RING_BASE,
                Pose.Landmark.RIGHT_PINKY_BASE,
                Pose.Landmark.RIGHT_THUMB_KNUCKLE,
                Pose.Landmark.RIGHT_INDEX_KNUCKLE,
                Pose.Landmark.RIGHT_MIDDLE_KNUCKLE,
                Pose.Landmark.RIGHT_RING_KNUCKLE,
                Pose.Landmark.RIGHT_PINKY_KNUCKLE
            }
        },
        {
            // Add similar mappings for LEFT_FOOT and RIGHT_FOOT, including all related foot landmarks
            Pose.Landmark.LEFT_FOOT, new List<Pose.Landmark>
            {
                Pose.Landmark.LEFT_FOOT,
                Pose.Landmark.LEFT_KNEE,
                Pose.Landmark.LEFT_HIP,
            }
        },
        {
            Pose.Landmark.RIGHT_FOOT, new List<Pose.Landmark>
            {
                Pose.Landmark.RIGHT_FOOT,
                Pose.Landmark.RIGHT_KNEE,
                Pose.Landmark.RIGHT_HIP,
            }
        }
    };

}
