using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureGoLeft : StarterGesture
    {
        public GestureGoLeft(Dictionary<string, Pose> poseRegistry) : base(GestureID.GO_LEFT)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.GO_LEFT1.ToString()],
                poseRegistry[PoseID.GO_LEFT2.ToString()],
                poseRegistry[PoseID.GO_LEFT1.ToString()]
            };

            List<float> frameIntervals = new List<float>() { 0.5f, 0.5f, 0.5f};
            AddPoses(gesturePoses, null , frameIntervals);
        }
    }
}