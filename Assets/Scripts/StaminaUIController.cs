using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StaminaUIController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private string staminaBarName = "StaminaBar";

    private ProgressBar staminaBar;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        staminaBar = root.Q<ProgressBar>(staminaBarName);

        if (playerMovement != null)
        {
            playerMovement.OnStaminaChanged.AddListener(UpdateStamina);
        }
    }

    private void OnDisable()
    {
        if (playerMovement != null)
        {
            playerMovement.OnStaminaChanged.RemoveListener(UpdateStamina);
        }
    }

    private void UpdateStamina(float current, float max)
    {
        if (staminaBar == null) return;

        staminaBar.highValue = max;
        staminaBar.value = current;
    }
}