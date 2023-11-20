using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackManager : MonoBehaviour
{
    public enum feedbackType { POSITIVE, NEGATIVE, DONT_UNDERSTAND, NUMERICAL, INITIATE_AGREEMENT }; //later we could combine mediapipe and hololens
    public feedbackType lastDetectedFeedback;
    public int lastDetectedNumeralFeedback;
    public UnityEvent FeedbackEvent = new UnityEvent();

    Gesture negativeGesture;
    Gesture positiveGesture;

    Animator animator;
    RiggingIK riggingIKScript;

    RecognizeGesture RecognizeGestureScript;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        riggingIKScript = GetComponent<RiggingIK>();
        RecognizeGestureScript = GetComponent<RecognizeGesture>();

        //These events are about noticing what the player is doing in the game.
        TwisterGame.successEvent.AddListener(handleSuccessEvent); //player succeeded
        TwisterGame.successEvent.AddListener(handleIllegalMoveEvent); //player messed up the game (fell over or lifted a locked limb)
        TwisterGame.mistakeEvent.AddListener(handleMistakeEvent); //player understood wrong. Perhaps this could also be triggered with a player performed gesture, like tilting their head asking for feedback
    }

    void handleSuccessEvent()
    {
        //play positive feedback animation
        if (riggingIKScript != null && positiveGesture != null)
        {
            riggingIKScript.playGesture(negativeGesture);
        }
    }

    void handleIllegalMoveEvent()
    {
        //play negative feedback animation
        if (riggingIKScript != null && negativeGesture != null)
        {
            riggingIKScript.playGesture(negativeGesture);
        }
    }

    void handleMistakeEvent()
    {
        //play negative feedback animation
    }

    // Update is called once per frame
    void Update()
    {
        if (RecognizeGestureScript != null)
        {
            Gesture characterGesture = Gesture.MatrixToGesture(RecognizeGestureScript.characterGesture);
            string meaning = LevelManager.dictionary.GetMeaningFromGesture(characterGesture);
            switch (meaning){
                case "POSTIIVE":
                    lastDetectedFeedback = feedbackType.POSITIVE;
                    break;
                case "NEGATIVE":
                    lastDetectedFeedback = feedbackType.NEGATIVE;
                    break;
                case "NUMERICAL":
                    lastDetectedFeedback = feedbackType.NUMERICAL;
                    //lastDetectedNumeralFeedback = int.TryParse(meaning.Split(' ')[1], out int result) ? result : 0; //TODO: The meaning would need to hold numerical information, perhaps we could have meanings such as "NUMERICAL 5"
                    break;
            }
            if(meaning == "NEGATIVE" || meaning == "POSITIVE")
            {
                FeedbackEvent.Invoke();
            }
        }


        //keyboard shortcuts for testing purposes
        if (Input.GetKey("n")) // we could add more shortcuts or perhaps add them in a cleaner manner
        {
            handleIllegalMoveEvent();
            lastDetectedFeedback = feedbackType.NEGATIVE;
            FeedbackEvent.Invoke();
        }else if (Input.GetKey("p"))
        {
            lastDetectedFeedback = feedbackType.POSITIVE;
            FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("1"))
        {
            lastDetectedFeedback = feedbackType.NUMERICAL;
            lastDetectedNumeralFeedback = 0; // this will be used to select element 0 of the pose sequence during demonstration
            FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("2"))
        {
            lastDetectedFeedback = feedbackType.NUMERICAL;
            lastDetectedNumeralFeedback = 1; 
            FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("3")) 
        {
            lastDetectedFeedback = feedbackType.NUMERICAL;
            lastDetectedNumeralFeedback = 2;
            FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("i"))
        {
            lastDetectedFeedback = feedbackType.INITIATE_AGREEMENT;
            FeedbackEvent.Invoke();
        }
    }
}
