using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public int currentPlayer = 0; //0-player, 1-A.I
    public float[,] goalGesture = new float[2, 3]; //change these according to gesture dictionary

    PanopticToIK estimationToIkScript;
    CompareGesture compareGestureScript;
    public GameObject PlayerUI;
    void Start()
    {
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();
        //decide which player's turn it is
        nextTurn();
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
        }
        else
        {
            //show the player what to demonstrate
            PlayerUI.SetActive(true); //TODO: make UI more dynamic, perhaps put a video or an ingame camera on an example character performing the gesture on loop.

            //have A.I copy the saved movements from gesturecompare
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