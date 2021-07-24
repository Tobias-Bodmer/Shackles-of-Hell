using System;

[Serializable]
public class JointStorageRoot
{
    public JointStorage[] joints;
}
[Serializable]
public class JointStorage
{
    public string name;

    public float[] spring = new float[2];
    public float[] damper = new float[2];
    public float[] minDistance = new float[2];
    public float[] maxDistance = new float[2];
    public float[] tolerance = new float[2];
}