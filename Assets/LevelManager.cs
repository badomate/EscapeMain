using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public GameObject GoalDisplayCharacter;
    public bool twisterRules = true;
    public int currentPlayer = 1; //0-player, 1-A.I
    private int levelCounter = 0;

    //private Gesture lastGoalGesture;//saved for the limb lock mechanic
    public Gesture goalGesture;

    public RiggingIK riggingIKScript;
    public RecognizeGesture recognizeGestureScript;
    public InteractByPointing pointerScript;
    public TwisterGame twisterGame;

    public TextMesh InfoBox;
    public TextMesh LevelInfoBox;

    public static DictionaryManager dictionary;
    public static Regex _poseRegex;
    List<Gesture> gestureList;

    int nrGesturesChosen = 0; //index used for picking a goal from the dictionary
    public UnityEvent LevelFinishedEvent = new UnityEvent();

    void Start()
    {
        dictionary = new DictionaryManager();
        _poseRegex = new Regex("Position=\\[\\s(?<x>-?\\d+(?:\\.\\d+)?),\\s(?<y>-?\\d+(?:\\.\\d+)?),\\s\\s(?<z>-?\\d+(?:\\.\\d+)?)\\]");

        RecognizeGesture.StillnessEvent.AddListener(handlePlayerConfirmedGesture); //wait for player to stay still for a bit before processing his gesture
        TwisterGame.successEvent.AddListener(Success);

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


    public void nextTurn()
    {
        levelCounter++;
        //pick a gesture from the dictionary
        recognizeGestureScript.recording = true;

        twisterGame.TwisterSpin(currentPlayer);

        //Player solves, A.I demonstrates
        if (currentPlayer == 0)
        {
            UpdateText("Next to demonstrate: A.I", "TURN "+levelCounter);
            riggingIKScript.SetPointPosition(twisterGame.getGoalSphere().transform.position);

            twisterGame.hideGoal();
        }
        else
        {
            UpdateText("Next to demonstrate: Player", "TURN " + levelCounter);
            riggingIKScript.StopPointing();

            //show the goaldisplay, so the player knows what to demonstrate
            twisterGame.displayGoal(); 
        }
    }

    // The player has been staying still for a while, he might be trying to tell the A.I something
    void handlePlayerConfirmedGesture()
    {
        if(pointerScript != null && pointerScript.GestureBeingBuilt != null && pointerScript.GestureBeingBuilt._poseSequence.Count > 0)
        {
            riggingIKScript.SetIKPositions(pointerScript.GestureBeingBuilt._poseSequence[0]._poseToMatch);
        }
    }

    public void Success()
    {
        //UpdateText("Puzzle Solved!", ""); //we are trying to keep third-party feedback to a minimum, so have the A.I relay this

        if (currentPlayer == 1)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer = 1;
        }
        StartCoroutine(DelayBeforeMethod(2.0f, nextTurn));
        LevelFinishedEvent.Invoke();
    }


    public void PrepareGestureOptions()
    {
        Dictionary<Gesture, string> myDictionary; 
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

    public Gesture pickFromDictionary()
    {
        int pickIndex = nrGesturesChosen % gestureList.Count;
        nrGesturesChosen++;

        Gesture gesture = gestureList[pickIndex];
        //Debug.Log("NEW GESTURE PICKED[" + pickIndex + "] \n" + gesture.ToString());
        
        return gestureList[pickIndex];
    }

    public void UpdateText(string newText, string newText2)
    {
        //there are 2 seperate text objects for showing various level/puzzle related information
        if (InfoBox != null)
        {
            InfoBox.text = newText;
        }
        else
        {
            Debug.LogError("InfoBox is not assigned.");
        }

        if (LevelInfoBox != null)
        {
            LevelInfoBox.text = newText2;
        }
        else
        {
            Debug.LogError("LevelInfoBox is not assigned.");
        }

    }
}