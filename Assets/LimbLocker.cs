using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbLocker : MonoBehaviour
{

    public float ScaleFactor = 0.1f;
    public float Threshold = 0.5f; //maximum distance between the limb and the lock position
    public GameObject warningCanvas;
    private Pose.Landmark lockedLimb;
    private Vector3 lockedLimbPosition; //where must the locked limb stay
    public GameObject[] lockVisualizationObjects = new GameObject[2];
    public Vector3 playerBasePosition = new Vector3(-4, 0.801f, 0);
    public Vector3 mirrorBasePosition = new Vector3(12.46f, 5, -1.73f);
    public EstimationToIK estimationScript;


    private float warningTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //TODO: it would be good to be able to find out the index in the .landmarks data from knowing the lockedLimb
    // Update is called once per frame
    void Update()
    {
        if (estimationScript)
        {
            if(estimationScript.landmarks.Length > 0)
            {
                if (Vector3.Distance(estimationScript.landmarks[0], lockedLimbPosition) > Threshold) 
                {
                    warningTimer += Time.deltaTime;
                    if (warningTimer >= 1f)
                    {
                        warningCanvas.SetActive(true);
                    }
                }
                else
                {
                    warningCanvas.SetActive(false);
                    warningTimer = 0f;
                }

            }
        }
    }

    public void lockLimb(Gesture goalGesture, Gesture lastGoalGesture)
    {
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
        lockVisualizationObjects[0].SetActive(false);
        lockVisualizationObjects[1].SetActive(false);
    }
}
