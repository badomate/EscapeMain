
using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureHandRises : StarterGesture
    {
        public GestureHandRises(Dictionary<string, Pose> poseRegistry) : base(GestureID.HAND_RISES)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.HAND_LEFT_DOWN.ToString()],
                poseRegistry[PoseID.HAND_LEFT_MIDDLE.ToString()],
                poseRegistry[PoseID.HAND_LEFT_UP.ToString()]
            };

            AddPoses(gesturePoses);
        }
    }
}