using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TwisterGame : MonoBehaviour
{
    public RiggingIK riggingIKScript; 

    public enum TwisterColor { RED, BLUE, YELLOW, GREEN }; //color that limb needs to be put on (if its already there, it moust be moved). Theres a corresponding material array.

    public TwisterColor goalTwisterColor;
    public Pose.Landmark goalTwisterLimb;
    public int goalTwisterCircleId;

    public GameObject goalColorDisplay;
    public GameObject goalLimbDisplay;
    public GameObject helperGoalVisualizer;

    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;

    public Material rightLegMaterial;
    public Material leftLegMaterial;
    public Material rightArmMaterial;
    public Material leftArmMaterial;

    public Material goalMaterial;
    //public Material colorlessMaterial;

    //Where would the real-life board be in the virtual world?
    public Vector3 bottomRightCorner; // Specify the center of the bottom right CIRCLE
    public Vector3 topLeftCorner; // Specify the center of the top left CIRCLE
    public float sphereSize = 1.0f; // Size of each sphere
    public float hittingFloorThreshold = 0.2f;

    static int COLUMNS = 4;
    static int ROWS = 6;
    public GameObject[,] twisterCircles = new GameObject[ROWS, COLUMNS];

    public bool LockInCalibration = false;

    public Dictionary<Pose.Landmark, GameObject> locksPlayer0 = new Dictionary<Pose.Landmark, GameObject>();
    public Dictionary<Pose.Landmark, GameObject> locksPlayer1 = new Dictionary<Pose.Landmark, GameObject>();

    int lastSpinnedPlayer = -1;
    Material[] colors;

    //variables related to recognizing if the player picked a circle.
    float timer = 0f;
    bool waitingForCirclePick = false;
    GameObject currentClosestCircle;

    public enum CircleMode { COLORS, COLORLESS }; //color that limb needs to be put on (if its already there, it moust be moved)
    public CircleMode currentCircleMode;

    public static UnityEvent successEvent = new UnityEvent(); //player is playing the game right, and already went for the correct goal
    public static UnityEvent mistakeEvent = new UnityEvent(); //player is playing the game right, but not going for the correct goal
    public static UnityEvent illegalMoveEvent = new UnityEvent(); //player is doing something you're not allowed to, like touching the mat with his elbows or lifting his locked limbs

    public Vector3 HelperMatCenter = new Vector3(0,0,0);

    public void TwisterSpin(int player) //TODO: Could we add an actual spinner?
    {
        goalTwisterColor = (TwisterColor)Random.Range(0, 3);
        goalTwisterLimb = (Pose.Landmark)Random.Range(1, 3); //TODO: This should be between 0-3, but the left hand is missing from the rig. For now, it's used exclusively for pointing instead.
        goalTwisterCircleId = Random.Range(0, ROWS * COLUMNS -1);
        lastSpinnedPlayer = player;
        waitingForCirclePick = true;
    }

    public GameObject getGoalSphere()
    {
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLUMNS; j++)
            {
                CircleInfo circleInfo = twisterCircles[i, j].GetComponent<CircleInfo>();
                if (circleInfo != null && circleInfo.circleId == goalTwisterCircleId)
                {
                    return twisterCircles[i, j];
                }
            }
        }
        return null;
    }

    public void hideGoal()
    {
        //goalLimbDisplay.SetActive(false);
        goalColorDisplay.SetActive(false);
        helperGoalVisualizer.SetActive(false);
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLUMNS; j++)
            {
                twisterCircles[i, j].GetComponent<Renderer>().enabled = false;
            }
        }
    }

    //this assumes the player is demonstrating
    public void displayGoal()
    {
        goalColorDisplay.SetActive(true);
        goalLimbDisplay.SetActive(true);
        Renderer colorRenderer = goalColorDisplay.GetComponent<Renderer>();
        Renderer limbRenderer = goalLimbDisplay.GetComponent<Renderer>();

        switch (goalTwisterColor)
        {
            case TwisterColor.GREEN:
                colorRenderer.material = greenMaterial;
                break;
            case TwisterColor.RED:
                colorRenderer.material = redMaterial;
                break;
            case TwisterColor.YELLOW:
                colorRenderer.material = yellowMaterial;
                break;
            case TwisterColor.BLUE:
                colorRenderer.material = blueMaterial;
                break;
        }

        switch (goalTwisterLimb)
        {
            case Pose.Landmark.LEFT_WRIST:
                limbRenderer.material = leftArmMaterial;
                break;
            case Pose.Landmark.RIGHT_WRIST:
                limbRenderer.material = rightArmMaterial;
                break;
            case Pose.Landmark.LEFT_FOOT:
                limbRenderer.material = leftLegMaterial;
                break;
            case Pose.Landmark.RIGHT_FOOT:
                limbRenderer.material = rightLegMaterial;
                break;
        }

        if (currentCircleMode == CircleMode.COLORLESS)
        {
            goalColorDisplay.SetActive(false);
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    CircleInfo circleInfo = twisterCircles[i, j].GetComponent<CircleInfo>();
                    if (circleInfo != null && circleInfo.circleId == goalTwisterCircleId)
                    {
                        //TODO: we could highlight already locked limb positions and such here, to make sure the player knows where to keep his limbs
                        //twisterCircles[i, j].GetComponent<Renderer>().material = goalMaterial;
                        helperGoalVisualizer.transform.position = twisterCircles[i, j].transform.position + HelperMatCenter; //TODO: Should we mirror the vector or no? If they were theoretically playing on the same MAT, there would be no mirroring.
                        twisterCircles[i, j].GetComponent<Renderer>().enabled = false;
                    }
                    else
                    {
                        // twisterCircles[i, j].GetComponent<Renderer>().material = colorlessMaterial;
                        twisterCircles[i, j].GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
    }

    void CreateTwisterBoard()
    {
        colors = new Material[] { redMaterial, blueMaterial, yellowMaterial, greenMaterial };
        Vector3 boardSize = topLeftCorner - bottomRightCorner;
        float columnSpacing = boardSize.x / (COLUMNS - 1);
        float rowSpacing = boardSize.z / (ROWS - 1);

        for (int i = 0; i < COLUMNS; i++)
        {
            for (int j = 0; j < ROWS; j++)
            {
                Vector3 position = bottomRightCorner + new Vector3(j * rowSpacing, 0, i * columnSpacing); 

                if (j < twisterCircles.GetLength(0) && i < twisterCircles.GetLength(1) && twisterCircles[j, i] != null)
                {
                    MoveSphere(twisterCircles[j, i], position);
                }
                else
                {
                    GameObject sphere = CreateSphere(position, i);
                    twisterCircles[j, i] = sphere;
                }
            }
        }
    }

    void MoveSphere(GameObject sphere, Vector3 position)
    {
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    int circleIdCounter = 0;
    //TODO: We need to be able to move the spheres at runtime to adjust their position to real life
    //Generally, the spheres should be hidden, and shown only for feedback or to help align them with their real-world counterparts
    //It may make more sense to use cylinders as they can be taller
    GameObject CreateSphere(Vector3 position, int colorIndex)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        sphere.GetComponent<Renderer>().material = colors[colorIndex];

        Collider sphereCollider = sphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }

        CircleInfo CircleInfo = sphere.AddComponent<CircleInfo>();
        CircleInfo.color = (TwisterColor)colorIndex;
        CircleInfo.circleId = circleIdCounter;

        circleIdCounter++;

        return sphere;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateTwisterBoard();

    }

   //TODO: If this was successful, the A.I could relay this with feedback, if it was unsuccessful, the A.I could relay that.
   void circlePickCheck()
    {
        Vector3 offset = new Vector3(0,0,0);
        if (lastSpinnedPlayer == 1)//A.I is the one trying to put their limb on the correct spot
        {
            offset = HelperMatCenter;
        }

        if (waitingForCirclePick)
        {
            GameObject newClosestCircle = findClosestCircle(goalTwisterLimb, offset);

            if (newClosestCircle != null && newClosestCircle != currentClosestCircle) //do we need to handle if it is null?
            {
                currentClosestCircle = newClosestCircle;
                timer = 0f;
            }
            else if (newClosestCircle == currentClosestCircle && currentClosestCircle != null)
            {
                CircleInfo circleInfo = currentClosestCircle.GetComponent<CircleInfo>();
                if (circleInfo != null) //TODO: A.I should react if the result is null for too long! Repeat ourselves
                {
                    timer += Time.deltaTime;
                    if (timer >= 1f)
                    {
                        if((currentCircleMode == CircleMode.COLORS && circleInfo.color == goalTwisterColor) || (currentCircleMode == CircleMode.COLORLESS && circleInfo.circleId == goalTwisterCircleId)) //is it a CORRECT circle
                        {
                            //TODO: invoke positvie feedback by A.I

                            //from now on, current player should keep their limb in this specific spot
                            if (lastSpinnedPlayer == 0)
                            {
                                locksPlayer0.Add(goalTwisterLimb, newClosestCircle);
                                successEvent.Invoke();
                            }
                            else
                            {
                                locksPlayer1.Add(goalTwisterLimb, newClosestCircle);
                                successEvent.Invoke();
                            }
                            waitingForCirclePick = false;
                        }
                        else //a circle was selected by the correct limb, but the circle's color is wrong
                        {
                            timer = 0f;
                            mistakeEvent.Invoke();
                            //TODO: invoke negative feedback from the A.I, since the player is on a wrong circle. (This means the human mistook the word for color. Can we do this somehow if he mistook the word for limb?)
                        }
                    }
                }
            }
        }
    }

    void illegalMoveCheck(Dictionary<Pose.Landmark, GameObject> DictionaryToCheck )
    {
        foreach (KeyValuePair<Pose.Landmark, GameObject> lockEntry in DictionaryToCheck)
        {
            Vector3 landmark = CameraStream.playerPose._landmarkArrangement[lockEntry.Key];
            if (Vector3.Distance(landmark, lockEntry.Value.transform.position) > sphereSize)
            {
                illegalMoveEvent.Invoke();
                //TODO: Illegal move detected! Listen to the event and send negative feedback from the A.I (or the game itself?)
            }
        }
    }

    void hittingFloorCheck()
    {
        Pose.Landmark[] forbiddenLandmarks= new Pose.Landmark[] { Pose.Landmark.RIGHT_KNEE };
        for (int i = 0; i < forbiddenLandmarks.Length; i++)
        {
            if (CameraStream.playerPose._landmarkArrangement.ContainsKey(forbiddenLandmarks[i]))
            {
                if (Mathf.Abs(topLeftCorner.y - CameraStream.playerPose._landmarkArrangement[forbiddenLandmarks[i]].y) < hittingFloorThreshold)
                {
                    illegalMoveEvent.Invoke(); //could be a seperate "fall" event if we wanted a different feedback, but it is a similar issue
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!LockInCalibration)
        {
            CreateTwisterBoard();
        }
        if (Input.GetKey("l"))
        {
            LockInCalibration = true;
        }
        //TODO: adding more keys for calibration could be useful if we plan to play out of editor

        illegalMoveCheck(locksPlayer0);
        //illegalMoveCheck(locksPlayer1); //TODO: Currenlty not implemented for A.I. Should he even be able to make illegal moves??
        hittingFloorCheck(); //Once again, should the A.I even be able to hit the floor / make illegal moves?
        circlePickCheck();
    }

    GameObject findClosestCircle(Pose.Landmark landmarkToCheck, Vector3 offset)
    {
        float closestDistance = sphereSize;
        int closestCircleRow = -1;
        int closestCircleColumn = -1;
        Vector3 limbPosition = new Vector3(0, 0, 0);

        if (lastSpinnedPlayer == 0) //get coordinates for player's limb
        {
            if (CameraStream.playerPose._landmarkArrangement.ContainsKey(landmarkToCheck))
            {
                limbPosition = CameraStream.playerPose._landmarkArrangement[landmarkToCheck];
            }
        }
        else //get coordinates for A.I's limb
        {
            limbPosition = riggingIKScript.landmarkToTarget[landmarkToCheck].transform.position;
        }

        for (int x = 0; x < twisterCircles.GetLength(0); x++)
        {
            for (int y = 0; y < twisterCircles.GetLength(1); y++)
            {
                float distance = Vector3.Distance(limbPosition, twisterCircles[x, y].transform.position + offset);

                if (distance < sphereSize && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCircleRow = x;
                    closestCircleColumn = y;
                }

            }
        }

        if(closestCircleRow != -1)
        {
            return twisterCircles[closestCircleRow, closestCircleColumn];
        }
        else
        {
            return null;
        }
    }
}
