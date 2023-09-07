using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackManager : MonoBehaviour
{
    public enum feedbackType { Positive, Negative, Dontunderstand }; //later we could combine mediapipe and hololens
    public feedbackType lastDetectedFeedback;
    public UnityEvent m_FeedbackEvent = new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: we need to detect if feedback is being given. If so, fire m_FeedbackEvent and change lastDetectedFeedback.
        //...perhaps we could use CompareGesture or the Gesture dictionary's functions for this or both
    }
}
