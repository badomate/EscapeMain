using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractByPointing : MonoBehaviour
{
    [Tooltip("Reference to The AI helper")]
    public GameObject Helper;

    public Camera playerCamera;

    private LevelManager LevelManagerScript;
    private Socket_toHl2 TCPScript;
    public Material highlightedMaterial;

    private Vector3 fingertipPosition = new Vector3(-2.897f, 1.047f, 0);
    private Vector3 fingertipDirection = new Vector3(100, 0, 0);


    [Tooltip("Show a red ray along where the player is pointing.")]
    public bool Visualize = true;
    public bool moveWithMouse = true;
    public float boneHitThreshold = 1.0f;

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
        if (Visualize)
        {
            // Draw ray as a line in the Scene view
            Gizmos.color = Color.red;
            Gizmos.DrawRay(fingertipPosition, fingertipDirection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveWithMouse && playerCamera)
        {
            fingertipPosition = playerCamera.transform.position;
            fingertipDirection = playerCamera.transform.forward*10 ;
        }

        Ray ray = new Ray(fingertipPosition, fingertipDirection); //mock data for now
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
                    Transform[] bonesToCheck = new Transform[] { leftArmBone, rightArmBone };
                    Transform[] boneHits = new Transform[bonesToCheck.Length];
                    int boneHitCount = 0;

                    for (int i = 0; i < bonesToCheck.Length; i++)
                    {
                        if (Vector3.Distance(hitInfo.point, bonesToCheck[i].position) < boneHitThreshold)
                        {
                            boneHits[boneHitCount] = bonesToCheck[i];
                            boneHitCount++;
                        }
                    }
                    Transform closestHitLimb = boneHits[0];

                    //if we had mulitple hits, check if any were closer to the point of the hit
                    if (boneHitCount > 1)
                    {
                        float closestDistance = Vector3.Distance(boneHits[0].transform.position, hitInfo.point);

                        for (int i = 1; i < boneHitCount; i++)
                        {
                            float distance = Vector3.Distance(boneHits[i].transform.position, hitInfo.point);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestHitLimb = boneHits[i];
                            }
                        }
                    }
                    Debug.Log(closestHitLimb.name);
                    //TODO: process what happens with closestHitLimb
                }
            }
        }

    }
}
