using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgreementManager : MonoBehaviour
{
    public FeedbackManager feedbackManager;
    public CompareGesture compareGesture;
    public InteractByPointing pointerScript;
    bool agreementOffered = false;
    bool agreementInProgress = false;
    // Start is called before the first frame update
    void Start()
    {
        feedbackManager.m_FeedbackEvent.AddListener(handleFeedbackEvent);
        compareGesture.m_MimicEvent.AddListener(handleHelperNewWordEvent);
        compareGesture.m_StillnessEvent.AddListener(handlePlayerNewWordEvent);
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
            LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(compareGesture.characterGesture), "shortcut", false);
            //TODO: "shortcut" should be the unique identifier of the gesture built so far, as such:
            //LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(compareGesture.characterGesture), pointerScript.GestureBeingBuilt.id, false); //TODO: "shortcut" should be the unique identifier of LevelManager.goalGesture
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
        if (!agreementInProgress && feedbackManager.lastDetectedFeedback == FeedbackManager.feedbackType.InitiateAgreement)
        {
            agreementOffered = true;
            //TODO: Helper should always accept
        }
        else if(!agreementInProgress && agreementOffered && feedbackManager.lastDetectedFeedback == FeedbackManager.feedbackType.Positive)
        {
            agreementInProgress = true;
        }
    }
}
