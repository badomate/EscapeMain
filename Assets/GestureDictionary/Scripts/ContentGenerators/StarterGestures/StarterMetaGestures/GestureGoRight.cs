using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureGoRight : StarterGesture
    {
        public GestureGoRight(Dictionary<string, Pose> poseRegistry) : base(GestureID.GO_RIGHT)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.GO_RIGHT1.ToString()],
                poseRegistry[PoseID.GO_RIGHT2.ToString()],
                poseRegistry[PoseID.GO_RIGHT1.ToString()]
            };

            List<float> frameIntervals = new List<float>() { 0.5f, 0.5f, 0.5f};
            AddPoses(gesturePoses, null , frameIntervals);
        }
    }
}