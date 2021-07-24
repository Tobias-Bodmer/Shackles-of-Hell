using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public GameObject thirdPersonView;
    public GameObject topDownView;
    public PhotonView playerPhotonView;

    public GameObject joinLength;

    MeshRenderer material;
    [Header("PHYSX")]
    public float deccelSpeed = 0.7f;
    public float maxSpeed = 20;
    public float jumpForce = 8;
    public bool jumpPressed = false;
    public bool isGrounded = true;
    public bool canCastRay;
    public bool canRecieveRay;
    private float maxSpeedStorage;
    private float maxDistance = 8;

    Vector3 moveDirection;

    Rigidbody rb;
    private Vector3 preMousePos = Vector3.zero;

    // Start is called before the first frame update

    private bool switchCamera = true;
    private void Awake()
    {
        playerPhotonView = GetComponent<PhotonView>();
        if (playerPhotonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            thirdPersonView.SetActive(true);
        }
    }
    void Start()
    {
        material = GetComponent<MeshRenderer>();

        FindObjectOfType<generateLightbeam>().updateData();

        maxSpeedStorage = maxSpeed;
        Color myColor = Random.ColorHSV();
        this.photonView.RPC("RPC_SendColor", RpcTarget.All,new Vector3(myColor.r,myColor.g,myColor.b));
    }


    // Update is called once per frame
    void Update()
    {
        if (playerPhotonView.IsMine)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (Input.GetKeyUp(KeyCode.T))
            {
                thirdPersonView.SetActive(!switchCamera);
                topDownView.SetActive(switchCamera);
                switchCamera = !switchCamera;
            }
            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                maxDistance++;
                if (GetComponent<SpringJoint>() != null)
                {
                    GetComponent<SpringJoint>().maxDistance = maxDistance;
                    this.photonView.RPC("RPC_SendDistance", RpcTarget.All, maxDistance);
                    joinLength.GetComponent<Text>().text ="Spring Length: " +  maxDistance.ToString();
                }
                Debug.Log(maxDistance);
            }

            if (Input.GetKeyDown(KeyCode.F2) && maxDistance > 0)
            {
                maxDistance--;
                if (GetComponent<SpringJoint>() != null)
                {
                    GetComponent<SpringJoint>().maxDistance = maxDistance;
                    this.photonView.RPC("RPC_SendDistance", RpcTarget.All, maxDistance);
                    joinLength.GetComponent<Text>().text ="Spring Length: " +  maxDistance.ToString();
                }
                Debug.Log(maxDistance);
            }

            transform.rotation *= Quaternion.Euler(new Vector3(0f, Input.GetAxis("Mouse X") * 5, 0f));
        }
    }

    private void FixedUpdate()
    {
        if (playerPhotonView.IsMine)
        {
            if ((rb.velocity.magnitude + moveDirection.magnitude) < new Vector3(maxSpeed, 0, maxSpeed).magnitude)
            {
                rb.velocity = rb.velocity + transform.TransformDirection(moveDirection);
            }

            if (jumpPressed)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                jumpPressed = false;
                if (maxSpeed == maxSpeedStorage)
                {
                    maxSpeed *= 0.3f;
                }
            }

            if (!jumpPressed && !isGrounded && rb.velocity.y < 0)
            {
                rb.AddForce(Vector3.down * 50, ForceMode.Acceleration);
            }


        }
    }

    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
        maxSpeed = maxSpeedStorage;
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

    public void Reset()
    {
        //sync pls
        if (playerPhotonView.IsMine)
        {
            this.transform.position = new Vector3(0, 2, 0);
        }
    }

    [PunRPC]
    void RPC_SendColor(Vector3 mycolo)
    {
        Color myC = new Color(mycolo.x,mycolo.y,mycolo.z);
        gameObject.GetComponent<MeshRenderer>().material.color = myC;
    }

    [PunRPC]
    void RPC_SendDistance(float _maxDistance)
    {
        gameObject.GetComponent<SpringJoint>().maxDistance = _maxDistance;
    }
}
