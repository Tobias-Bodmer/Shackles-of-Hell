using UnityEngine;
using UnityEngine.UI;

public class MyJointsUI : MonoBehaviour
{
    public GameObject springCanvas;
    public SpringJoint mySpringJoint;
    public Text spring;
    public Text damper;
    public Text minDistance;
    public Text maxDistance;

    private void Update() {
        updateUIElementsSpring();
    }


    public void updateUIElementsSpring()
    {
        if (springCanvas.activeInHierarchy)
        {
            spring.text = PlayerPrefs.GetFloat("spring").ToString();
            damper.text = PlayerPrefs.GetFloat("damper").ToString();
            minDistance.text = PlayerPrefs.GetFloat("minDistance").ToString();
            maxDistance.text = PlayerPrefs.GetFloat("maxDistance").ToString();
        }
    }
    public void setSrpingCanvas(bool active)
    {
        springCanvas.SetActive(active);
    }

}