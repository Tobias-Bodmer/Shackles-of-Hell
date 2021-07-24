using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin_cross_chris : MonoBehaviour
{
    public Transform target;
    public float degrees = 50f;
    public string axis = "y";
    
    void Update()
    {
        if(axis.Equals( "x"))
        {
            transform.RotateAround(target.position, Vector3.right, Time.deltaTime * degrees);
        }
        else if (axis.Equals("y"))
        {
            transform.RotateAround(target.position, Vector3.up, Time.deltaTime * degrees);
        }
        else if (axis.Equals("z"))
        {
            transform.RotateAround(target.position, Vector3.forward, Time.deltaTime * degrees);
        }
        else
        {
            Debug.Log("wrong axis input");
        }

    }
}
