using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithHelper : MonoBehaviour
{

    [Tooltip("Reference to The AI helper")]
    public GameObject aiHelper;

    private Animator aiHelperAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (aiHelper != null)
        {
            aiHelperAnimator = aiHelper.GetComponent<Animator>();
            aiHelperAnimator.SetBool("isWalking", true);
        }
        else
        {
            Debug.Log("I can't find the AiHelper.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aiHelperAnimator)
        {
            if (Input.GetKey("1"))
            {
                aiHelperAnimator.SetBool("isWalking", true);


            }
            else { 
                aiHelperAnimator.SetBool("isWalking", false);
            }
        }
    }
}
