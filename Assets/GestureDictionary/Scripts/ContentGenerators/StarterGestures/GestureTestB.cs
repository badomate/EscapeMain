
using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureTestB : StarterGesture
    {
        public GestureTestB(Dictionary<string, Pose> poseRegistry) : base(GestureID.TEST_B)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.TEST_B.ToString()]
            };

            AddPoses(gesturePoses);
        }
    }
}