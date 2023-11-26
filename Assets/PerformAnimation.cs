using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FeedbackManager;

public class PerformAnimation : MonoBehaviour
{
    RecognizeGesture recognizer;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        RecognizeGesture.RecognitionEvent += handleRecognitionEvent;
        animator = GetComponent<Animator>();
    }

    void handleRecognitionEvent(Actions action)
    {
        switch (action)
        {
            case Actions.GO_FORWARD:
                Debug.Log("Play animation GO_FORWARD now!");
                animator.SetBool("WalkFwd", true);
                break;
            case Actions.GO_RIGHT:
                Debug.Log("Play animation GO_RIGHT now!");
                break;
            case Actions.VICTORY:
                Debug.Log("Victory sign detected");
                animator.SetTrigger("Backflip"); //needs to have the exact name of an animationController trigger (currently using DemoAnimController)
                break;
            default:
                animator.SetBool("WalkFwd", false);
                break;
        }
    }

    bool pressing = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1")) //this usually causes the event to fire multiple times, but that's fine, we want animations to play while the gesture is held
        {
            pressing = true;
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_FORWARD);
        }
        else if (Input.GetKey("2"))
        {
            pressing = true;
            RecognizeGesture.RecognitionEvent.Invoke(Actions.GO_RIGHT);
        }
        else if (Input.GetKey("3"))
        {
            pressing = true;
            RecognizeGesture.RecognitionEvent.Invoke(Actions.VICTORY);
        }
        else if(pressing)
        {
            pressing = false;
            RecognizeGesture.RecognitionEvent.Invoke(Actions.UNRECOGNIZED);
        }
    }

    private void OnDisable()
    {
        RecognizeGesture.RecognitionEvent -= handleRecognitionEvent;
    }
}
