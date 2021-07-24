using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particelController : MonoBehaviour
{

    public ParticleSystem pSys;

    public void changeStartColor(Color myColor)
    {
        ParticleSystem.MainModule settings = pSys.main;
        settings.startColor = myColor;
    }
}
