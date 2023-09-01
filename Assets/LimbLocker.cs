using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbLocker : MonoBehaviour
{

    public Material materialCorrect; //Material to use when the limb is in the correct position
    public Material materialIncorrect; //Material to use when the limb is in an incorrect position

    public float ScaleFactor = 0.1f;
    public float Threshold = 0.5f; //maximum distance between the limb and the lock position
    public GameObject warningCanvas;
    private Pose.Landmark lockedLimb;
    private int lockedLimbIndex = 16; //TODO: it would be good to be able to find out the index in the .landmarks data from knowing the lockedLimb
    private Vector3 lockedLimbPosition; //where must the locked limb stay
    public GameObject[] lockVisualizationObjects = new GameObject[2];
    public Vector3 playerBasePosition = new Vector3(-4, 0.801f, 0);
    public Vector3 mirrorBasePosition = new Vector3(12.46f, 5, -1.73f);
    public EstimationToIK estimationScript;

    private bool locked = false;
    private float warningTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void turnColor(bool correct)
    {
        foreach (GameObject visualizer in lockVisualizationObjects)
        {
            Renderer render = visualizer.GetComponent<Renderer>();
            if (correct)
            {
                render.material = materialCorrect;
            }
            else
            {
                render.material = materialIncorrect;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (estimationScript && locked)
        {
            if(estimationScript.landmarks.Length > lockedLimbIndex)
            {
                if (Vector3.Distance(estimationScript.landmarks[lockedLimbIndex], lockedLimbPosition) > Threshold) 
                {
                    warningTimer += Time.deltaTime;
                    if (warningTimer >= 1f)
                    {
                        warningCanvas.SetActive(true);
                        turnColor(false);
                    }
                }
                else
                {
                    warningCanvas.SetActive(false);
                    turnColor(true);
                    warningTimer = 0f;
                }

            }
        }
    }

    public void lockLimb(Gesture goalGesture, Gesture lastGoalGesture)
    {
        locked = true;
        List<Pose.Landmark> landmarkList = goalGesture.relatedLandmarks();
        int randomIndex = new System.Random().Next(landmarkList.Count);
        lockedLimb = landmarkList[randomIndex];

        lockedLimbPosition = ScaleFactor * lastGoalGesture._poseSequence[lastGoalGesture._poseSequence.Count - 1]._poseToMatch._landmarkArrangement[lockedLimb]; //the position of the locked limb in the last pose of the last gesture
        lockVisualizationObjects[0].transform.position = playerBasePosition + lockedLimbPosition; //+ players base position 
        lockVisualizationObjects[1].transform.position = mirrorBasePosition + Vector3.Scale(lockedLimbPosition,new Vector3(-1,1,1)); //+ mirror's base position (vector scale is for inverting x since the mirror is inverted)
        lockVisualizationObjects[0].SetActive(true);
        lockVisualizationObjects[1].SetActive(true);
    }

    public void releaseLockedLimb()
    {
        locked = false;
        lockVisualizationObjects[0].SetActive(false);
        lockVisualizationObjects[1].SetActive(false);
        warningCanvas.SetActive(false);
    }
}
