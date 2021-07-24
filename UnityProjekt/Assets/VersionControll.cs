using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionControll : MonoBehaviour
{
   public GameObject versionScreen;
    void Start()
    {
        versionScreen.GetComponent<Text>().text = "Vers. " + Application.version;
    }

  
    
}
