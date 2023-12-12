using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionOfObject : MonoBehaviour
{
    public GameObject ObjectToCopy;
    // Start is called before the first frame update
    void Awake()
    {
        transform.position = ObjectToCopy.transform.position;
    }
}
