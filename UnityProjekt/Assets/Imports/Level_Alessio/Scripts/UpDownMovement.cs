using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 distance = new Vector3(0, 0, 0);
    public float movementSpeed = 1f;

    private Vector3 origPos;

    // Start is called before the first frame update
    void Start()
    {
        origPos = target.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        target.localPosition = Vector3.Lerp(origPos - distance, origPos + distance, Mathf.PingPong(Time.time * movementSpeed, 1.0f));
    }

}

