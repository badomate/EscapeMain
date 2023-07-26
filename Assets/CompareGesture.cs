using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareGesture : MonoBehaviour
{
    public static int recordingLength = 2; //how many frames do we save for comparison? should match the dictionary
    public static int sampleLength = 3; //how many coordinates are we receiving in total? (e.g. 3 keypoints are likely equal to a sample length of 9)

    public bool recording = true;

    public float[,] characterGesture = new float[recordingLength, sampleLength];
    public float[,] goalGesture = new float[recordingLength, sampleLength];
    private int recordingProgress = 0; //how many samples of the currently playing gesture have we saved so far
    private float matchThreshold = 0.1f; //0 would mean an absolute perfect match across all samples

    // Start is called before the first frame update
    void Start()
    {
        goalGesture = new float[,] { { 4, 5, 6 }, { 4, 5, 6 } }; //example value for testing
        //Debug.Log(MeanSquaredError(characterGesture, goalGesture) < matchThreshold);
    }


    private float frameInterval = 1f / 30f; // 30hz
    private float timeSinceLastFrame = 0f;
    // Update is called once per frame
    void Update()
    {
        timeSinceLastFrame += Time.deltaTime;

        // Check if the desired time interval has passed (30fps)
        if (timeSinceLastFrame >= frameInterval)
        {
            saveGestureFrame();
            if (goalGestureCompleted())
            {
                Debug.Log("Goal gesture was performed correctly.");
                recording = false; recordingProgress = 0;
            }
            timeSinceLastFrame = 0f; // Reset the time counter
        }
    }

    public bool goalGestureCompleted()
    {
        return recording && recordingProgress == recordingLength && MeanSquaredError(characterGesture, goalGesture) < matchThreshold;

    }

    private Socket_toHl2 TcpScript;
    //We save the gesture's samples received through TCP as a matrix and keep comparing it to the goal until they match. Every row is a sample (at 30hz) from hololens
    //If the matrix is full, we will throw away the oldest sample so we can keep matrix size the same
    public void saveGestureFrame()
    {
        if (recording)
        {
            TcpScript = GetComponent<Socket_toHl2>();
            float[] currentPositions = { TcpScript.position.x, TcpScript.position.y, TcpScript.position.z };

            
            if (recordingProgress < recordingLength) //building up the matrix
            {
                for (int i = 0; i < sampleLength; i++)
                {
                    characterGesture[recordingProgress, i] = currentPositions[i];
                }
                recordingProgress++;
            }
            else //updating the matrix
            {
                for (int i = 0; i < characterGesture.GetLength(0) -1; i++)
                {
                    for (int j = 0; j < sampleLength; j++)
                    {
                        characterGesture[i, j] = characterGesture[i + 1, j];
                    }
                }
                for (int i = 0; i < sampleLength; i++)
                {
                    characterGesture[characterGesture.GetLength(0) - 1, i] = currentPositions[i];
                }

            }
        }
    }

    public float MeanSquaredError(float[,] matrix1, float[,] matrix2)
    {
        int rows = matrix1.GetLength(0);
        int cols = matrix1.GetLength(1);
        float sumOfSquaredDifferences = 0.0f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float diff = matrix1[i, j] - matrix2[i, j];
                sumOfSquaredDifferences += diff * diff;
            }
        }

        return sumOfSquaredDifferences / (rows * cols);
    }
}
