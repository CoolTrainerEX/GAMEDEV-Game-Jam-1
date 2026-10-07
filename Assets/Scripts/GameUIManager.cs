using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
[RequireComponent(typeof(ControlsManager))]
[RequireComponent(typeof(PauseInput))]
public class GameUIManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private ControlsManager controls;
    private PauseInput pauseInput;
    private PanelRenderer panelRenderer;
    
    private VisualElement hudElement;
    private VisualElement pauseElement;

    private VisualElement promptElement;
    private VisualElement textElement;

    private Label pelletLabel;
    private Label generatorLabel;
    private Label promptLabel;
    private bool isPromptActive = false;
    private ProgressBar repairProgressBar;
    private bool isProgressBarActive = false;
    private Button backButton;
    private Button exitButton;

    private bool paused = false;
    private int uiVersion = 0;

    void Start()
    {
        controls = GetComponent<ControlsManager>();
        pauseInput = GetComponent<PauseInput>();
        panelRenderer = GetComponent<PanelRenderer>();

        panelRenderer.RegisterUIReloadCallback(OnUIReload);
        controls.MouseLocked = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateUI;
            GameManager.Instance.OnPromptVisibilityChanged += HandlePromptVisibility;
            GameManager.Instance.OnRepairProgressChanged += HandleRepairProgress;
        }
        
        UpdateUI();
    }

    void OnDestroy()
    {
        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateUI;
            GameManager.Instance.OnPromptVisibilityChanged -= HandlePromptVisibility;
            GameManager.Instance.OnRepairProgressChanged -= HandleRepairProgress;
        }
    }

    void Update()
    {
        if (pauseElement == null) return;

        if (pauseInput.IsPaused && !paused)
        {
            Time.timeScale = 0;

            if (hudElement != null) hudElement.style.display = DisplayStyle.None;

            pauseElement.style.display = DisplayStyle.Flex;
            controls.MouseLocked = false;

            mixer.SetFloat("MasterVolume", -80);
        }
        else if (!pauseInput.IsPaused && paused)
        {
            Time.timeScale = 1;

            if (hudElement != null) hudElement.style.display = DisplayStyle.Flex;
            
            if (promptLabel != null && isPromptActive) promptLabel.style.display = DisplayStyle.Flex;
            if (repairProgressBar != null && isProgressBarActive) repairProgressBar.style.display = DisplayStyle.Flex;
            
            if (promptElement != null && (isPromptActive || isProgressBarActive)) 
                promptElement.style.display = DisplayStyle.Flex;

            pauseElement.style.display = DisplayStyle.None;
            controls.MouseLocked = true;

            mixer.SetFloat("MasterVolume", 0);
        }

        paused = pauseInput.IsPaused;
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        if (uiVersion == version) return;
        uiVersion = version;
        
        hudElement = rootElement.Q<VisualElement>("HUD"); 
        pauseElement = rootElement.Q<VisualElement>("Pause");

        if (backButton != null) backButton.clicked -= OnPlay;
        if (exitButton != null) exitButton.clicked -= OnExit;

        backButton = rootElement.Q<Button>("Back");
        exitButton = rootElement.Q<Button>("Exit");

        if (backButton != null) backButton.clicked += OnPlay;
        if (exitButton != null) exitButton.clicked += OnExit;

        if (hudElement != null)
        {
            pelletLabel = hudElement.Q<Label>("PelletText");
            generatorLabel = hudElement.Q<Label>("GeneratorText");
            
            promptElement = hudElement.Q<VisualElement>("Prompt"); 
            
            promptLabel = hudElement.Q<Label>("PromptLabel");
            repairProgressBar = hudElement.Q<ProgressBar>("RepairProgressBar");
            
            if (promptLabel == null) Debug.LogError("PromptLabel is MISSING in UI Builder!");

            UpdateUI();
        }
        else
        {
            Debug.LogError("Could not find the HUD template! Check the name in UI Builder.");
        }
    }

    private void HandlePromptVisibility(bool isVisible, string message)
    {
        isPromptActive = isVisible;
            
        if (promptLabel != null) 
        {
            promptLabel.text = message;
            promptLabel.style.display = (isVisible && !paused) ? DisplayStyle.Flex : DisplayStyle.None; 
        }

        if (promptElement != null) 
            promptElement.style.display = ((isPromptActive || isProgressBarActive) && !paused) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void HandleRepairProgress(bool isVisible, float progressPercent)
    {
        isProgressBarActive = isVisible;

        if (repairProgressBar != null) 
        {
            repairProgressBar.value = progressPercent;
            repairProgressBar.style.display = (isVisible && !paused) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (promptElement != null)
            promptElement.style.display = ((isPromptActive || isProgressBarActive) && !paused) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        if (pelletLabel != null)
        {
            
            pelletLabel.text = $"Red: {GameManager.Instance.redPellets}/{GameManager.Instance.pelletsNeeded}\n" +
                               $"Pink: {GameManager.Instance.pinkPellets}/{GameManager.Instance.pelletsNeeded}\n" +
                               $"Cyan: {GameManager.Instance.cyanPellets}/{GameManager.Instance.pelletsNeeded}\n" +
                               $"Orange: {GameManager.Instance.orangePellets}/{GameManager.Instance.pelletsNeeded}";
        }

        if (generatorLabel != null)
        {
            generatorLabel.text = $"Generators: {GameManager.Instance.generatorsRepaired}";
        }
    }

    private void OnPlay()
    {
        pauseInput.TogglePause();
    }

    private void OnExit()
    {
        Time.timeScale = 1;
        mixer.SetFloat("MasterVolume", 0);
        SceneManager.LoadScene("Menu");
    }
}