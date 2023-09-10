using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackManager : MonoBehaviour
{
    public enum feedbackType { Positive, Negative, Dontunderstand }; //later we could combine mediapipe and hololens
    public feedbackType lastDetectedFeedback;
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
            }
            if(meaning == "NEGATIVE" || meaning == "POSITIVE")
            {
                m_FeedbackEvent.Invoke();
            }
        }


        //keyboard shortcuts for testing purposes
        if (Input.GetKey("n"))
        {
            lastDetectedFeedback = feedbackType.Negative;
            m_FeedbackEvent.Invoke();
        }else if (Input.GetKey("p"))
        {
            lastDetectedFeedback = feedbackType.Positive;
            m_FeedbackEvent.Invoke();
        }
    }
}
