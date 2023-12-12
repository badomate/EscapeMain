using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPoseCombiner : MonoBehaviour
{
    public Pose combinedPlayerPose;

    //Customize the following lists in the inspector view to set which landmarks should come from HL2. If a listed landmark appears in both sources, HL2 will be used instead.
    //For example, legs will come from the mediapipe camera but we might want finger landmarks (when possible) from HL2
    public List<Pose.Landmark> landmarksFromHL2;


    // Method to combine dictionaries based on landmark lists
    public void CombineDictionaries()
    {
        //TODO: Consider making this more dynamic. If mediapipe has finger landmarks available, and they arent visible to the HL2 camera, then fall back to mediapipe info.
        if (Socket_toHl2.hololensPlayerPose != null && CameraStream.playerPose != null 
            && Socket_toHl2.hololensPlayerPose._landmarkArrangement.Count > 0 && CameraStream.playerPose._landmarkArrangement.Count > 0)
        {
            var combinedLandmarksHL2 = Socket_toHl2.hololensPlayerPose._landmarkArrangement.Where(kv => landmarksFromHL2.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var combinedLandmarksCamera = CameraStream.playerPose._landmarkArrangement.Where(kv => !combinedLandmarksHL2.ContainsKey(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var combinedDictionaries = combinedLandmarksHL2.Concat(combinedLandmarksCamera)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            combinedPlayerPose = new Pose(combinedDictionaries);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CombineDictionaries();
    }
}
