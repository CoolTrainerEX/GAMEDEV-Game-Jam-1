using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 Move { get; private set; } = Vector2.zero;

    private InputAction moveAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
    }

    void OnDestroy()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }
}
