using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.ToLower() == "player")
        {
            other.gameObject.GetComponent<PlayerMove>().resetpoint = other.gameObject.transform.position;
        }
    }
}
