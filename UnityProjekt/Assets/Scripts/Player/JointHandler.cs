using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class JointHandler : MonoBehaviourPun
{
    public bool canConnect = true;
    public SpringJoint springJoint = null;
    public int body = -1;
    private JointStorage storage;
    private bool joinActive = false;

    private float spring = 0;
    private float damper = 0;
    private float minDistance = 0;
    public float maxDistance = 0;


    private void Update()
    {
        if (springJoint != null)
        {
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.minDistance = minDistance;
            springJoint.maxDistance = maxDistance;
        }
    }
    public void setJoint()
    {
        this.photonView.RPC("Rpc_setJoint", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    public void Rpc_setJoint(int photonIDYourself)
    {
        GameObject yourself = PhotonView.Find(photonIDYourself).gameObject;
        JointHandler jointHandler = yourself.GetComponentInChildren<JointHandler>();
        if (jointHandler.joinActive)
        {
            yourself.GetComponent<PlayerMove>().isConnected = false;

            canConnect = true;

            yourself.GetComponentInChildren<generateLightbeam>().connected = false;

            jointHandler.springJoint = null;
            Destroy(yourself.GetComponentInChildren<SpringJoint>());
            jointHandler.joinActive = false;

            yourself.GetComponentInChildren<LineRenderer>().positionCount = 0;
            yourself.GetComponentInChildren<generateLightbeam>().targetTransform = null;
        }
        else
        {
            yourself.GetComponent<PlayerMove>().isConnected = true;

            if (springJoint != null)
            {
                if (springJoint.connectedBody != null)
                {
                    springJoint.connectedBody.gameObject.GetComponent<PlayerMove>().isConnected = false;
                }
            }

            yourself.GetComponentInChildren<LineRenderer>().positionCount = 2;
            jointHandler.springJoint = yourself.AddComponent<SpringJoint>();
            jointHandler.springJoint.anchor = Vector3.zero;
            jointHandler.springJoint.autoConfigureConnectedAnchor = false;

            jointHandler.springJoint.maxDistance = 10;
            jointHandler.joinActive = true;

        }
    }

    public void setValues(byte _key, float _value, int photonID)
    {
        this.photonView.RPC("RpcSetValues", RpcTarget.AllBuffered, _key, _value, photonID, this.gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    public void RpcSetValues(byte _key, float _value, int photonIDTarget, int photonIDYourself)
    {
        GameObject gTarget = PhotonView.Find(photonIDTarget).gameObject;
        Rigidbody _body = gTarget.GetComponentInChildren<Rigidbody>();
        springJoint = PhotonView.Find(photonIDYourself).gameObject.GetComponentInChildren<SpringJoint>();
        if (this.gameObject.GetPhotonView().ViewID == photonIDYourself)
        {
            switch ((JointParameter)_key)
            {
                case JointParameter.SPRING:
                    spring = _value;
                    if (springJoint != null)
                    {
                        springJoint.spring = _value;
                    }
                    break;

                case JointParameter.DAMPER:
                    damper = _value;
                    if (springJoint != null)
                    {
                        springJoint.damper = _value;
                    }
                    break;

                case JointParameter.MINDISTANCE:
                    minDistance = _value;
                    if (springJoint != null)
                    {
                        springJoint.minDistance = _value;
                    }
                    break;

                case JointParameter.MAXDISTANCE:
                    maxDistance = _value;
                    if (springJoint != null)
                    {
                        springJoint.maxDistance = _value;
                    }
                    break;

                case JointParameter.TOLERANCE:
                    if (springJoint != null)
                    {
                        springJoint.tolerance = _value;
                    }
                    break;

                case JointParameter.BODY:

                    body = photonIDTarget;

                    if (springJoint.connectedBody != null)
                    {
                        springJoint.connectedBody.GetComponent<PlayerMove>().isConnected = false;
                        springJoint.connectedBody.GetComponentInChildren<generateLightbeam>().connected = false;
                    }

                    springJoint.connectedBody = _body;

                    _body.GetComponent<PlayerMove>().isConnected = true;


                    springJoint.GetComponent<generateLightbeam>().targetTransform = gTarget.transform;

                    if (GetComponent<LineRenderer>().positionCount <= 0)
                    {
                        GetComponent<LineRenderer>().positionCount = 2;
                    }

                    GetComponent<LineRenderer>().SetPosition(0, springJoint.transform.position);
                    GetComponent<LineRenderer>().SetPosition(1, gTarget.transform.position);

                    GetComponent<generateLightbeam>().connected = true;

                    springJoint.connectedAnchor = Vector3.zero;

                    GetComponent<JointHandler>().canConnect = false;
                    break;

                default:
                    break;
            }
            GetComponent<MyJointsUI>().updateUIElementsSpring();
        }
    }
}
