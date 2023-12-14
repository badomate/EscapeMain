using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Agent.Movement
{
    public class PanopticConverter : MonoBehaviour
    {

        public GameObject[] keypoints = null;
        public int[] keypointIndices = null;

        [System.Serializable]
        public class BodyData
        {
            public float version;
            public float univTime;
            public string fpsType;
            public Body[] bodies;
        }

        [System.Serializable]
        public class Body
        {
            public int id;
            public float[] joints19;
        }

        private void SetJointAngles(float[] angles)
        {
            float scaleFactor = 0.005f; // Adjust the scaling factor as needed
            if(keypoints.Length == keypointIndices.Length)
            {
                for (int i = 0; i < keypoints.Length; i++)
                {
                    int currentIndex = keypointIndices[i];

                    // Extract the rotation angles from the joints array
                    float neckX = angles[currentIndex * 4] * scaleFactor; // x-axis rotation angle
                    float neckY = angles[currentIndex * 4 + 1] * scaleFactor; // y-axis rotation angle
                    float neckZ = angles[currentIndex * 4 + 2] * scaleFactor; // z-axis rotation angle

                    // If the data is rotations
                    //Quaternion neckRotation = Quaternion.Euler(neckX, neckY, neckZ);
                    //keypoints[i].transform.localRotation = neckRotation;

                    keypoints[i].transform.position = new Vector3(neckX, neckY, neckZ); //Moving one bone affects the rest. This is really bad.

                }
                //keypoints[1].transform.position = new Vector3(1000, 1000, 1000);

            }
            else
            {
                Debug.Log("Keypoints and their corresponding indices don't match!");
            }

            //Debug.Log(neckXAngle);

        }

        private void LoadJSONData(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);

            // Parse the JSON data into a data structure
            BodyData bodyData = JsonUtility.FromJson<BodyData>(jsonString);

            // Extract the joint angles from the body data
            float[] jointAngles = bodyData.bodies[0].joints19;

            // Set the character's joints to the extracted angles
            SetJointAngles(jointAngles);
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadJSONData("assets/body3DScene_00000000.json");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
