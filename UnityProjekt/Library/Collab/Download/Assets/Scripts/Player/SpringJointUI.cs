using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum JointParameter : byte
{
    SPRING,
    DAMPER,
    MINDISTANCE,
    MAXDISTANCE,
    TOLERANCE,
    BODY
}

public class SpringJointUI : MonoBehaviourPun
{
    private JointHandler jointHandler;
    private List<JointStorage> joints = new List<JointStorage>();
    public GameObject player;
    public Dropdown dropdown;
    public InputField nameTxt;
    public Slider springSlider;
    public Slider damperSlider;
    public Slider minDisSlider;
    public Slider maxDisSlider;
    public Slider tolerance;

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
        dropdown.ClearOptions();
        joints.Clear();

        if (!string.IsNullOrEmpty(File.ReadAllText("./Assets/Json/jointsPresets.json")))
        {
            string txt = File.ReadAllText("./Assets/Json/jointsPresets.json");
            joints.AddRange(JsonUtility.FromJson<JointStorageRoot>("{\"joints\":" + txt + "}").joints);
        }

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options.Add(new Dropdown.OptionData("choose..."));

        if (joints != null && joints != new List<JointStorage>())
        {
            foreach (var item in joints)
            {
                options.Add(new Dropdown.OptionData(item.name));
            }
        }

        dropdown.AddOptions(options);
    }

    public void load()
    {
        if (dropdown.value != 0)
        {
            JointStorage newJoint = joints.Find(item => item.name == dropdown.options[dropdown.value].text);

            jointHandler.setValues((byte)JointParameter.SPRING, newJoint.spring[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.DAMPER, newJoint.damper[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.MINDISTANCE, newJoint.minDistance[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.MAXDISTANCE, newJoint.maxDistance[0], player.GetPhotonView().ViewID);
            jointHandler.setValues((byte)JointParameter.TOLERANCE, newJoint.tolerance[0], player.GetPhotonView().ViewID);

            springSlider.enabled = System.Convert.ToBoolean(newJoint.spring[1]);
            damperSlider.enabled = System.Convert.ToBoolean(newJoint.damper[1]);
            minDisSlider.enabled = System.Convert.ToBoolean(newJoint.minDistance[1]);
            maxDisSlider.enabled = System.Convert.ToBoolean(newJoint.maxDistance[1]);
            tolerance.enabled = System.Convert.ToBoolean(newJoint.tolerance[1]);

            springSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = System.Convert.ToBoolean(newJoint.spring[1]);
            damperSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = System.Convert.ToBoolean(newJoint.damper[1]);
            minDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = System.Convert.ToBoolean(newJoint.minDistance[1]);
            maxDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = System.Convert.ToBoolean(newJoint.maxDistance[1]);
            tolerance.transform.parent.GetComponentInChildren<Toggle>().isOn = System.Convert.ToBoolean(newJoint.tolerance[1]);

            springSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = System.Convert.ToBoolean(newJoint.spring[1]);
            damperSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = System.Convert.ToBoolean(newJoint.damper[1]);
            minDisSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = System.Convert.ToBoolean(newJoint.minDistance[1]);
            maxDisSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = System.Convert.ToBoolean(newJoint.maxDistance[1]);
            tolerance.transform.parent.GetComponentInChildren<Toggle>().enabled = System.Convert.ToBoolean(newJoint.tolerance[1]);

            loadDropdownMenu();
        }
        else
        {
            springSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = true;
            damperSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = true;
            minDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = true;
            maxDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn = true;
            tolerance.transform.parent.GetComponentInChildren<Toggle>().isOn = true;

            springSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = true;
            damperSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = true;
            minDisSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = true;
            maxDisSlider.transform.parent.GetComponentInChildren<Toggle>().enabled = true;
            tolerance.transform.parent.GetComponentInChildren<Toggle>().enabled = true;

        }
    }

    public void save()
    {
        if (nameTxt.text != "")
        {
            nameTxt.image.color = new Color(1, 1, 1, 1);

            JointStorage saveObject = new JointStorage();

            saveObject.name = nameTxt.text;

            saveObject.spring[0] = springSlider.value;
            saveObject.damper[0] = damperSlider.value;
            saveObject.minDistance[0] = minDisSlider.value;
            saveObject.maxDistance[0] = maxDisSlider.value;
            saveObject.tolerance[0] = tolerance.value;

            saveObject.spring[1] = System.Convert.ToSingle(springSlider.transform.parent.GetComponentInChildren<Toggle>().isOn);
            saveObject.damper[1] = System.Convert.ToSingle(damperSlider.transform.parent.GetComponentInChildren<Toggle>().isOn);
            saveObject.minDistance[1] = System.Convert.ToSingle(minDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn);
            saveObject.maxDistance[1] = System.Convert.ToSingle(maxDisSlider.transform.parent.GetComponentInChildren<Toggle>().isOn);
            saveObject.tolerance[1] = System.Convert.ToSingle(tolerance.transform.parent.GetComponentInChildren<Toggle>().isOn);

            joints.Add(saveObject);

            string json = "[";
            foreach (var item in joints)
            {
                json += JsonUtility.ToJson(item);
                json += ",";
            }
            json = json.Substring(0, json.Length - 1) + "]";

            File.WriteAllText("./Assets/Json/jointsPresets.json", json);

            loadDropdownMenu();
        }
        else
        {
            nameTxt.image.color = new Color(1, 0, 0, 1);
            nameTxt.Select();
        }
    }

    public void rodJoint()
    {
        jointHandler.setValues((byte)JointParameter.SPRING, 10000, player.GetPhotonView().ViewID);
        jointHandler.setValues((byte)JointParameter.DAMPER, 10000, player.GetPhotonView().ViewID);
    }

    public void UpdateData(int _key)
    {
        switch (_key)
        {
            case (int)JointParameter.SPRING:
                jointHandler.setValues((byte)JointParameter.SPRING, springSlider.value, player.GetPhotonView().ViewID);
                break;

            case (int)JointParameter.DAMPER:
                jointHandler.setValues((byte)JointParameter.DAMPER, damperSlider.value, player.GetPhotonView().ViewID);
                break;

            case (int)JointParameter.MINDISTANCE:
                jointHandler.setValues((byte)JointParameter.MINDISTANCE, minDisSlider.value, player.GetPhotonView().ViewID);
                break;

            case (int)JointParameter.MAXDISTANCE:
                jointHandler.setValues((byte)JointParameter.MAXDISTANCE, maxDisSlider.value, player.GetPhotonView().ViewID);
                break;

            case (int)JointParameter.TOLERANCE:
                jointHandler.setValues((byte)JointParameter.TOLERANCE, tolerance.value, player.GetPhotonView().ViewID);
                break;

            default:
                break;
        }
    }
}
