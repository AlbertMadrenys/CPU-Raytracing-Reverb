using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float m_WalkSpeed = 10f;
    [SerializeField] private float m_Mass = 30f;
    [SerializeField] private float m_JumpForce = 1200f;
    [SerializeField] private LayerMask m_GroundLayerMask;

    [SerializeField] private GameObject m_AbsortionLayout;
    [SerializeField] private AudioRaycaster m_Raycaster;

    private InputManager m_InputManager;
    private CharacterController m_CharacterController;

    private Vector3 m_MovementVelocity = Vector3.zero;
    private bool m_IsGrounded;
    private bool m_TryJump;

    private const float GRAVITY = -9.81f;

    private void Awake()
    {
        m_InputManager = GetComponent<InputManager>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_InputManager.OnJumpInputPerformed.AddListener(Jump);
        m_InputManager.OnDoSomethingInputPerformed.AddListener(ChangeLayout);
    }

    private void FixedUpdate()
    {
        m_IsGrounded = Physics.CheckSphere(transform.position, 0.2f, m_GroundLayerMask);
        float previousVerticalVelocity = m_MovementVelocity.y;

        m_MovementVelocity = transform.right * m_InputManager.HorizontalMovement.x + transform.forward * m_InputManager.HorizontalMovement.y;
        m_MovementVelocity *= m_WalkSpeed;

        if (m_IsGrounded && m_TryJump)
        {
            m_MovementVelocity.y = m_JumpForce / m_Mass;
        }
        else
        {
            m_MovementVelocity.y = previousVerticalVelocity + GRAVITY * m_Mass * Time.deltaTime;
        }
        m_TryJump = false;

        m_CharacterController.Move(m_MovementVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        m_TryJump = true;
    }

    private void ChangeLayout()
    {
        m_AbsortionLayout.SetActive(!m_AbsortionLayout.activeSelf);
        m_Raycaster.ForceNextTrigger(m_AbsortionLayout.activeSelf);
    }
}
