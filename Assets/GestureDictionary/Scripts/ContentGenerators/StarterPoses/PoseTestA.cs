
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators.StarterPoses
{
    public class PoseTestA : StarterPose
    {
        public PoseTestA() : base(PoseID.TEST_A)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.LEFT_WRIST, new Vector3(5, 0, 0)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}