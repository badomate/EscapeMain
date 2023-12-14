
using System.Collections.Generic;
using UnityEngine;

namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    public class PoseLeftHandMiddle : StarterPose
    {
        public PoseLeftHandMiddle() : base(PoseID.HAND_LEFT_MIDDLE)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.LEFT_WRIST, new Vector3(2, 3, 4)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}