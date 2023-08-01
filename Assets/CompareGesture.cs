using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompareGesture : MonoBehaviour
{
    public static int recordingLength = 2; //how many frames do we save for comparison? should match the dictionary
    public static int sampleLength = 3; //how many coordinates are we receiving in total? (e.g. 3 keypoints are likely equal to a sample length of 9)
    public int stillnessFramesRequired = 2;


    public bool recording = false;

    public float[,] characterGesture = new float[recordingLength, sampleLength];
    public float[,] goalGesture = new float[recordingLength, sampleLength];
    private int recordingProgress = 0; //how many samples of the currently playing gesture have we saved so far
    private float matchThreshold = 0.1f; //0 would mean an absolute perfect match across all samples
    private float stillnessThreshold = 0.1f; //used to "lock in" a pose

    public UnityEvent m_StillnessEvent = new UnityEvent();

    private LevelManager LevelManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        LevelManagerScript = GetComponent<LevelManager>();
    }


    private float frameInterval = 1f / 30f; // 30hz
    private float timeSinceLastFrame = 0f;
    // Update is called once per frame
    void Update()
    {
        timeSinceLastFrame += Time.deltaTime;

        // Check if the desired time interval has passed (30fps)
        if (timeSinceLastFrame >= frameInterval && recording)
        {
            saveGestureFrame();
            detectStillness();  //TODO: only run this function if needed
            if (goalGestureCompleted()) //we could use this to detect other gestures too, not just the solution
            {
                if(LevelManagerScript.currentPlayer == 0) //TODO: implement A.I (player1) performing the goal gesture
                {
                    recording = false; recordingProgress = 0;
                    LevelManagerScript.Success();
                }
            }
            timeSinceLastFrame = 0f; // Reset the time counter
        }
    }

    public void detectStillness()
    {
        // Check if there are enough rows to check for stillness
        if (recordingLength < stillnessFramesRequired)
        {
            Debug.Log("Recording length is set too short");
            return;
        }

        // the difference in each element of the last stillnessFramesRequired rows must be under threshold
        for (int recordingIndex = recordingLength - stillnessFramesRequired; recordingIndex < recordingLength -1; recordingIndex++)
        {
            for (int sampleIndex = 0; sampleIndex < sampleLength; sampleIndex++)
            {
                if (System.Math.Abs(characterGesture[recordingIndex, sampleIndex] - characterGesture[recordingIndex + 1, sampleIndex]) > stillnessThreshold)
                {
                    Debug.Log("Not still");
                    return; // Difference exceeded the threshold
                }
            }
        }
        if (m_StillnessEvent != null)
        {
            m_StillnessEvent.Invoke();
        }
        else
        {
            m_StillnessEvent = new UnityEvent();
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
