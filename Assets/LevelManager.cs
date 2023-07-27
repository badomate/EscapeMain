using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public int currentPlayer = 0; //0-player, 1-A.I
    // Start is called before the first frame update

    PanopticToIK estimationToIkScript;
    CompareGesture compareGestureScript;
    void Start()
    {
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();
        //decide which player's turn it is
    }

    public void nextTurn()
    {
        //pick a gesture from the dictionary
        compareGestureScript.recording = true;
        compareGestureScript.goalGesture = pickFromDictionary();
        //Player solves, A.I demonstrates
        if (currentPlayer == 0) 
        {
            //mimic that gesture using the IK script
            //estimationToIkScript.UsingHololensTCP = false;
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
        }
        else
        {
            Debug.Log("Next to solve: Player");

        }
    }

    public float[,] pickFromDictionary()
    {
        return new float[,] { { 4, 5, 6 }, { 4, 5, 6 } }; //example value for testing
    }
}