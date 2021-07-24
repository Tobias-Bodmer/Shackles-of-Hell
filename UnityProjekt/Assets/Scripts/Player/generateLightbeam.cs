using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateLightbeam : MonoBehaviour
{
    public GameObject[] players;
    public LineRenderer lineRenderer;
    public float maxDistance = 8;
    private Vector3[] positions;

    public bool connected = false;

    public Transform targetTransform;


    void Update()
    {
        if (connected)
        {
            GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
            GetComponent<LineRenderer>().SetPosition(1, targetTransform.position);
        }
    }



    
}
