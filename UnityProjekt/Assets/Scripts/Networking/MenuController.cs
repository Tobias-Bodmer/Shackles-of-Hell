using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using UnityEngine.UI;

public class MenuController : MonoBehaviourPunCallbacks
{
    private string VersionName = "0.0000000001";
    public GameObject connectPanel;
    public InputField createGameInput;
    public InputField joinGameInput;

    
    RoomOptions options = new RoomOptions() { MaxPlayers = 4 };

    private void Awake()
    {
        PhotonNetwork.GameVersion = VersionName;
        PhotonNetwork.NickName = "player " + Random.Range(0,9999);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 40; // 30
        PhotonNetwork.SerializationRate = 40;
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        connectPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        //should fix connection problems
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.ConnectUsingSettings();
        
        // PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("connected");
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(createGameInput.text, options, null);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinOrCreateRoom(joinGameInput.text, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        connectPanel.SetActive(false);
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("joined");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();   
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();   
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

}

