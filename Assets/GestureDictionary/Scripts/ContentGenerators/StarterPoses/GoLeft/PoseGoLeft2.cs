
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators.StarterPoses
{
    public class PoseGoLeft2 : StarterPose
    {
        public PoseGoLeft2() : base(PoseID.GO_LEFT2)
        {
            Dictionary<Landmark, Vector3> dictPose =
               new() {
                    {Landmark.RIGHT_WRIST, new Vector3(0,0.5f,0.5f)},
                    {Landmark.RIGHT_SHOULDER, new Vector3(0,0,0)},
                    {Landmark.RIGHT_WRIST_ROOT, new Vector3(0.15f,0,0)},
                         {Landmark.RIGHT_THUMB, new Vector3(0,0.3f,0.2f)},
                          {Landmark.RIGHT_RING, new Vector3(0,0.3f,0.2f)},
                         {Landmark.RIGHT_PINKY, new Vector3(0,0.3f,0.2f)},
                        {Landmark.RIGHT_MIDDLE, new Vector3(0,0.3f,0.2f)},
                         {Landmark.RIGHT_INDEX, new Vector3(0,0.3f,0.2f)}

               };

            _landmarkArrangement = dictPose;
        }
    }
}