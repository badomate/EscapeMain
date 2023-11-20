using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgreementManager : MonoBehaviour
{
    public FeedbackManager feedbackManager;
    public RecognizeGesture recognizeGesture;
    public InteractByPointing pointerScript;
    bool agreementOffered = false;
    bool agreementInProgress = false;
    // Start is called before the first frame update
    void Start()
    {
        feedbackManager.FeedbackEvent.AddListener(handleFeedbackEvent);
        recognizeGesture.MimicEvent.AddListener(handleHelperNewWordEvent);
        RecognizeGesture.StillnessEvent.AddListener(handlePlayerNewWordEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void beginAgreement()
    {
        
    }

    void handlePlayerNewWordEvent()
    {
        if (agreementInProgress)
        {
            //the player suggested a word. We should mimic it or reply positively to show that we agree.
            //LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(recognizeGesture.characterGesture), "shortcut", false);
            //TODO: "shortcut" should be the unique identifier of the gesture built so far, as such:
            //LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(recognizeGesture.characterGesture), pointerScript.GestureBeingBuilt.id, false); //TODO: "shortcut" should be the unique identifier of LevelManager.goalGesture
        }
    }


    void handleHelperNewWordEvent() //for when the player copies a word suggested by the A.I
    {
        if (agreementInProgress)
        {
            //LevelManager.dictionary.AddGesture(suggestedNewWord, levelManager.goalGesture.id, false);
            //TODO: we lack gesture identifiers as well as a way for the Helper to suggest a word
        }
    }

        void handleFeedbackEvent()
    {
        if (!agreementInProgress && feedbackManager.lastDetectedFeedback == FeedbackManager.feedbackType.INITIATE_AGREEMENT)
        {
            agreementOffered = true;
            //TODO: Helper should always accept
        }
        else if(!agreementInProgress && agreementOffered && feedbackManager.lastDetectedFeedback == FeedbackManager.feedbackType.POSITIVE)
        {
            agreementInProgress = true;
        }
    }
}
