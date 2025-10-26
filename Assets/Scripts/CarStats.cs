using UnityEngine;

public class CarStats : MonoBehaviour
{

    [Range(120, 2500)] public int MaxPowerNM = 1200;
    [Range(2, 5)] public float MaxSteerAngle = 4;
}
