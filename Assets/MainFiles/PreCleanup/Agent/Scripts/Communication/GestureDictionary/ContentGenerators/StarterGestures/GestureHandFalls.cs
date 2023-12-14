
using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureHandFalls : StarterGesture
    {
        public GestureHandFalls(Dictionary<string, Pose> poseRegistry) : base(GestureID.HAND_FALLS)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.HAND_LEFT_UP.ToString()],
                poseRegistry[PoseID.HAND_LEFT_MIDDLE.ToString()],
                poseRegistry[PoseID.HAND_LEFT_DOWN.ToString()]
            };

            AddPoses(gesturePoses);
        }
    }
}