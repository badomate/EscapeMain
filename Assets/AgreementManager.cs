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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void beginAgreement()
    {

    }

    void handleMimicEvent()
    {
        if (agreementInProgress)
        {
            LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(compareGesture.characterGesture), "shortcut", false);
            //TODO: "shortcut" should be the unique identifier of the gesture built so far, as such:
            //LevelManager.dictionary.AddGesture(Gesture.MatrixToGesture(compareGesture.characterGesture), pointerScript.GestureBeingBuilt.id, false); //TODO: "shortcut" should be the unique identifier of LevelManager.goalGesture
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
