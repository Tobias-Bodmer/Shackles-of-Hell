using UnityEngine;

public class LoadingLevel : MonoBehaviour
{


    private void Start()
    {
        GameObject.Find("GameManager").GetComponent<myGameManager>().SpawnPlayer();
    }
}