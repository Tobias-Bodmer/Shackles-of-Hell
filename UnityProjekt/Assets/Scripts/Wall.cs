using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public PlayerColor playerColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.ToLower() == "player")
        {
            if (other.gameObject.GetComponentInChildren<PlayerMove>().colorTag != playerColor)
            {
                other.gameObject.transform.position = other.gameObject.GetComponentInChildren<PlayerMove>().resetpoint;
            }
        }
    }
}
