using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

public class rotateFollowTarget : MonoBehaviourPun
{
    // Start is called before the first frame update
    public float mouseAmplifier;
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

                transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * 5);
            }

        }
    }

}
