using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 Move { get; private set; } = Vector2.zero;
    public bool IsSprinting { get; private set; } = false;

    private InputAction moveAction;
    private InputAction sprintAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        sprintAction = InputSystem.actions.FindAction("Sprint");
        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprint;
    }

    void OnDestroy()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        sprintAction.performed -= OnSprint;
        sprintAction.canceled -= OnSprint;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        IsSprinting = context.ReadValueAsButton();
    }
}
