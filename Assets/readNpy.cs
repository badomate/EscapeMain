using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;
using UnityEngine.Experimental.AI;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class readNpy : MonoBehaviour
{
    public RiggingIK rigToUse;
    public static Gesture endResult;

    bool playing = true;

    void Update()
    {
        if (playing) //bit of a workaround to make sure everything else is already initialized and this only plays once
        {
            playing = false;
            string filePath = "Assets/GestureDictionary/Recordings/points_3d_list_normalized.npy";
            NDArray npArray = np.load(filePath);

            List<Pose> poseList = new List<Pose>();
            for (int i = 0; i < npArray.shape[0]; i++) 
            {
                Pose currentPose = new Pose();
                foreach (var landmark in LandmarkIndicesDictionary.cocoIndices.Keys)
                {
                    int landmarkIndex = LandmarkIndicesDictionary.cocoIndices[landmark];
                    double x = npArray[i, 0, landmarkIndex];
                    double y = npArray[i, 1, landmarkIndex];
                    double z = npArray[i, 2, landmarkIndex];

                    Vector3 landmarkPosition = new Vector3((float)x, -(float)y, (float)z);

                    currentPose._landmarkArrangement.Add(landmark, landmarkPosition);
                }
                poseList.Add(currentPose);
            }
            endResult = new Gesture();
            endResult.AddPoses(poseList);

            Debug.Log(endResult._poseSequence.Count);
            StartCoroutine(rigToUse.playGesture(endResult));
        }
    }
}
