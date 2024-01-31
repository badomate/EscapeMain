using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 2.0f;
    public float fastRotationMultiplier = 10.0f;
    public Vector3 rotationCenter = Vector3.zero;

    void Start()
    {
        RecognizeGesture.RecognitionEvent += handleRotationRecognitionEvent;
    }

    void handleRotationRecognitionEvent(Actions action)
    {
        /*if(action == Actions.CAMERA_LEFT)
        {
            RotateCameraAround(rotationSpeed * -fastRotationMultiplier);
        }
        else if(action == Actions.CAMERA_RIGHT)
        {
            RotateCameraAround(rotationSpeed * fastRotationMultiplier);
        }*/
    }
        void Update()
    {
        // Check for arrow key presses
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateCameraAround(rotationSpeed * -fastRotationMultiplier);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateCameraAround(rotationSpeed * fastRotationMultiplier);
        }
    }

    void RotateCameraAround(float speed)
    {
        // Calculate the rotation axis based on the rotation center
        Vector3 rotationAxis = transform.TransformDirection(Vector3.up);

        // Rotate the camera around the rotation center
        transform.RotateAround(rotationCenter, rotationAxis, speed * Time.deltaTime);
    }


    private void OnDisable()
    {
        RecognizeGesture.RecognitionEvent -= handleRotationRecognitionEvent;
    }

}
