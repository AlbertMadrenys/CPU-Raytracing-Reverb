using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputMaster m_InputMaster;
    private Vector2 m_HorizontalMovement;
    private Vector2 m_CameraDeltaLook;
    
    public Vector2 HorizontalMovement
    {
        get => m_HorizontalMovement;
    }

    public Vector2 CameraDeltaLook
    {
        get => m_CameraDeltaLook;
    }

    public UnityEvent OnJumpInputPerformed;

    public UnityEvent OnDoSomethingInputPerformed;

    private void Awake()
    {
        m_InputMaster = new InputMaster();
        m_CameraDeltaLook = new Vector2();
        m_HorizontalMovement = new Vector2();

        m_InputMaster.Movement.HorizontalMovement.performed += OnHorizontalMovementInputPerformed_Internal;
        m_InputMaster.Movement.Jump.performed += OnJumpInputPerformed_Internal;
        m_InputMaster.Movement.DoSomething.performed += OnDoSomethingInputPerformed_Internal;

        m_InputMaster.CameraLook.DeltaX.performed += OnCameraLookXInputPerformed_Internal;
        m_InputMaster.CameraLook.DeltaY.performed += OnCameraLookYInputPerformed_Internal;
        m_InputMaster.CameraLook.DeltaX.canceled += OnCameraLookXInputPerformed_Internal;
        m_InputMaster.CameraLook.DeltaY.canceled += OnCameraLookYInputPerformed_Internal;
    }

    private void OnEnable()
    {
        m_InputMaster.Enable();
    }

    private void OnDisable()
    {
        m_InputMaster.Disable();
    }

    private void OnHorizontalMovementInputPerformed_Internal(InputAction.CallbackContext ctx)
    {
        m_HorizontalMovement = ctx.ReadValue<Vector2>().normalized;
    }

    private void OnJumpInputPerformed_Internal(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) OnJumpInputPerformed?.Invoke();
    }

    private void OnCameraLookXInputPerformed_Internal(InputAction.CallbackContext ctx)
    {
        m_CameraDeltaLook.x = ctx.ReadValue<float>();
    }

    private void OnCameraLookYInputPerformed_Internal(InputAction.CallbackContext ctx)
    {
        m_CameraDeltaLook.y = ctx.ReadValue<float>();
    }

    private void OnDoSomethingInputPerformed_Internal(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) OnDoSomethingInputPerformed?.Invoke();
    }
}
