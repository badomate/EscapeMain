
using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GesturePositive : StarterGesture
    {
        public GesturePositive(Dictionary<string, Pose> poseRegistry) : base(GestureID.POSITIVE)
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