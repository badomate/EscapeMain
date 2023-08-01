using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
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

        StartCoroutine(DelayBeforeNextTurn(2.0f));


    }

    IEnumerator DelayBeforeNextTurn(float wait)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(wait);

        // Call nextTurn() after the 2-second delay
        nextTurn();
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
            PlayerUI.SetActive(true); //TODO: make UI more dynamic, perhaps put a video or an ingame camera on an example character performing the gesture on loop.

            //Debug.Log("Adding listener");
            //compareGestureScript.m_StillnessEvent.AddListener(handlePlayerConfirmedGesture); //wait for player to "lock in" his gesture
            //have A.I copy the saved movements from gesturecompare
        }
    }

    void handlePlayerConfirmedGesture()
    {
        if (currentPlayer == 1) //player demonstrates
        {
            //Debug.Log("Player locked in his demonstration");
            estimationToIkScript.saveRecording(compareGestureScript.characterGesture); //give the temp memory to the estimation
            estimationToIkScript.usingRecording = true;
        }
    }

    public void Success()
    {
        UpdateText("Puzzle Solved!");
        StartCoroutine(DelayBeforeNextTurn(2.0f));
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