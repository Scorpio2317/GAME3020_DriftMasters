using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Rigidbody rb;

    public float maxSpeed = 0.0f; // The maximum speed in km/h for the speedometer

    public float minSpeedArrowAngle;
    public float maxSpeedArrowAngle;

    [Header("UI")]
    public TextMeshProUGUI speedLabel; // The label that displays the speed;
    public RectTransform arrow; // The arrow in the speedometer

    private float speed = 0.0f;
    private void Update()
    {
        speed = rb.linearVelocity.magnitude * 3.6f;

        if (speedLabel != null)
        {
            speedLabel.text = ((int)speed) + " km/h";
        }

        if (arrow != null)
        {
            arrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
        }
    }
}
