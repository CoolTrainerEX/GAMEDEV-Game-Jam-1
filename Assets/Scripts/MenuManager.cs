using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class MenuManager : MonoBehaviour
{
    private PanelRenderer panelRenderer;
    private int uiVersion = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelRenderer = GetComponent<PanelRenderer>();

        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDestroy()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        if (uiVersion == version) return;

        uiVersion = version;
        rootElement.Q<Button>("Play").clicked += OnPlay;
        rootElement.Q<Button>("Exit").clicked += OnExit;
    }

    private static void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }

    private static void OnExit()
    {
        Application.Quit();
    }
}