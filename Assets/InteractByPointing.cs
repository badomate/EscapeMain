using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractByPointing : MonoBehaviour
{
    [Tooltip("Reference to The AI helper")]
    public GameObject Helper;

    public Camera playerCamera;

    private LevelManager LevelManagerScript;
    private Socket_toHl2 TCPScript;
    public Material highlightedMaterial;
    public Material regularMaterial;

    private Vector3 fingertipPosition = new Vector3(-2.897f, 1.047f, 0);
    private Vector3 fingertipDirection = new Vector3(100, 0, 0);


    [Tooltip("Show a red ray along where the player is pointing.")]
    public bool Visualize = true;
    public bool moveWithMouse = true;
    public float boneHitThreshold = 2.0f;
    public float selfHitThreshold = 2.0f;
    public FeedbackManager feedbackManager;

    Animator helperAnimator;
    // Start is called before the first frame update
    void Start()
    {
        if (Helper != null)
        {
            LevelManagerScript = Helper.GetComponent<LevelManager>();
            helperAnimator = Helper.GetComponent<Animator>();
            TCPScript = Helper.GetComponent<Socket_toHl2>();
        }
        else
        {
            Debug.Log("I can't find the Helper.");
        }
        feedbackManager.m_FeedbackEvent.AddListener(handleFeedbackEvent); //wait for player to "lock in" his gesture
    }


    private void handleFeedbackEvent()
    {
        if (feedbackManager.lastDetectedFeedback == FeedbackManager.feedbackType.Negative)
        {
            unselectLimb();
        }
    }

    //used to put a sphere Gizmo at the hit impact
    private Vector3 hitPoint;
        
    //for visualization in Scene view
    private void OnDrawGizmos()
    {
        if (Visualize)
        {
            // Draw ray as a line in the Scene view
            Gizmos.color = Color.red;
            Gizmos.DrawRay(fingertipPosition, fingertipDirection);

            if (hitPoint != new Vector3(0,0,0))
            {
                Gizmos.DrawSphere(hitPoint, 0.1f);
            }
        }
    }

    bool hoveringSomething = false;
    public EstimationToIK estimationScript;
    int closestPointIndex;
    bool selfPointCheck(Ray ray)
    {
        closestPointIndex = -1;
        float closestDistance = boneHitThreshold;

        for (int i = 0; i < estimationScript.landmarks.Length; i++)
        {
            Vector3 rayToPoint = estimationScript.landmarks[i] - ray.origin;
            float projection = Vector3.Dot(ray.direction, rayToPoint);

            if (projection < 0)
            {
                //Skip points behind the ray
                continue;
            }

            Vector3 closestPoint = ray.origin + projection * ray.direction;
            float distance = Vector3.Distance(estimationScript.landmarks[i], closestPoint);

            if (distance < selfHitThreshold && distance < closestDistance)
            {
                closestDistance = distance;
                closestPointIndex = i;
            }
        }

        if (closestPointIndex != -1
            && LandmarkIndicesDictionary.mediapipeIndicesToLimbs.Values.Any(list => list.Contains(closestPointIndex)))
        {
            Pose.Landmark currentLandmark = LandmarkIndicesDictionary.mediapipeIndicesToLimbs.FirstOrDefault(x => x.Value.Contains(closestPointIndex)).Key;  //TODO: we are going through the dictionary twice, we should fix that
            if (currentLandmark != landmarkSelected) { //New major landmark is hovered
                landmarkSelected = currentLandmark;
            }
            Debug.Log("Self-pointed at landmark: " + closestPointIndex);

            startTime = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }

    bool helperPointCheck(Ray ray)
    {
        if (hitInfo.collider.gameObject == Helper)
        {
            if (Visualize)
            {
                hitPoint = hitInfo.point; //save this so the Gizmos can use it
            }

            if (helperAnimator != null)
            {
                //Get the bones of the Helper's skeleton
                Transform leftArmBone = helperAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm); //HumanBodyBones is a built-in enum
                Transform rightArmBone = helperAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);

                //Check if the hit point is close to any of the interesting bones
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
                if (hoveredLimb != closestHitLimb)
                {
                    hoveredLimb = closestHitLimb;
                    switch (hoveredLimb.name)
                    {
                        case "mixamorig:LeftArm": //if we had saved the HumanBody enum from earlier, we could be switching on that 
                            landmarkSelected = Pose.Landmark.LEFT_WRIST;
                            break;
                        case "mixamorig:RightArm":
                            landmarkSelected = Pose.Landmark.RIGHT_WRIST;
                            break;
                        default:
                            landmarkSelected = Pose.Landmark.LEFT_WRIST;
                            break;
                    }
                    startTime = Time.time;
                }
                //Debug.Log("Locking in the following limb: " + hoveredLimb + "...");

                //TODO: feedback when a limb is in the process of selection or already seletced
            }
        }
        else //we never hit the Helper's (generous) collision box, so he is not being pointed at
        {
            return false;
        }
        return true;
    }


    private Transform hoveredLimb = null;
    private float startTime; // Stores when hovering the current limb started;
    private Pose PoseBeingBuilt;
    Dictionary<Pose.Landmark, Vector3> LandmarksForPose = new Dictionary<Pose.Landmark, Vector3>();
    public Gesture GestureBeingBuilt = new Gesture();
    RaycastHit hitInfo;

    void unselectLimb()
    {
       hoveredLimb = null; //releases the selection lock on this limb and resets the script to the original selection phase
       hoveringSomething = false;
    }


    Pose.Landmark landmarkSelected;
    // Update is called once per frame
    void Update()
    {
        if (moveWithMouse && playerCamera)
        {
            fingertipPosition = playerCamera.transform.position;
            fingertipDirection = playerCamera.transform.forward*10 ;
        }
        else
        {
            if(estimationScript.landmarks.Length > 20)
            {
                fingertipPosition = estimationScript.landmarks[16];
                fingertipDirection = estimationScript.landmarks[20];
            }
        }

        Ray ray = new Ray(fingertipPosition, fingertipDirection); //mock data for now

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity) && LevelManagerScript.currentPlayer == 1) //Disabled unless it's player's turn to explain, not sure if it has a use otherwise
        {
            
            //Get the Animator component of the Helper
            //Animator helperAnimator = Helper.GetComponent<Animator>();

            if (Time.time - startTime < 3.0f || !hoveringSomething)
            {
                if (!selfPointCheck(ray))
                {
                    if (!helperPointCheck(ray))
                    {
                        hoveredLimb = null;
                        hoveringSomething = false;
                    }
                    else
                    {
                        hoveringSomething = true;
                    }
                }
                else{
                    hoveringSomething = true;
                }
            }
            else { 
                //Once the limb is locked in, this block here runs every frame.

                EstimationToIK estimationScript = Helper.GetComponent<EstimationToIK>();
                //Debug.Log("Limb locked in: " + hoveredLimb.name);
                LandmarksForPose[landmarkSelected] = hitInfo.point;

                PoseBeingBuilt = new Pose(LandmarksForPose);
                if(GestureBeingBuilt._poseSequence.Count < 1)
                {
                    GestureBeingBuilt.AddPose(PoseBeingBuilt);
                }

                if (Input.GetKey("q"))
                {
                    unselectLimb();
                }

            }
        }
        


    }
}
