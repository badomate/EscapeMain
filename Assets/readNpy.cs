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
        Debug.Log("Shape of the array: " + npArray.shape);
        Debug.Log(npArray[0]);
    
    }
}
