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
    private int levelCounter = 0;

    public Gesture goalGesture = null;//change this from gesture dictionary
    private Pose.Landmark lockedLimb;

    PanopticToIK estimationToIkScript;
    CompareGesture compareGestureScript;
    public InteractByPointing pointerScript;
    public GameObject PlayerUI;
    public TextMesh InfoBox;
    public TextMesh LevelInfoBox;
    private bool connected = false; //used for events
    private DictionaryManager dictionary;
    public static Regex _poseRegex;

    void Start()
    {
        Debug.Log(lockedLimb);
        dictionary = new DictionaryManager();
        _poseRegex = new Regex("Position=\\[\\s(?<x>-?\\d+(?:\\.\\d+)?),\\s(?<y>-?\\d+(?:\\.\\d+)?),\\s\\s(?<z>-?\\d+(?:\\.\\d+)?)\\]");
        compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<PanopticToIK>();


        estimationToIkScript.usingHololensTcp = false;
        estimationToIkScript.usingPanoptic = false;

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
        levelCounter++;
        //pick a gesture from the dictionary
        compareGestureScript.recording = true;

        if(levelCounter > 2) //pick a random limb to lock down
        {
            List<Pose.Landmark> landmarkList = goalGesture.relatedLandmarks();
            int randomIndex = new System.Random().Next(landmarkList.Count);
            lockedLimb = landmarkList[randomIndex];
        }

        //Player solves, A.I demonstrates
        if (currentPlayer == 0)
        {
            UpdateText("Next to demonstrate: A.I", "LEVEL "+levelCounter);

            //hide the goaldisplay
            PlayerUI.SetActive(false);

            //mimic that gesture using the IK script
            estimationToIkScript.saveRecording(Gesture.GestureToMatrix(goalGesture));
            estimationToIkScript.usingRecording = true;
        }
        else
        {
            UpdateText("Next to demonstrate: Player", "LEVEL " + levelCounter);
            estimationToIkScript.usingRecording = false;

            //show the goaldisplay, so the player knows what to demonstrate
            displayGoal(); 
        }
    }

    void displayGoal()
    {
        PlayerUI.SetActive(true);
        if(GoalDisplayCharacter != null)
        {
            PanopticToIK goalCharEstimator = GoalDisplayCharacter.GetComponent<PanopticToIK>();
            goalCharEstimator.saveRecording(Gesture.GestureToMatrix(goalGesture));
            goalCharEstimator.usingRecording = true;
        }
    }

    void handlePlayerConfirmedGesture() //TODO: instead of checking the various ways the puzzle can be solved, perhaps we could just get whatever the Helper is doing and compare that directly
    {
        if (currentPlayer == 1) //player demonstrates
        {
            //Debug.Log("Player locked in his demonstration");
            if (compareGestureScript.goalGestureCompleted(compareGestureScript.characterGesture)){
                estimationToIkScript.saveRecording(compareGestureScript.characterGesture); //give the temp memory to the estimation
                estimationToIkScript.usingRecording = true;

                Success(); //for now, succeed automatically if we can directly mimic. But realistically, how does A.I know he is to mimic, and not interpret?
            }
            else //interpret the gesture
            {
                if (estimationToIkScript.usingPointedDircetions) //are we following the directions the player is giving us via pointing?
                {
                    if (pointerScript)
                    {
                        if (pointerScript.GestureBeingBuilt._poseSequence.Count > 0 &&
                            compareGestureScript.goalGestureCompleted(Gesture.GestureToMatrix(pointerScript.GestureBeingBuilt)))
                        {
                            //Puzzle was completed using directions given via pointing
                            Success();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No Pointer script found. Please check script paramters.");
                    }
                }
                else
                {
                    //INTERPRET the gesture as a shortcut for another, or a sequence, or a part of a sequence
                    Gesture characterGesture = Gesture.MatrixToGesture(compareGestureScript.characterGesture);
                    string meaning = dictionary.GetMeaningFromGesture(characterGesture);
                    estimationToIkScript.usingPointedDircetions = true; //could also be set from the pointing script?
                    //Debug.Log("Gesture was interpreted to mean: " + meaning);
                }

                //or interpret as a METAGESTURE and act accordingly. But perhaps metagestures should trigger events instead
            }
        }
    }

    public void Success()
    {
        UpdateText("Puzzle Solved!", "");
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
    public Gesture pickFromDictionary()
    {
        int pickIndex = nrGesturesChosen % gestureList.Count;
        nrGesturesChosen++;

        Gesture gesture = gestureList[pickIndex];
        Debug.Log("NEW GESTURE PICKED[" + pickIndex + "] \n" + gesture.ToString());
        
        return gestureList[pickIndex];
    }

    public void UpdateText(string newText, string newText2)
    {
        //theres 2 seperate objects for showing level/puzzle related information
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