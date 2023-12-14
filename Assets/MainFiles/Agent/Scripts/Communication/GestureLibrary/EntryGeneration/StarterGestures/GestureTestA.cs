
using System.Collections.Generic;

namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    public class GestureTestA : StarterGesture
    {
        public GestureTestA(Dictionary<string, Pose> poseRegistry) : base(GestureID.TEST_A)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.TEST_A.ToString()],
                poseRegistry[PoseID.TEST_A.ToString()]
            };

            AddPoses(gesturePoses);
        }
    }
}