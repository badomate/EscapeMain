using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractByPointing : MonoBehaviour
{
    [Tooltip("Reference to The AI helper")]
    public GameObject Helper;

    private LevelManager LevelManagerScript;
    private Socket_toHl2 TCPScript;

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

    // Update is called once per frame
    void Update()
    {

            Ray ray = new Ray(new Vector3(0,1,0), new Vector3(1, 0, 0)); //mock data for now
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity)) //might need a mask?
            {
                if (hitInfo.collider.gameObject == Helper)
                {
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
                            // we are pointing at left arm
                        }
                        else if (Vector3.Distance(hitInfo.point, rightArmBone.position) < boneHitThreshold)
                        {
                            // we are pointing at right arm
                        }

                    }
                }
            }

    }
}
