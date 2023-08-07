using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithHelper : MonoBehaviour
{

    [Tooltip("Reference to The AI helper")]
    public GameObject Helper;

    private LevelManager LevelManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        if (Helper != null)
        {
            LevelManagerScript = Helper.GetComponent<LevelManager>();
        }
        else
        {
            Debug.Log("I can't find the Helper.");
        }
    }

    private bool keyheld = false;
    // Update is called once per frame
    void Update()
    {
        if (LevelManagerScript)
        {
            if (Input.GetKey("1") && !keyheld)
            {
                Debug.Log("pressed");
                keyheld = true;
                LevelManagerScript.Success();
            }
            else if(!Input.GetKey("1")) {
                keyheld = false;
            }
        }
    }
}
