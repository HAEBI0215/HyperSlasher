using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("타겟")]
    public Transform followPos;   // Yaw 담당
    public Transform lookPos;     // Pitch 담당

    [Header("감도")]
    public float mouseXSensitivity = 200f;
    public float mouseYSensitivity = 150f;

    [Header("상하 제한")]
    public float minPitch = -35f;
    public float maxPitch = 60f;

    [Header("설정")]
    public bool lockCursor = true;

    private float yaw;
    private float pitch;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        yaw = followPos.eulerAngles.y;

        pitch = lookPos.localEulerAngles.x;
        if (pitch > 180f)
            pitch -= 360f;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseXSensitivity * Time.deltaTime;
        pitch -= mouseY * mouseYSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        followPos.rotation = Quaternion.Euler(0f, yaw, 0f);
        lookPos.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}