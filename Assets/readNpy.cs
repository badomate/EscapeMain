using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;

public class readNpy : MonoBehaviour
{
    void Start()
    {
        // Specify the path to your .npy file
        string filePath = "Assets/GestureDictionary/Recordings/points_3d_list.npy";

        // Read the .npy file
        NDArray npArray = np.load(filePath);

        // Access and manipulate the data as needed
        // For example, print the shape of the array
        //Debug.Log("Shape of the array: " + npArray.shape);
        //Debug.Log(npArray[0]);

        List<Pose> poseList = new List<Pose>();

        for (int i = 0; i < 100; i++) //TODO: want npArray.shape[0] instead but gives errors
        {
            Pose currentPose = new Pose();
            foreach (var landmark in LandmarkIndicesDictionary.cocoIndices.Keys)
            {
                int landmarkIndex = LandmarkIndicesDictionary.cocoIndices[landmark];
                float x = npArray[i, landmarkIndex, 0];
                float y = npArray[i, landmarkIndex, 1];
                float z = npArray[i, landmarkIndex, 2];
                Vector3 landmarkPosition = new Vector3(x, y, z);

                currentPose._landmarkArrangement.Add(landmark, landmarkPosition);
            }
            poseList.Add(currentPose);
        }
        Gesture endResult = new Gesture();
        endResult.AddPoses(poseList);
    }
}
