using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnButtonController : MonoBehaviour
{

    public GameObject spawnButton;
    public myGameManager gameManager;
    // Start is called before the first frame update

    private void Awake()
    {
        if (spawnButton != null)
        {
            spawnButton.SetActive(true);
        }
    }

    public void OnSpawnClick()
    {
        gameManager.loadFirstLevel();
        spawnButton.SetActive(false);
    }

}
