
using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureNegative : StarterGesture
    {
        public GestureNegative(Dictionary<string, Pose> poseRegistry) : base(GestureID.NEGATIVE)
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