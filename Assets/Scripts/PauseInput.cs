using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    public bool IsPaused { get; private set; } = false;

    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    private InputAction pauseAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerActionMap = InputSystem.actions.FindActionMap("Player");
        uiActionMap = InputSystem.actions.FindActionMap("UI");
        pauseAction = InputSystem.actions.FindAction("Pause");
        pauseAction.performed += OnPause;

        playerActionMap.Enable();
        uiActionMap.Disable();
    }

    void OnDestroy()
    {
        pauseAction.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            playerActionMap.Disable();
            uiActionMap.Enable();
        }
        else
        {
            playerActionMap.Enable();
            uiActionMap.Disable();
        }
    }
}