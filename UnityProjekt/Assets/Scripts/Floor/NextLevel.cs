using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NextLevel : MonoBehaviour
{
    public List<GameObject> players;

    public myGameManager gameManager;
    void Start()
    {
        players = new List<GameObject>();
        gameManager = GameObject.Find("GameManager").GetComponent<myGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other && other.tag.ToLower() == "player")
        {
            if (players.Find(element => element == other.gameObject) == null)
            {
                players.Add(other.gameObject);
            }
        }

        if (players.Count > gameManager.playerList.Count-1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                players[0].GetComponent<PlayerMove>().gameManager.loadNextLevel();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other && other.tag.ToLower() == "player")
        {
            if (players.Find(element => element == other.gameObject) != null)
            {
                players = (players.FindAll(element => element != other.gameObject));
            }
        }
    }
}
