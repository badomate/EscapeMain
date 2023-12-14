
namespace Agent.Communication.GestureLibrary.EntryGeneration
{
    
/// <summary> Pose known since the start of the game </summary>
    public class StarterPose : Pose
    {
        public string poseId;
        public StarterPose(PoseID id):base()
        {
            poseId = id.ToString();
        }
    }
}
