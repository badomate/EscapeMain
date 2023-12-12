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
    public RiggingIK rigToVisualizeOn;
    public static Gesture endResult;

    public bool playAgain = true;
    public bool FormattingType1 = false;
    public string recordingToPlay = "points_3d_list_normalized";

    void Update()
    {
        if (playAgain) //bit of a workaround to make sure everything else is already initialized and this only plays once
        {
            playAgain = false;
            string filePath = "Assets/GestureDictionary/Recordings/"+ recordingToPlay+".npy";
            NDArray npArray = np.load(filePath);

            List<Pose> poseList = new List<Pose>();
            for (int i = 0; i < npArray.shape[0]; i++) 
            {
                Pose currentPose = new Pose();
                foreach (var landmark in LandmarkIndicesDictionary.cocoIndices.Keys)
                {
                    Vector3 landmarkPosition;
                    if (FormattingType1) //if shape is like ...,3,17
                    {
                        int landmarkIndex = LandmarkIndicesDictionary.cocoIndices[landmark];
                        double x = npArray[i, 0, landmarkIndex];
                        double y = npArray[i, 1, landmarkIndex];
                        double z = npArray[i, 2, landmarkIndex];

                        landmarkPosition = new Vector3((float)x, -(float)y, (float)z);
                    }
                    else //if shape is like ...,17,3
                    {
                        int landmarkIndex = LandmarkIndicesDictionary.cocoIndices[landmark];
                        float x = npArray[i, landmarkIndex, 0];
                        float y = npArray[i, landmarkIndex, 1];
                        float z = npArray[i, landmarkIndex, 2];
                        landmarkPosition = new Vector3(x, -y, z);
                    }

                    currentPose._landmarkArrangement.Add(landmark, landmarkPosition);
                }
                poseList.Add(currentPose);
            }
            endResult = new Gesture();
            endResult.AddPoses(poseList);

            Debug.Log(endResult._poseSequence.Count);
            StartCoroutine(rigToUse.playGesture(endResult));
            StartCoroutine(rigToVisualizeOn.playGesture(endResult));
        }
    }
}
