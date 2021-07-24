using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class LoadSaveJointsCopy : MonoBehaviourPun
{

    private JointHandler jointHandler;
    public List<JointStorage> joints = new List<JointStorage>();
    public GameObject player;
    // public Dropdown dropdown;
    // public InputField nameTxt;
    // public Slider springSlider;
    // public Slider damperSlider;
    // public Slider minDisSlider;
    // public Slider maxDisSlider;
    // public Slider tolerance;

    private string jsonPath;

    private void Awake()
    {
        loadDropdownMenu();
    }

    void Start()
    {
        jointHandler = player.GetComponent<JointHandler>();
    }


    private void loadDropdownMenu()
    {
        jsonPath = Path.Combine(Application.streamingAssetsPath, "Json/jointsPresets.json");

        joints.Clear();

        if (!string.IsNullOrEmpty(File.ReadAllText(jsonPath)))
        {
            string txt = File.ReadAllText(jsonPath);
            joints.AddRange(JsonUtility.FromJson<JointStorageRoot>("{\"joints\":" + txt + "}").joints);
        }
    }

    public void loadConnection(string _name)
    {
        JointStorage newJoint = joints.Find(item => item.name == _name);

        if (newJoint != null)
        {
            jointHandler.setValues((byte)JointParameter.SPRING, newJoint.spring[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.DAMPER, newJoint.damper[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.MINDISTANCE, newJoint.minDistance[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.MAXDISTANCE, newJoint.maxDistance[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.TOLERANCE, newJoint.tolerance[0], player.GetPhotonView().ViewID);
        }
    }
}
