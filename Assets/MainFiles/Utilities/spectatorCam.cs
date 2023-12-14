using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    //Adds keyboard and mouse controls to a camera and dampens (smoothens) both for a cinematic effect. Can be combined with the Recorder to make demonstrational videos.
    public class SpectatorCam : MonoBehaviour
    {
        public float movementSpeed = 3.0f;
        public float rotationSpeed = 3.0f;
        public float damping = 0.2f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private Vector3 velocity = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            //Camera movement
            float translation = Input.GetAxis("Vertical") * movementSpeed;
            float strafe = Input.GetAxis("Horizontal") * movementSpeed;

            Vector3 movement = new Vector3(strafe, 0, translation);
            movement = Vector3.ClampMagnitude(movement, movementSpeed);
            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            transform.position += movement;

            //Camera rotation
            yaw += rotationSpeed * Input.GetAxis("Mouse X");
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            Quaternion targetRotation = Quaternion.Euler(-pitch, yaw, 0.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
        }
    }
}
