
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators.StarterPoses
{
    public class PoseGoFurther2 : StarterPose
    {
        public PoseGoFurther2() : base(PoseID.GO_FURTHER2)
        {
            Dictionary<Landmark, Vector3> dictPose =
               new() {
                         {Landmark.RIGHT_WRIST, new Vector3(0,0.5f,0.2f)},
                      {Landmark.RIGHT_SHOULDER, new Vector3(0,0,0)},
                    {Landmark.RIGHT_WRIST_ROOT, new Vector3(0,0,0)},
                         {Landmark.RIGHT_THUMB, new Vector3(0,0.2f,0.1f)},
                          {Landmark.RIGHT_RING, new Vector3(0,0.2f,0.1f)},
                         {Landmark.RIGHT_PINKY, new Vector3(0,0.2f,0.1f)},
                        {Landmark.RIGHT_MIDDLE, new Vector3(0,0.2f,0.1f)},
                         {Landmark.RIGHT_INDEX, new Vector3(0,0.2f,0.1f)}

               };

            _landmarkArrangement = dictPose;
        }
    }
}