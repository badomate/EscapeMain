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
    private bool connected = false; //used for events
    void Start()
    {
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();
        //decide which player's turn it is
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
        Debug.Log("Puzzle solved!");
        if (currentPlayer == 0) {
            //handle the puzzle successfully being solved
            Debug.Log("Next to solve: A.I");
            currentPlayer = 1;
        }
        else
        {
            Debug.Log("Next to solve: Player");
            currentPlayer = 0;

        }
        nextTurn();
    }

    public float[,] pickFromDictionary()
    {
        return new float[,] { { 5, 0, 0 }, { 5, 0, 0 } }; //example value for testing
    }
}