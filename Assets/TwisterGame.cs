using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwisterGame : MonoBehaviour
{

    public enum TwisterColor { RED, YELLOW, GREEN, BLUE }; //color that limb needs to be put on (if its already there, it moust be moved)
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

    GameObject[,] visaulizers = new GameObject[ROWS,COLUMNS];

    public bool LockInCalibration = false;

    public void TwisterSpin() //TODO: Could we add an actual spinner?
    {
        goalTwisterColor = (TwisterColor)Random.Range(0, 3);
        goalTwisterLimb = (Pose.Landmark)Random.Range(0, 3);

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
        Material[] colors = new Material[] { redMaterial, blueMaterial, yellowMaterial, greenMaterial };
        Vector3 boardSize = topLeftCorner - bottomRightCorner;
        float columnSpacing = boardSize.x / (COLUMNS - 1);
        float rowSpacing = boardSize.z / (ROWS - 1);

        for (int i = 0; i < COLUMNS; i++)
        {
            for (int j = 0; j < ROWS; j++)
            {
                Vector3 position = bottomRightCorner + new Vector3(j * rowSpacing, 0, i * columnSpacing); 
                Quaternion rotation = Quaternion.identity;

                if (j < visaulizers.GetLength(0) && i < visaulizers.GetLength(1) && visaulizers[j, i] != null)
                {
                    MoveSphere(visaulizers[j, i], position, rotation);
                }
                else
                {
                    GameObject sphere = CreateSphere(position, rotation, colors[i]);
                    visaulizers[j, i] = sphere;
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
    GameObject CreateSphere(Vector3 position, Quaternion rotation, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.rotation = rotation;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
        sphere.GetComponent<Renderer>().material = material;

        Collider sphereCollider = sphere.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
        }
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
    }
}
