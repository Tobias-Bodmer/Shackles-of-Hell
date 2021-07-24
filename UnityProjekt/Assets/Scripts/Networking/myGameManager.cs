using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class myGameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject prefab;

    [Header("Players")]
    public List<GameObject> playerList = new List<GameObject>();
    public Color[] playerColors;

    private int levelIndex;

    public GameObject spawnPoint;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void SpawnPlayer()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        GameObject lul = PhotonNetwork.Instantiate(prefab.name, spawnPoint.transform.position + new Vector3(0, 2, 0), Quaternion.identity, 0);
        int id = lul.transform.GetComponentInChildren<PhotonView>().ViewID;
        photonView.RPC("addPlayer", RpcTarget.MasterClient, id);
    }

    [PunRPC]
    private void addPlayer(int playerID)
    {
        if (playerList.Count <= 4)
        {

            GameObject player = PhotonView.Find(playerID).gameObject;
            playerList.Add(player);
        }
        List<int> playerIDs = new List<int>();
        foreach (GameObject player in playerList)
        {
            playerIDs.Add(player.GetPhotonView().ViewID);
        }
        photonView.RPC("setPlayerList", RpcTarget.All, playerIDs.ToArray());

    }

    [PunRPC]
    private void setPlayerList(int[] playerIDs)
    {
        playerList.Clear();
        int index = 0;
        foreach (int ID in playerIDs)
        {
            GameObject player = PhotonView.Find(ID).gameObject;
            if (!player.GetComponentInChildren<PlayerMove>().colorized)
            {
                PlayerMove pMove = player.GetComponentInChildren<PlayerMove>();
                pMove.colorTag = (PlayerColor)Enum.Parse(typeof(PlayerColor), Enum.GetName(typeof(PlayerColor), index));
                pMove.myColor = playerColors[index];
                pMove.setMyColor();
                pMove.colorized = true;
            }
            index++;
            playerList.Add(player);
        }
    }

    public void loadFirstLevel()
    {
        loadLevel(1);
        levelIndex = 1;
    }

    public void loadNextLevel()
    {
        if (levelIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            levelIndex += 1;
        }
        else
        {
            levelIndex = 1;
        }
        playerList.Clear();
        loadLevel(levelIndex);
    }

    public void loadLevel(int index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(index);
        }
    }


}
