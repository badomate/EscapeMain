using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public GameObject GoalDisplayCharacter;
    public int currentPlayer = 1; //0-player, 1-A.I
    public float[,] goalGesture = new float[2, 3]; //change these according to gesture dictionary

    PanopticToIK estimationToIkScript;
    CompareGesture compareGestureScript;
    public GameObject PlayerUI;
    public GameObject InfoBox;
    private bool connected = false; //used for events

    void Start()
    {
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();

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
        goalGesture = pickFromDictionary();
        compareGestureScript.goalGesture = goalGesture;
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
                StartCoroutine(DelayBeforeMethod(2.0f, Success));
            }
        }
    }

    public void Success()
    {
        UpdateText("Puzzle Solved!");
        if(currentPlayer == 1)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer = 1;
        }
        StartCoroutine(DelayBeforeMethod(2.0f, nextTurn));
    }

    public float[,] pickFromDictionary()
    {
        return new float[,] { { 5, 0, 0 }, { 5, 0, 0 } }; //example value for testing
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