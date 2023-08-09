using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractByPointing : MonoBehaviour
{
    [Tooltip("Reference to The AI helper")]
    public GameObject Helper;

    private LevelManager LevelManagerScript;
    private Socket_toHl2 TCPScript;
    public Material highlightedMaterial;

    private Vector3 fingertipPosition = new Vector3(-2.897f, 1.047f, 0);
    private Vector3 fingertipDirection = new Vector3(100, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        if (Helper != null)
        {
            LevelManagerScript = Helper.GetComponent<LevelManager>();
            TCPScript = Helper.GetComponent<Socket_toHl2>();
        }
        else
        {
            Debug.Log("I can't find the Helper.");
        }
    }

    //should help draw the point ray
    private void OnDrawGizmos()
    {
        // Draw ray as a line in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawRay(fingertipPosition, fingertipDirection);
    }

    // Update is called once per frame
    void Update()
    {

            Ray ray = new Ray(fingertipPosition, fingertipDirection); //mock data for now
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) //might need a mask?
            {
                if (hitInfo.collider.gameObject == Helper) //Issue: this is never true. Maybe Helper is missing collision?
                {
                    Debug.Log("Hit Helper");
                    // Get the Animator component of the Helper character
                    Animator helperAnimator = Helper.GetComponent<Animator>();

                    if (helperAnimator != null)
                    {
                        // Get the bones of the Helper's skeleton
                        Transform leftArmBone = helperAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm); //HumanBodyBones is a built-in enum
                        Transform rightArmBone = helperAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);

                        // Check if the hit point is close to any of the bones
                        float boneHitThreshold = 0.1f;
                        if (Vector3.Distance(hitInfo.point, leftArmBone.position) < boneHitThreshold)
                        {
                            leftArmBone.GetComponent<Renderer>().material = highlightedMaterial;
                        }
                        else if (Vector3.Distance(hitInfo.point, rightArmBone.position) < boneHitThreshold)
                        {
                            rightArmBone.GetComponent<Renderer>().material = highlightedMaterial;
                        }
                    //TODO: if multiple bones are close enough, highlight the closest one

                    }
            }
            }

    }
}
