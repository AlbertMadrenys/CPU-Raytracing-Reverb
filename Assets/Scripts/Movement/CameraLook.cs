using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class CameraLook : MonoBehaviour
{
    [SerializeField] Transform m_CameraTransform;
    [SerializeField] float m_Sensitivity = 15f;
    [SerializeField] float m_VerticalClampAngle = 85f;

    private InputManager m_InputManager;

    float verticalRotation = 0f;

    private void Awake()
    {
        m_InputManager = GetComponent<InputManager>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        verticalRotation = m_CameraTransform.localEulerAngles.x;
    }

    private void Update()
    {
        Vector2 cameraDeltaLook = m_InputManager.CameraDeltaLook;

        transform.Rotate(Vector3.up * cameraDeltaLook.x * Time.deltaTime * m_Sensitivity);

        verticalRotation -= cameraDeltaLook.y * Time.deltaTime * m_Sensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -m_VerticalClampAngle, m_VerticalClampAngle);

        m_CameraTransform.localRotation = Quaternion.Euler(Vector3.right * verticalRotation);
    }
}
