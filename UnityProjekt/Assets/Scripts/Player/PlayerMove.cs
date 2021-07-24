using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public LineRenderer lineRenderer;
    public GameObject thirdPersonView;
    public GameObject cineCam;

    public GameObject stateMachine;
    public Cinemachine.CinemachineFreeLook cinemachineFreeLook;
    public Cinemachine.CinemachineVirtualCamera cinemachineFirstPerson;

    public Animator cameraAnimatorController;

    public PhotonView playerPhotonView;
    [Header("Lvl")]
    public GameObject nextLvlCanvas;
    public myGameManager gameManager;

    [Header("color")]
    public Color myColor;
    public PlayerColor colorTag;
    public bool colorized = false;
    MeshRenderer material;

    [Header("Connection")]

    public GameObject hub;
    public bool springActive = false;
    public Image springCanvas;
    public bool ropeActive = false;
    public Image ropeCanvas;
    public bool rodActive = false;
    public Image rodCanvas;

    private LoadSaveJointsCopy loadSaveJoints;

    [Header("PHYSX")]
    public float maxSpeed = 20;
    public float jumpForce = 8;
    public int myGravity;
    public bool jumpPressed = false;
    public bool isGrounded = true;
    public LayerMask layerMask;
    public bool singleJump = true;
    public bool isConnected = false;

    private float lastJump = 0;
    private float maxSpeedStorage;

    public Vector3 resetpoint;

    [Header("UISwitches")]
    private bool jointCanvasActive;

    public GameObject controllUI;
    private bool controlSwitch = false;

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
            stateMachine.SetActive(true);
            controllUI.SetActive(false);


            hub.SetActive(true);
            ropeCanvas.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            rodCanvas.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            springCanvas.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        }
    }

    void Start()
    {
        material = GetComponent<MeshRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<myGameManager>();
        jointCanvasActive = false;
        maxSpeedStorage = maxSpeed;
        if (photonView.IsMine)
        {
            cameraAnimatorController.SetBool("firstPerson", false);
        }

        if (playerPhotonView.IsMine)
        {
            loadSaveJoints = transform.parent.GetComponentInChildren<LoadSaveJointsCopy>();

            ropeActive = (loadSaveJoints.joints.Find(item => item.name == SceneManagerHelper.ActiveSceneName + "rope")) != null ? true : false;
            rodActive = (loadSaveJoints.joints.Find(item => item.name == SceneManagerHelper.ActiveSceneName + "rod")) != null ? true : false;
            springActive = (loadSaveJoints.joints.Find(item => item.name == SceneManagerHelper.ActiveSceneName + "spring")) != null ? true : false;

            if (ropeActive)
            {
                ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (rodActive)
            {
                rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (springActive)
            {
                springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
        }
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
            // Cursor.lockState = CursorLockMode.Confined;
            //checks ground
            groundCheck();

            //sets anchor for player
            anchorCheck();

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            //godmode
            if (Input.GetKeyDown(KeyCode.F11))
            {
                singleJump = !singleJump;
            }

            //switch control UI
            if (Input.GetKeyDown(KeyCode.F1))
            {
                controlSwitch = !controlSwitch;
                controllUI.SetActive(controlSwitch);
            }

            //jump
            if (singleJump)
            {
                if (Input.GetButtonDown("Jump") && isGrounded || Input.GetButtonDown("Jump") && lastJump >= 2 && isConnected)
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

            //reset to start
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }

            // activate and load Connection
            if (Input.GetKeyDown(KeyCode.Alpha1) && ropeActive && GetComponent<JointHandler>().springJoint == null)
            {
                jointCanvasActive = true;
                ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);

                if (rodActive)
                {
                    rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }
                if (springActive)
                {
                    springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }

                this.photonView.RPC("RpcSetLineColor", RpcTarget.AllBuffered, new float[] { 0, 1f, 1f }, new float[] { 0, 1f, 1f });

                ropeCanvas.color = new Color(1, 1, 1, 1);

                loadSaveJoints.loadConnection(SceneManagerHelper.ActiveSceneName + "rope");
            }

            //activate and load Connection
            if (Input.GetKeyDown(KeyCode.Alpha2) && rodActive && GetComponent<JointHandler>().springJoint == null)
            {
                jointCanvasActive = true;
                rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);

                if (ropeActive)
                {
                    ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }
                if (springActive)
                {
                    springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }

                this.photonView.RPC("RpcSetLineColor", RpcTarget.AllBuffered, new float[] { 1f, 0, 1f }, new float[] { 1f, 0, 1f });

                rodCanvas.color = new Color(1, 1, 1, 1);

                loadSaveJoints.loadConnection(SceneManagerHelper.ActiveSceneName + "rod");
            }

            //activate and load Connection
            if (Input.GetKeyDown(KeyCode.Alpha3) && springActive && GetComponent<JointHandler>().springJoint == null)
            {
                jointCanvasActive = true;
                springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);

                if (ropeActive)
                {
                    ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }
                if (rodActive)
                {
                    rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                }

                this.photonView.RPC("RpcSetLineColor", RpcTarget.AllBuffered, new float[] { 1f, 0.5f, 0 }, new float[] { 1f, 0.5f, 0 });

                springCanvas.color = new Color(1, 1, 1, 1);

                loadSaveJoints.loadConnection(SceneManagerHelper.ActiveSceneName + "spring");
            }

            //camera switcheroni
            if (Input.GetKeyDown(KeyCode.V))
            {
                bool myBool = cameraAnimatorController.GetBool("firstPerson");
                myBool = !myBool;
                cameraAnimatorController.SetBool("firstPerson", myBool);
            }

            //establish line conenction between 2 players
            if (Input.GetMouseButton(0) && GetComponent<JointHandler>().canConnect == true)
            {
                selectConnectionTarget();
            }

            //breake connection
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (GetComponent<JointHandler>().springJoint != null)
                {
                    if (ropeActive)
                    {
                        ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                    }
                    if (rodActive)
                    {
                        rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                    }
                    if (springActive)
                    {
                        springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
                    }

                    jointCanvasActive = false;
                    GetComponent<JointHandler>().setJoint();
                }
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

            //camera movement
            moveCamera();

            if (Input.GetKeyDown(KeyCode.F12))
            {
                lineRenderer.startColor = new Color(1f, 0.5f, 0);
                lineRenderer.endColor = new Color(1f, 0.5f, 0);
                jointCanvasActive = !jointCanvasActive;
                GetComponent<MyJointsUI>().setSrpingCanvas(jointCanvasActive);
                if (jointCanvasActive == false && GetComponentInChildren<SpringJoint>() != null)
                {
                    GetComponent<JointHandler>().setJoint();
                    GetComponent<JointHandler>().canConnect = true;
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
                lastJump = 0;
            }

            if (!jumpPressed && !isGrounded && rb.velocity.y < 0)
            {
                // rb.AddForce(Vector3.down * myGravity + new Vector3(rb.velocity.x, 0, rb.velocity.z), ForceMode.Acceleration);
                rb.AddForce(Vector3.down * myGravity, ForceMode.Force);
            }
        }
    }

    void moveCamera()
    {
        if (cameraAnimatorController.GetBool("firstPerson") == false)
        {
            if (Input.GetMouseButtonDown(1))
            {
                cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";

            }
            if (Input.GetMouseButtonUp(1))
            {
                cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
                cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
                cinemachineFreeLook.m_XAxis.m_InputAxisValue = 0;
                cinemachineFreeLook.m_YAxis.m_InputAxisValue = 0;
            }
        }
        else
        {
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
        }
    }

    void anchorCheck()
    {
        if (isGrounded && Input.GetButtonDown("Anchor"))
        {
            this.photonView.RPC("RpcSetKinematic", RpcTarget.All, true);
        }

        if (Input.GetButtonUp("Anchor"))
        {
            this.photonView.RPC("RpcSetKinematic", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void RpcSetKinematic(bool value)
    {
        gameObject.GetComponent<PlayerMove>().GetComponent<Rigidbody>().isKinematic = value;
    }

    [PunRPC]
    public void RpcSetLineColor(float[] sColor, float[] eColor)
    {
        gameObject.GetComponent<LineRenderer>().startColor = new Color(sColor[0], sColor[1], sColor[2]);
        gameObject.GetComponent<LineRenderer>().endColor = new Color(eColor[0], eColor[1], eColor[2]);
    }

    void selectConnectionTarget()
    {
        int layerMask = LayerMask.GetMask("Player");
        if (jointCanvasActive)
        {
            var ray = thirdPersonView.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            if (Physics.RaycastAll(ray, Mathf.Infinity, layerMask) != null)
            {
                hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
                if (hits.Length > 0)
                {
                    RaycastHit player = hits[0];
                    Debug.Log((this.transform.position - player.transform.position).magnitude);
                    if (Mathf.Abs((this.transform.position - player.transform.position).magnitude) <= GetComponent<JointHandler>().maxDistance + 1f)
                    {
                        if (player.rigidbody && player.collider.tag == "Player")
                        {
                            if (player.transform.gameObject.GetPhotonView().ViewID != this.transform.gameObject.GetPhotonView().ViewID)
                            {
                                GetComponent<JointHandler>().setJoint();
                                GetComponent<JointHandler>().setValues((byte)JointParameter.BODY, 0, player.transform.gameObject.GetComponent<PhotonView>().ViewID);
                                jointCanvasActive = false;
                            }
                            else
                            {
                                Debug.Log("Cant connect to yourself");
                            }
                        }
                    }
                }
            }
        }
    }


    public float timeSiceNGround;
    private void groundCheck()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, .23f, Vector3.down, out hit, 1.3f, layerMask))
        {
            isGrounded = true;

            lastJump = 0;

            bool getReset = false;

            if (hit.collider.gameObject.GetComponent<PlayerSpecificFloor>() != null)
            {
                getReset = false;
                PlayerSpecificFloor specificFloor = hit.collider.gameObject.GetComponent<PlayerSpecificFloor>();
                if (colorTag != specificFloor.playerColor)
                {
                    Reset();
                    getReset = true;
                }
            }

            if (hit.collider.gameObject.tag == "movingPlatform" && !getReset)
            {
                this.photonView.RPC("RPC_SendNewParent", RpcTarget.All, hit.collider.gameObject.GetComponent<PhotonView>().ViewID);
            }

            maxSpeed = maxSpeedStorage;
            timeSiceNGround = 0;
        }
        else
        {
            if (timeSiceNGround >= 0.1)
            {
                isGrounded = false;

                lastJump += Time.deltaTime;

                if (transform.parent != null)
                {
                    this.photonView.RPC("RPC_SendNewParent", RpcTarget.All, -1);
                }

                if (maxSpeed == maxSpeedStorage)
                {
                    maxSpeed *= 0.8f;
                }
            }
            else
            {
                if (rb.velocity.y <= 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                }
                timeSiceNGround += Time.deltaTime;
            }
        }
    }

    public void Reset()
    {
        if (playerPhotonView.IsMine)
        {
            if (ropeActive)
            {
                ropeCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (rodActive)
            {
                rodCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (springActive)
            {
                springCanvas.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (this.GetComponent<SpringJoint>() != null)
            {
                this.GetComponent<JointHandler>().setJoint();
            }
            if (gameManager.playerList.Find(element => element.GetComponentInChildren<JointHandler>().body == photonView.ViewID) != null)
            {
                if (gameManager.playerList.Find(element => element.GetComponentInChildren<JointHandler>().body == photonView.ViewID).GetComponentInChildren<SpringJoint>() != null)
                {
                    gameManager.playerList.Find(element => element.GetComponentInChildren<JointHandler>().body == photonView.ViewID).GetComponentInChildren<JointHandler>().setJoint();
                }
            }
            this.transform.position = resetpoint;
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
            // gameObject.transform.position = gameObject.transform.parent.transform.position;
        }
    }

    [PunRPC]
    void RPC_SendColor(Vector3 mycolo)
    {
        Color myC = new Color(mycolo.x, mycolo.y, mycolo.z);
        gameObject.GetComponent<MeshRenderer>().material.color = myC;
        gameObject.GetComponent<particelController>().changeStartColor(myC);
    }
}
