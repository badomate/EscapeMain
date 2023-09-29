using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwisterGame : MonoBehaviour
{

    public enum TwisterColor { RED, YELLOW, GREEN, BLUE }; //color that limb needs to be put on (if its already there, it moust be moved)
    //public enum TwisterLimb { RIGHT_LEG, LEFT_LEG, RIGHT_ARM, LEFT_ARM };
    public TwisterColor goalTwisterColor;
    public Pose.Landmark goalTwisterLimb;


    public GameObject goalColorDisplay;
    public GameObject goalLimbDisplay;


    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;


    public Material rightLegMaterial;
    public Material leftLegMaterial;
    public Material rightArmMaterial;
    public Material leftArmMaterial;

    public void TwisterSpin() //TODO: Could we add an actual spinner?
    {
        goalTwisterColor = (TwisterColor)Random.Range(0, 3);
        goalTwisterLimb = (Pose.Landmark)Random.Range(0, 3);

    }

    public void displayGoal()
    {
        goalColorDisplay.SetActive(true);
        goalLimbDisplay.SetActive(true);
        Renderer colorRenderer = goalColorDisplay.GetComponent<Renderer>();
        Renderer limbRenderer = goalLimbDisplay.GetComponent<Renderer>();

        switch (goalTwisterColor)
        {
            case TwisterColor.GREEN:
                colorRenderer.material = greenMaterial;
                break;
            case TwisterColor.RED:
                colorRenderer.material = redMaterial;
                break;
            case TwisterColor.YELLOW:
                colorRenderer.material = yellowMaterial;
                break;
            case TwisterColor.BLUE:
                colorRenderer.material = blueMaterial;
                break;
        }

        switch (goalTwisterLimb)
        {
            case Pose.Landmark.LEFT_WRIST:
                limbRenderer.material = leftArmMaterial;
                break;
            case Pose.Landmark.RIGHT_WRIST:
                limbRenderer.material = rightArmMaterial;
                break;
            case Pose.Landmark.LEFT_FOOT:
                limbRenderer.material = leftLegMaterial;
                break;
            case Pose.Landmark.RIGHT_FOOT:
                limbRenderer.material = rightLegMaterial;
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
