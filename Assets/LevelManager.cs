using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureDictionary;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using static LevelManager;

public class LevelManager : MonoBehaviour
{
    public GameObject Helper;
    public GameObject GoalDisplayCharacter;
    public bool twisterRules = true;
    public int currentPlayer = 1; //0-player, 1-A.I
    private int levelCounter = 0;

    private Gesture lastGoalGesture;//saved for the limb lock mechanic
    public Gesture goalGesture;

    EstimationToIK estimationToIkScript;
    public CompareGesture compareGestureScript;
    public LimbLocker limbLockerScript;
    public InteractByPointing pointerScript;
    public TwisterGame twisterGame;

    public GameObject PlayerUI;
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
        //compareGestureScript = Helper.GetComponent<CompareGesture>();
        estimationToIkScript = Helper.GetComponent<EstimationToIK>();
        //compareGestureScript.StillnessEvent.AddListener(handlePlayerConfirmedGesture); //wait for player to "lock in" his gesture
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
        compareGestureScript.recording = true;

        lastGoalGesture = goalGesture;

        twisterGame.TwisterSpin(currentPlayer);

        //goalGesture = pickFromDictionary();
        /*
        if (currentPlayer == 1) //pick a random limb to lock down
        {
            limbLockerScript.lockLimb(goalGesture, lastGoalGesture);
        }
        else
        {
            limbLockerScript.releaseLockedLimb();
        }*/

        //Player solves, A.I demonstrates
        if (currentPlayer == 0)
        {
            UpdateText("Next to demonstrate: A.I", "TURN "+levelCounter);

            //hide the goaldisplay
            //PlayerUI.SetActive(false);

            /*
            //mimic that gesture using the IK script
            estimationToIkScript.saveRecording(Gesture.GestureToMatrix(goalGesture));
            estimationToIkScript.currentEstimationSource = EstimationToIK.estimationSource.Recording;
            estimationToIkScript.Looping = true;*/
        }
        else
        {
            UpdateText("Next to demonstrate: Player", "TURN " + levelCounter);
            estimationToIkScript.currentEstimationSource = EstimationToIK.estimationSource.None;

            //show the goaldisplay, so the player knows what to demonstrate
            displayGoal(); 
        }
    }

    void displayGoal()
    {
        if(GoalDisplayCharacter != null && !twisterRules)
        {
            PlayerUI.SetActive(true);
            EstimationToIK goalCharEstimator = GoalDisplayCharacter.GetComponent<EstimationToIK>();
            goalCharEstimator.saveRecording(Gesture.GestureToMatrix(goalGesture));
            goalCharEstimator.currentEstimationSource = EstimationToIK.estimationSource.Recording;
        }
        else if (twisterRules)
        {
            twisterGame.displayGoal();
        }
    }

    void handlePlayerConfirmedGesture() //TODO: instead of checking the various ways the puzzle can be solved, perhaps we could just get whatever the Helper is doing and compare that directly
    {
        if (currentPlayer == 1 && goalGesture != null) //player demonstrates
        {
            //Debug.Log("Player locked in his demonstration");
            if (compareGestureScript.goalGestureCompleted(compareGestureScript.characterGesture)){
                estimationToIkScript.saveRecording(compareGestureScript.characterGesture); //give the temp memory to the estimation
                estimationToIkScript.currentEstimationSource = EstimationToIK.estimationSource.Recording;

                Success(); //for now, succeed automatically if we can directly mimic. But realistically, how does A.I know he is to mimic, and not interpret?
            }
            else //interpret the gesture
            {
                if (estimationToIkScript.currentEstimationSource == EstimationToIK.estimationSource.PointedDircetions) //are we following the directions the player is giving us via pointing?
                {
                    if (pointerScript)
                    {
                        Vector3[,] pointerBuiltGestureMatrix = Gesture.GestureToMatrix(pointerScript.GestureBeingBuilt);
                        if (pointerScript.GestureBeingBuilt._poseSequence.Count > 0 &&
                            compareGestureScript.goalGestureCompleted(pointerBuiltGestureMatrix))
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
                    estimationToIkScript.currentEstimationSource = EstimationToIK.estimationSource.PointedDircetions; //could also be set from the pointing script?
                    //Debug.Log("Gesture was interpreted to mean: " + meaning);
                }

                //or interpret as a METAGESTURE and act accordingly. But perhaps metagestures should trigger events instead
            }
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