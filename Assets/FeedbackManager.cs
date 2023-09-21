using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackManager : MonoBehaviour
{
    public enum feedbackType { Positive, Negative, Dontunderstand, Numerical, InitiateAgreement }; //later we could combine mediapipe and hololens
    public feedbackType lastDetectedFeedback;
    public int lastDetectedNumeralFeedback;
    public UnityEvent m_FeedbackEvent = new UnityEvent();

    CompareGesture compareGestureScript;
    // Start is called before the first frame update
    void Start()
    {
        compareGestureScript = GetComponent<CompareGesture>();

    }

    // Update is called once per frame
    void Update()
    {
        if (compareGestureScript != null)
        {
            Gesture characterGesture = Gesture.MatrixToGesture(compareGestureScript.characterGesture);
            string meaning = LevelManager.dictionary.GetMeaningFromGesture(characterGesture);
            switch (meaning){
                case "POSTIIVE":
                    lastDetectedFeedback = feedbackType.Positive;
                    break;
                case "NEGATIVE":
                    lastDetectedFeedback = feedbackType.Negative;
                    break;
                case "NUMERICAL":
                    lastDetectedFeedback = feedbackType.Numerical;
                    //lastDetectedNumeralFeedback = int.TryParse(meaning.Split(' ')[1], out int result) ? result : 0; //TODO: The meaning would need to hold numerical information, perhaps we could have meanings such as "NUMERICAL 5"
                    break;
            }
            if(meaning == "NEGATIVE" || meaning == "POSITIVE")
            {
                m_FeedbackEvent.Invoke();
            }
        }


        //keyboard shortcuts for testing purposes
        if (Input.GetKey("n")) // we could add more shortcuts or perhaps add them in a cleaner manner
        {
            lastDetectedFeedback = feedbackType.Negative;
            m_FeedbackEvent.Invoke();
        }else if (Input.GetKey("p"))
        {
            lastDetectedFeedback = feedbackType.Positive;
            m_FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("1"))
        {
            lastDetectedFeedback = feedbackType.Numerical;
            lastDetectedNumeralFeedback = 0; // this will be used to select element 0 of the pose sequence during demonstration
            m_FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("2"))
        {
            lastDetectedFeedback = feedbackType.Numerical;
            lastDetectedNumeralFeedback = 1; 
            m_FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("3")) 
        {
            lastDetectedFeedback = feedbackType.Numerical;
            lastDetectedNumeralFeedback = 2;
            m_FeedbackEvent.Invoke();
        }
        else if (Input.GetKey("i"))
        {
            lastDetectedFeedback = feedbackType.InitiateAgreement;
            m_FeedbackEvent.Invoke();
        }
    }
}
