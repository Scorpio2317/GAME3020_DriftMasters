using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{

    [Range(120, 2500)] public int MaxPowerNM = 1200;
    [Range(2, 5)] public float MaxSteerAngle = 4;
    public driveMode driveMode = driveMode.allWheelDrive;
}

[System.Serializable]
public enum driveMode
{
    frontWheelDrive,
    rearWheelDrive,
    allWheelDrive
}
