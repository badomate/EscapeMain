using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwisterGame : MonoBehaviour
{
    public EstimationToIK playerEstimationScript;
    public enum TwisterColor { RED, BLUE, YELLOW, GREEN }; //color that limb needs to be put on (if its already there, it moust be moved)
    //public enum TwisterLimb { RIGHT_LEG, LEFT_LEG, RIGHT_ARM, LEFT_ARM };
    public TwisterColor goalTwisterColor;
    public Pose.Landmark goalTwisterLimb;

    public GameObject goalColorDisplay;
    public GameObject goalLimbDisplay;

    public Material greenMaterial;
    public Material redMaterial;
    public Material yellowMaterial;
    public Material blueMaterial;

    public Material rightLegMaterial;
    public Material leftLegMaterial;
    public Material rightArmMaterial;
    public Material leftArmMaterial;


    //Where would the real-life board be in the virtual world?
    public Vector3 bottomRightCorner; // Specify the center of the bottom right CIRCLE
    public Vector3 topLeftCorner; // Specify the center of the top left CIRCLE

    static int COLUMNS = 4;
    static int ROWS = 6;
    public float sphereSize = 1.0f; // Size of each sphere
    //public float sphereSpread = 1.5f; // Spacing between spheres

    GameObject[,] twisterCircles = new GameObject[ROWS,COLUMNS];

    public bool LockInCalibration = false;

    public Dictionary<Pose.Landmark, GameObject> locksPlayer0 = new Dictionary<Pose.Landmark, GameObject>();
    public Dictionary<Pose.Landmark, GameObject> locksPlayer1 = new Dictionary<Pose.Landmark, GameObject>();

    int lastSpinnedPlayer = -1;
    Material[] colors;

    public void TwisterSpin(int player) //TODO: Could we add an actual spinner?
    {
        goalTwisterColor = (TwisterColor)Random.Range(0, 3);
        goalTwisterLimb = (Pose.Landmark)Random.Range(0, 3);
        lastSpinnedPlayer = player;
        /*
        if (player)
        {
            locksPlayer1
            mediapipeIndicesToLimbs.Add(goalTwisterLimb, );
        }*/
    }

    void lockFulfilledCheck(int player)
    {

    }

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
                Quaternion rotation = Quaternion.identity;

                if (j < twisterCircles.GetLength(0) && i < twisterCircles.GetLength(1) && twisterCircles[j, i] != null)
                {
                    MoveSphere(twisterCircles[j, i], position, rotation);
                }
                else
                {
                    GameObject sphere = CreateSphere(position, rotation, i);
                    twisterCircles[j, i] = sphere;
                }
            }
        }
    }

    void MoveSphere(GameObject sphere, Vector3 position, Quaternion rotation)
    {
        sphere.transform.position = position;
        sphere.transform.rotation = rotation;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    //TODO: We need to be able to move the spheres at runtime to adjust their position to real life
    //Generally, the spheres should be hidden, and shown only for feedback or to help align them with their real-world counterparts
    //It may make more sense to use cylinders as they can be taller
    GameObject CreateSphere(Vector3 position, Quaternion rotation, int colorIndex)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.rotation = rotation;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        sphere.GetComponent<Renderer>().material = colors[colorIndex];

        Collider sphereCollider = sphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }

        ColorInfo colorInfo = sphere.AddComponent<ColorInfo>();
        colorInfo.color = (TwisterColor)colorIndex;

        return sphere;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateTwisterBoard();
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

        //checkHovers(Pose.Landmark.RIGHT_WRIST);
    }

    GameObject findClosestCircle(Pose.Landmark landmarkToCheck)
    {
        float closestDistance = sphereSize;
        int closestCircleRow = -1;
        int closestCircleColumn = -1;

        int landmarkToCheckIndex = LandmarkIndicesDictionary.mediapipeIndices[landmarkToCheck];
        if(landmarkToCheckIndex < playerEstimationScript.landmarks.Length)
        {
            for (int x = 0; x < twisterCircles.GetLength(0); x++)
            {
                for (int y = 0; y < twisterCircles.GetLength(1); y++)
                {
                    float distance = Vector3.Distance(playerEstimationScript.landmarks[landmarkToCheckIndex], twisterCircles[x, y].transform.position);

                    if (distance < sphereSize && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCircleRow = x;
                        closestCircleColumn = y;
                    }

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
