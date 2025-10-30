using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAlignment : MonoBehaviour {

    [Header("Links")]
    public GameObject wheelGraphic;
    public WheelCollider wheelCol;

    public bool steerable = false;
    [HideInInspector] public float steeringAngle;

    void LateUpdate()
    {
        if (!wheelCol || !wheelGraphic) return;

        Vector3 pos;
        Quaternion rot;
        wheelCol.GetWorldPose(out pos, out rot);

        wheelGraphic.transform.SetPositionAndRotation(pos, rot);
    }
}
