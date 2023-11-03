using System.Collections.Generic;
using UnityEngine;

namespace GestureDictionary.ContentGenerators.StarterPoses
{
    public class PoseDontUnderstand1 : StarterPose
    {
        public PoseDontUnderstand1() : base(PoseID.DONT_UNDERSTAND1)
        {
            Dictionary<Landmark, Vector3> dictPose =
                new() {
                    {Landmark.RIGHT_SHOULDER, new Vector3(0,0.55f,0)},
                     {Landmark.LEFT_SHOULDER, new Vector3(0,0.55f,0)},

                       {Landmark.RIGHT_WRIST, new Vector3(0.5f,0.5f,0)},
                  {Landmark.RIGHT_WRIST_ROOT, new Vector3(0.5f,0.5f,0)},
                       {Landmark.RIGHT_THUMB, new Vector3(1,0.5f,-0.1f)},
                       {Landmark.RIGHT_INDEX, new Vector3(1,0.5f,-0.1f)},
                      {Landmark.RIGHT_MIDDLE, new Vector3(1,0.5f,-0.1f)},
                        {Landmark.RIGHT_RING, new Vector3(1,0.5f,-0.1f)},
                       {Landmark.RIGHT_PINKY, new Vector3(1,0.5f,-0.1f)},

                        {Landmark.LEFT_WRIST, new Vector3(-0.5f,0.5f,0)},
                   {Landmark.LEFT_WRIST_ROOT, new Vector3(-0.5f,0.5f,0)},
                        {Landmark.LEFT_THUMB, new Vector3(-1,0.5f,-0.1f)},
                        {Landmark.LEFT_INDEX, new Vector3(-1,0.5f,-0.1f)},
                       {Landmark.LEFT_MIDDLE, new Vector3(-1,0.5f,-0.1f)},
                         {Landmark.LEFT_RING, new Vector3(-1,0.5f,-0.1f)},
                        {Landmark.LEFT_PINKY, new Vector3(-1,0.5f,-0.1f)}
                };

            _landmarkArrangement = dictPose;
        }
    }
}