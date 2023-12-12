using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonVisualizer : MonoBehaviour
{

    public GameObject[] RightSideLegToArm;
    public GameObject[] LeftSideLegToArm;
    public GameObject[] HipsLower;
    public GameObject[] HipsUpper;
    public GameObject[] Head;

    public GameObject[] LeftIndex;
    public GameObject[] LeftMiddle;
    public GameObject[] LeftThumb;
    public GameObject[] LeftPinky;
    public GameObject[] LeftRing;
    public GameObject[] LeftConnect;

    public GameObject[] RightIndex;
    public GameObject[] RightMiddle;
    public GameObject[] RightThumb;
    public GameObject[] RightPinky;
    public GameObject[] RightRing;
    public GameObject[] RightConnect;



    private LineRenderer[] lineRenderers;
    GameObject[][] allArrays;

    // Start is called before the first frame update
    void Start()
    {
        allArrays = new GameObject[][]{ RightSideLegToArm, LeftSideLegToArm, HipsLower, HipsUpper,
            LeftIndex,
            LeftMiddle,
            LeftThumb,
            LeftPinky,
            LeftRing,
            LeftConnect,
            RightIndex,
            RightMiddle,
            RightThumb,
            RightPinky,
            RightRing,
            RightConnect,
            Head,
        };


        int totalLines = allArrays.Length;

        lineRenderers = new LineRenderer[totalLines];

        for (int i = 0; i < totalLines; i++)
        {
            lineRenderers[i] = CreateLineRenderer(allArrays[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            UpdateLineRenderer(lineRenderers[i], allArrays[i]);
        }
    }

    LineRenderer CreateLineRenderer(GameObject[] objects)
    {
        GameObject lineObject = new GameObject("LineRenderer");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        //lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = objects.Length;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        return lineRenderer;
    }

    void UpdateLineRenderer(LineRenderer lineRenderer, GameObject[] objects)
    {
        Vector3[] positions = new Vector3[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            positions[i] = objects[i].transform.position;
        }

        lineRenderer.SetPositions(positions);
    }
}
