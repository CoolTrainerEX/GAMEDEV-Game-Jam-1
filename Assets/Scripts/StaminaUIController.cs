using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class StaminaUIController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private string staminaBarName = "StaminaBar";

    private PanelRenderer panelRenderer;
    private ProgressBar staminaBar;
    private int uiVersion = 0;

    private void OnEnable()
    {
        panelRenderer = GetComponent<PanelRenderer>();

        panelRenderer.RegisterUIReloadCallback(OnUIReload);

        if (playerMovement != null)
        {
            playerMovement.OnStaminaChanged.AddListener(UpdateStamina);
        }
    }

    private void OnDisable()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        if (playerMovement != null)
        {
            playerMovement.OnStaminaChanged.RemoveListener(UpdateStamina);
        }
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        if (uiVersion == version) return;

        uiVersion = version;

        staminaBar = rootElement.Q<ProgressBar>(staminaBarName);
    }



    private void UpdateStamina(float current, float max)
    {
        if (staminaBar == null) return;

        staminaBar.highValue = max;
        staminaBar.value = current;
    }
}