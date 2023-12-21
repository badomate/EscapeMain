using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchReactor : MonoBehaviour
{
    Color defaultColor;
    Color highlightColor;

    private void Start() {
        defaultColor = GetComponent<MeshRenderer>().material.color;
        highlightColor = new Color(0.5f, 0.0f, 0.5f);
    }

    private void OnTriggerEnter(Collider other) {
        this.GetComponent<MeshRenderer>().material.color = highlightColor;
    }
    private void OnTriggerExit(Collider other) {
        this.GetComponent<MeshRenderer>().material.color = defaultColor;
    }
}
