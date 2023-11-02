using System.Collections.Generic;

namespace GestureDictionary.ContentGenerators.StarterGestures
{
    public class GestureGoFurther : StarterGesture
    {
        public GestureGoFurther(Dictionary<string, Pose> poseRegistry) : base(GestureID.GO_FURTHER)
        {
            List<Pose> gesturePoses = new List<Pose>() {
                poseRegistry[PoseID.GO_FURTHER1.ToString()],
                poseRegistry[PoseID.GO_FURTHER2.ToString()],
                poseRegistry[PoseID.GO_FURTHER1.ToString()]
            };

            List<float> frameIntervals = new List<float>() { 0.5f, 0.5f, 0.5f};
            AddPoses(gesturePoses, null , frameIntervals);
        }
    }
}