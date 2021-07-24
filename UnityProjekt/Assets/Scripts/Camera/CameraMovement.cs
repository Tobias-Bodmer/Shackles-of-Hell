using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraMovement : MonoBehaviourPun
{

    public Transform target;

    public float mouseAmplifier;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButton(1))
            {
                Vector3 mouseDirection = new Vector3(0, Mathf.Clamp(Input.GetAxis("Mouse Y") * -1, -1, 1), 0);

                float currentY = transform.localPosition.y;

                if (currentY + mouseDirection.y > -4.8f && currentY + mouseDirection.y < 4.8f)
                {
                    transform.Translate(mouseDirection * mouseAmplifier * Time.deltaTime);
                }

                transform.RotateAround(transform.parent.position, Vector3.up, Input.GetAxis("Mouse X") * 5);
            }
            transform.LookAt(target);
        }
    }
}
