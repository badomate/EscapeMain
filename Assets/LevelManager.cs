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
            //mimic that gesture using the IK script
            estimationToIkScript.usingHololensTcp = false;
            estimationToIkScript.usingPanoptic = false;
        }
        else
        {
            //show the player what to demonstrate
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
        return new float[,] { { 4, 5, 6 }, { 4, 5, 6 } }; //example value for testing
    }
}