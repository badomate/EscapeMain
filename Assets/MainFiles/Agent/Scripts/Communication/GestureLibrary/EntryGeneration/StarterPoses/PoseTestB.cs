
using System.Collections.Generic;
using UnityEngine;

namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    public class PoseTestB : StarterPose
    {
        public PoseTestB() : base(PoseID.TEST_B)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.LEFT_WRIST, new Vector3(2, 2, 2)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}