using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public bool MouseLocked { get; set; } = false;
    public bool IsTouchscreen { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDestroy()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.ActionPerformed) return;

        var device = ((InputAction)obj).activeControl.device;

        IsTouchscreen = device is Touchscreen;

        if ((device is Mouse || device is Keyboard) && !MouseLocked)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}