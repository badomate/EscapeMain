using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureDontUnderstand : StarterGesture
    {
        public GestureDontUnderstand(Dictionary<string, Pose> poseRegistry) : base(GestureID.DONT_UNDERSTAND)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.DONT_UNDERSTAND1.ToString()],
                poseRegistry[PoseID.DONT_UNDERSTAND2.ToString()],
                poseRegistry[PoseID.DONT_UNDERSTAND1.ToString()]
            };

            List<float> frameIntervals = new List<float>() { 0.5f, 0.5f, 0.5f};
            AddPoses(gesturePoses, null , frameIntervals);
        }
    }
}