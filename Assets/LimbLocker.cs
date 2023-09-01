using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbLocker : MonoBehaviour
{


    private Pose.Landmark lockedLimb;
    private Vector3 lockedLimbPosition; //where must the locked limb stay
    public GameObject[] lockVisualizationObjects = new GameObject[2];
    public Vector3 playerBasePosition = new Vector3(-4, 0.801f, 0);
    public Vector3 mirrorBasePosition = new Vector3(12.46f, 5, -1.73f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void lockLimb(Gesture goalGesture, Gesture lastGoalGesture)
    {
        List<Pose.Landmark> landmarkList = goalGesture.relatedLandmarks();
        int randomIndex = new System.Random().Next(landmarkList.Count);
        lockedLimb = landmarkList[randomIndex];

        lockedLimbPosition = 0.1f * lastGoalGesture._poseSequence[lastGoalGesture._poseSequence.Count - 1]._poseToMatch._landmarkArrangement[lockedLimb]; //the position of the locked limb in the last pose of the last gesture
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
