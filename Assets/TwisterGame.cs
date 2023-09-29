using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwisterGame : MonoBehaviour
{

    public enum TwisterColor { RED, YELLOW, GREEN, BLUE }; //later we could combine mediapipe and hololens
    public enum TwisterLimb { RIGHT_LEG, LEFT_LEG, RIGHT_ARM, LEFT_ARM }; //later we could combine mediapipe and hololens
    public TwisterColor goalTwisterColor;
    public TwisterLimb goalTwisterLimb;


    public GameObject goalColorDisplay;
    public GameObject goalLimbDisplay;


    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;

    public void TwisterSpin()
    {
        goalTwisterColor = (TwisterColor)Random.Range(0, 3);
        goalTwisterLimb = (TwisterLimb)Random.Range(0, 3);

    }

    public void displayGoal()
    {
        goalColorDisplay.SetActive(true);
        goalLimbDisplay.SetActive(true);
        Renderer renderer = goalColorDisplay.GetComponent<Renderer>();

        switch (goalTwisterColor)
        {
            case TwisterColor.GREEN:
                renderer.material = greenMaterial;
                break;
            case TwisterColor.RED:
                renderer.material = redMaterial;
                break;
            case TwisterColor.YELLOW:
                renderer.material = yellowMaterial;
                break;
            case TwisterColor.BLUE:
                renderer.material = blueMaterial;
                break;

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
