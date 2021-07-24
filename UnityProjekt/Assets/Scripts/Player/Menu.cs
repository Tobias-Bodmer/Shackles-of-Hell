using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject toggleYAxisInvertCam;

    private void Update()
    {
        if (gameObject.GetComponent<PlayerMove>().cinemachineFreeLook.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput != System.Convert.ToBoolean(PlayerPrefs.GetInt("invert")))
        {
            gameObject.GetComponent<PlayerMove>().cinemachineFreeLook.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = System.Convert.ToBoolean(PlayerPrefs.GetInt("invert"));
            toggleYAxisInvertCam.GetComponent<Toggle>().isOn = !gameObject.GetComponent<PlayerMove>().cinemachineFreeLook.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput;
        }
    }


    public void loadNextLevel()
    {
        GetComponent<PlayerMove>().gameManager.loadNextLevel();
    }

    public void setYAxisCam()
    {
        bool current = toggleYAxisInvertCam.GetComponent<Toggle>().isOn;
        if (!current)
        {
            PlayerPrefs.SetInt("invert", 1);
        }
        else
        {
            PlayerPrefs.SetInt("invert", 0);
        }

        Debug.Log(PlayerPrefs.GetInt("invert"));

    }
}