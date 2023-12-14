
using System.Collections.Generic;
using UnityEngine;

namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    public class PoseLeftHandUp : StarterPose
    {
        public PoseLeftHandUp() : base(PoseID.HAND_LEFT_UP)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.LEFT_WRIST, new Vector3(2, 5, 4)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}