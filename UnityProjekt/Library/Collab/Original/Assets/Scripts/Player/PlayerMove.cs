using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public GameObject thirdPersonView;
    public GameObject cineCam;
    public Cinemachine.CinemachineFreeLook cinemachineFreeLook;

    public PhotonView playerPhotonView;
    [Header("Lvl")]
    public GameObject nextLvlCanvas;
    public myGameManager gameManager;

    [Header("color")]
    public Color myColor;
    public PlayerColor colorTag;
    public bool colorized = false;
    MeshRenderer material;

    [Header("PHYSX")]
    public float maxSpeed = 20;
    public float jumpForce = 8;
    public int myGravity;
    public bool jumpPressed = false;
    public bool isGrounded = true;
    public LayerMask layerMask;
    public bool singleJump = true;

    private float maxSpeedStorage;

    private Vector3 resetpoint;

    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update

    private void Awake()
    {
        playerPhotonView = GetComponent<PhotonView>();
        if (playerPhotonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            cineCam.SetActive(true);
            thirdPersonView.SetActive(true);
        }
    }

    void Start()
    {
        material = GetComponent<MeshRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<myGameManager>();

        maxSpeedStorage = maxSpeed;
    }
    public void setMyColor()
    {
        if (photonView.IsMine)
        {
            this.photonView.RPC("RPC_SendColor", RpcTarget.AllBuffered, new Vector3(myColor.r, myColor.g, myColor.b));
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (playerPhotonView.IsMine)
        {
            groundCheck();

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (singleJump)
                {
                    singleJump = false;
                }
                else
                {
                    singleJump = true;
                }
            }


            if (singleJump)
            {
                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    jumpPressed = true;
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    jumpPressed = true;
                }
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }

            if (Input.GetMouseButtonDown(1))
            {
                cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
            }
            if (Input.GetMouseButtonUp(1))
            {
                cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
                cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
            }


            //establish line conenction between 2 players
            if (Input.GetMouseButton(0) && GetComponent<JointHandler>().canConnect == true)
            {
                selectConnectionTarget();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetComponent<JointHandler>().setJoint();
            }


            // show next level button
            if (Input.GetKeyDown(KeyCode.Tab))
            {

                if (!nextLvlCanvas.activeInHierarchy)
                {
                    nextLvlCanvas.SetActive(true);
                }
                else
                {
                    nextLvlCanvas.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerPhotonView.IsMine)
        {
            if ((rb.velocity.magnitude + moveDirection.magnitude) < new Vector3(maxSpeed, 0, maxSpeed).magnitude)
            {
                rb.velocity = rb.velocity + transform.TransformDirection(moveDirection);
                transform.rotation = Quaternion.Euler(0, thirdPersonView.transform.eulerAngles.y, 0);
            }

            if (jumpPressed)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                jumpPressed = false;
            }

            if (!jumpPressed && !isGrounded && rb.velocity.y < 0)
            {
                rb.AddForce(Vector3.down * myGravity, ForceMode.Acceleration);
            }


        }
    }

    void selectConnectionTarget()
    {
        if (GetComponent<SpringJoint>() != null)
        {
            var ray = thirdPersonView.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.rigidbody && hit.collider.tag == "Player")
                {
                    if (hit.transform.gameObject.GetPhotonView().ViewID != this.transform.gameObject.GetPhotonView().ViewID)
                    {
                        GetComponent<JointHandler>().setValues((byte)JointParameter.BODY, 0, hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
                    }
                    else
                    {
                        Debug.Log("Cant connect to yourself");
                    }
                }
            }
        }
    }

    private void groundCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, .23f, Vector3.down, out hit, 1.3f, layerMask))
        {
            isGrounded = true;

            bool getReset = false;

            if (hit.collider.gameObject.GetComponent<PlayerSpecificFloor>() != null)
            {
                getReset = false;
                PlayerSpecificFloor specificFloor = hit.collider.gameObject.GetComponent<PlayerSpecificFloor>();
                if (colorTag != specificFloor.playerColor)
                {
                    transform.position = resetpoint;
                    getReset = true;
                }
            }

            resetpoint = transform.position;


            if (hit.collider.gameObject.tag == "movingPlatform" && !getReset)
            {
                this.photonView.RPC("RPC_SendNewParent", RpcTarget.All, hit.collider.gameObject.GetComponent<PhotonView>().ViewID);
            }

            maxSpeed = maxSpeedStorage;
        }
        else
        {
            isGrounded = false;

            if (transform.parent != null)
            {
                this.photonView.RPC("RPC_SendNewParent", RpcTarget.All, -1);
            }

            if (maxSpeed == maxSpeedStorage)
            {
                maxSpeed *= 0.3f;
            }
        }
    }

    public void Reset()
    {
        if (playerPhotonView.IsMine)
        {
            this.transform.position = gameManager.spawnPoint.transform.position;
        }
    }

    [PunRPC]
    void RPC_SendNewParent(int photonIDTarget)
    {
        if (photonIDTarget == -1)
        {
            gameObject.transform.parent.parent = null;
        }
        else
        {
            gameObject.transform.parent.parent = PhotonView.Find(photonIDTarget).gameObject.transform;
        }
    }

    [PunRPC]
    void RPC_SendColor(Vector3 mycolo)
    {
        Color myC = new Color(mycolo.x, mycolo.y, mycolo.z);
        gameObject.GetComponent<MeshRenderer>().material.color = myC;
    }
}
