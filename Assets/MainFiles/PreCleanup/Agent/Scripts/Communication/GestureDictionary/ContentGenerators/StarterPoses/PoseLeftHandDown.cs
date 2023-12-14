
using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators.StarterPoses
{
    public class PoseLeftHandDown : StarterPose
    {
        public PoseLeftHandDown() : base(PoseID.HAND_LEFT_DOWN)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.LEFT_WRIST, new Vector3(2, 0, 4)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}