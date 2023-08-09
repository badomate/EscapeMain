using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary;
using System.Text.RegularExpressions;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public GameObject GoalDisplayCharacter;
    public int currentPlayer = 1; //0-player, 1-A.I
    public Vector3[,] goalGesture = null;//new float[2, 3]; //change these according to gesture dictionary

    PanopticToIK estimationToIkScript;
    CompareGesture compareGestureScript;
    public GameObject PlayerUI;
    public GameObject InfoBox;
    private bool connected = false; //used for events
    private DictionaryManager dictionary;
    public static Regex _poseRegex;

    void Start()
    {
        dictionary = new DictionaryManager();
        _poseRegex = new Regex("Position=\\[\\s(?<x>-?\\d+(?:\\.\\d+)?),\\s(?<y>-?\\d+(?:\\.\\d+)?),\\s\\s(?<z>-?\\d+(?:\\.\\d+)?)\\]");
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();
        
        PrepareGestureOptions();
        goalGesture = pickFromDictionary();
        StartCoroutine(DelayBeforeMethod(2.0f, nextTurn));


    }

    public delegate void myDelegate();
    public IEnumerator DelayBeforeMethod(float wait, myDelegate method)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(wait);

        // Call nextTurn() after the 2-second delay
        method();
    }

    void Update()
    {
        if (!connected)
        {
            connected = true;
            compareGestureScript.m_StillnessEvent.AddListener(handlePlayerConfirmedGesture); //wait for player to "lock in" his gesture
        }
    }

    public void nextTurn()
    {
        //pick a gesture from the dictionary
        compareGestureScript.recording = true;
        //Player solves, A.I demonstrates
        if (currentPlayer == 0)
        {
            UpdateText("Next to demonstrate: A.I");
            PlayerUI.SetActive(false);
            //mimic that gesture using the IK script
            estimationToIkScript.usingHololensTcp = false;
            estimationToIkScript.usingPanoptic = false;
            estimationToIkScript.saveRecording(goalGesture);
            estimationToIkScript.usingRecording = true;
            //perhaps should also utilize the stillness mechanic to "lock in" the answer instead of doing it continously like now
        }
        else
        {
            UpdateText("Next to demonstrate: Player");
            estimationToIkScript.usingHololensTcp = false;
            estimationToIkScript.usingPanoptic = false;
            estimationToIkScript.usingRecording = false;
            //show the player what to demonstrate
            displayGoal(); 
        }
    }

    void displayGoal()
    {
        PlayerUI.SetActive(true);
        if(GoalDisplayCharacter != null)
        {
            PanopticToIK goalCharEstimator = GoalDisplayCharacter.GetComponent<PanopticToIK>();
            goalCharEstimator.saveRecording(goalGesture);
            goalCharEstimator.usingRecording = true;
        }
    }

    void handlePlayerConfirmedGesture()
    {
        if (currentPlayer == 1) //player demonstrates
        {
            //Debug.Log("Player locked in his demonstration");
            estimationToIkScript.saveRecording(compareGestureScript.characterGesture); //give the temp memory to the estimation
            estimationToIkScript.usingRecording = true;
            if (compareGestureScript.goalGestureCompleted(compareGestureScript.characterGesture)){
                Success();
            }
        }
    }

    public void Success()
    {
        UpdateText("Puzzle Solved!");
        goalGesture = pickFromDictionary();
        if (currentPlayer == 1)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer = 1;
        }
        StartCoroutine(DelayBeforeMethod(2.0f, nextTurn));
    }

    //private int pickIndex = 0;
    //DictionaryManager gestureDictionaryScript;
    private Dictionary<Gesture, string> myDictionary;
    List<Gesture> gestureList;

    public void PrepareGestureOptions()
    {
        myDictionary = dictionary.GetGestureRegistry();
        if (myDictionary == null)
        {
            Debug.Log("Missing gestureDict");
        }

        gestureList = new List<Gesture>(myDictionary.Keys);
        if (gestureList == null)
        {
            Debug.Log("Missing gestureLst");
        }
    }

    int nrGesturesChosen = 0;
    public Vector3[,] pickFromDictionary()
    {
        int pickIndex = nrGesturesChosen % gestureList.Count;
        nrGesturesChosen++;
        Gesture gesture = gestureList[pickIndex];
        Debug.Log("NEW GESTURE PICKED[" + pickIndex + "]: \n" + gesture.ToString());
        Vector3[,] gestureMatrix = Gesture.GestureToMatrix(gesture);
        Debug.Log("GESTURE MATRIX:\n" + gestureMatrix.ToString());
        return gestureMatrix;

        /*if(pickIndex % 2 == 0)
        {
            pickIndex++;
            return new float[,] { { 5, 0, 0 }, { 5, 0, 0 } }; //example value for testing
        }
        else
        {
            pickIndex++;
            return new float[,] { { 2, 2, 2 }, { 2, 2, 2 } }; //example value for testing
        }*/
    }
    public void UpdateText(string newText)
    {
        // Check if the InfoBox GameObject is not null
        if (InfoBox != null)
        {
            // Get the TextMesh component attached to the InfoBox GameObject
            TextMesh textMeshComponent = InfoBox.GetComponent<TextMesh>();

            // Check if the TextMesh component is not null
            if (textMeshComponent != null)
            {
                // Update the text of the TextMesh component
                textMeshComponent.text = newText;
            }
            else
            {
                Debug.LogError("TextMesh component not found on InfoBox GameObject.");
            }
        }
        else
        {
            Debug.LogError("InfoBox GameObject is not assigned.");
        }


    }
}