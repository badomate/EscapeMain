using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureComeCloser : StarterGesture
    {
        public GestureComeCloser(Dictionary<string, Pose> poseRegistry) : base(GestureID.COME_CLOSER)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.COME_CLOSER1.ToString()],
                poseRegistry[PoseID.COME_CLOSER2.ToString()],
                poseRegistry[PoseID.COME_CLOSER1.ToString()]
            };

            List<float> frameIntervals = new List<float>() { 0.5f, 0.5f, 0.5f};
            AddPoses(gesturePoses, null , frameIntervals);
        }
    }
}