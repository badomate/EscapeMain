
using System.Collections.Generic;

namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    public class GestureTestB : StarterGesture
    {
        public GestureTestB(Dictionary<string, Pose> poseRegistry) : base(GestureID.TEST_B)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.TEST_B.ToString()],                
                poseRegistry[PoseID.TEST_B.ToString()]
            };

            AddPoses(gesturePoses);
        }
    }
}