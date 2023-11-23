using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FeedbackManager;

public class PerformAnimation : MonoBehaviour
{
    RecognizeGesture recognizer;
    // Start is called before the first frame update
    void Start()
    {
        RecognizeGesture.RecognitionEvent += handleRecognitionEvent;
    }

    void handleRecognitionEvent(string gestureName)
    {
        switch (gestureName)
        {
            case "1":
                Debug.Log("Play animation 1 now!");
                break;
            case "victory":
                Debug.Log("Victory sign detected");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1")) //this usually causes the event to fire multiple times, but that's fine, we want animations to play while the gesture is held
        {
            RecognizeGesture.RecognitionEvent.Invoke("1");
        }
        else if (Input.GetKey("2"))
        {
            RecognizeGesture.RecognitionEvent.Invoke("2");
        }
    }
}
