using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target and Position")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, -4f);
    public float distance = 5f;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 3f;

    [Tooltip("Minimum pitch (look up). -30 = up, 0 = flat, 90 = straight down.")]
    public float pitchMin = -10f;

    [Tooltip("Maximum pitch (look down). Smaller value = less ability to look down.")]
    public float pitchMax = 40f;

    private float yaw = 0f;
    private float pitch = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        target.rotation = Quaternion.Euler(0f, yaw, 0f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset.normalized * distance;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
