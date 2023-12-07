using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;
using UnityEngine.Experimental.AI;
using System.Linq;

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
            string filePath = "Assets/GestureDictionary/Recordings/points_3d_list.npy";
            NDArray npArray = np.load(filePath);

            List<Pose> poseList = new List<Pose>();

            for (int i = 0; i < npArray.shape[0]; i++) //TODO: want npArray.shape[0] instead but gives errors
            {
                Pose currentPose = new Pose();
                foreach (var landmark in LandmarkIndicesDictionary.cocoIndices.Keys)
                {
                    int landmarkIndex = LandmarkIndicesDictionary.cocoIndices[landmark];
                    float x = npArray[i, 0, landmarkIndex];
                    float y = npArray[i, 1, landmarkIndex];
                    float z = npArray[i, 2, landmarkIndex];

                    x = Mathf.Round(x * 1000f) / 1000f;
                    y = Mathf.Round(y * 1000f) / 1000f;
                    z = Mathf.Round(z * 1000f) / 1000f;

                    Vector3 landmarkPosition = new Vector3(x, y, z);

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
