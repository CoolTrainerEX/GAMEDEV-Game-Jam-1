using System.Collections;
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
    [SerializeField] private GameSettings settings;
    [SerializeField] private AudioSource jumpscareAudioSource;

    private ControlsManager controls;
    private PauseInput pauseInput;
    private PanelRenderer panelRenderer;

    private VisualElement hudElement;
    private VisualElement pauseElement;
    private VisualElement jumpscareOverlay;
    private VisualElement gameOverElement;

    private VisualElement promptElement;
    private Label pelletLabel;
    private Label generatorLabel;
    private Label promptLabel;
    private Label livesLabel;
    private VisualElement livesContainer;
    private Label deathText;

    private ProgressBar repairProgressBar;
    private Button backButton;
    private Button exitButton;
    private Button restartButton;
    private Button gameOverExitButton;

    private bool isPromptActive = false;
    private bool isProgressBarActive = false;
    private bool paused = false;
    private int uiVersion = 0;

    private Label redPelletLabel;
    private Label pinkPelletLabel;
    private Label cyanPelletLabel;
    private Label orangePelletLabel;

    private VisualElement winElement;
    private Button winRestartButton;
    private Button winExitButton;

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
            GameManager.Instance.OnLivesChanged += UpdateLivesUI;
            GameManager.Instance.OnPromptVisibilityChanged += HandlePromptVisibility;
            GameManager.Instance.OnRepairProgressChanged += HandleRepairProgress;
            GameManager.Instance.OnPlayerHit += TriggerJumpscare;
            GameManager.Instance.OnGameOver += TriggerGameOver;
            GameManager.Instance.OnGameWin += TriggerWinScreen;
        }

        UpdateUI();
        UpdateLivesUI();
    }

    void OnDestroy()
    {
        if (panelRenderer != null)
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateUI;
            GameManager.Instance.OnLivesChanged -= UpdateLivesUI;
            GameManager.Instance.OnPromptVisibilityChanged -= HandlePromptVisibility;
            GameManager.Instance.OnRepairProgressChanged -= HandleRepairProgress;
            GameManager.Instance.OnPlayerHit -= TriggerJumpscare;
            GameManager.Instance.OnGameOver -= TriggerGameOver;
            GameManager.Instance.OnGameWin -= TriggerWinScreen;
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

            if (mixer != null) mixer.SetFloat("MasterVolume", -80);
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

            if (mixer != null) mixer.SetFloat("MasterVolume", 0);
        }

        paused = pauseInput.IsPaused;
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        if (uiVersion == version) return;
        uiVersion = version;

        hudElement = rootElement.Q<VisualElement>("HUD");
        pauseElement = rootElement.Q<VisualElement>("Pause");
        jumpscareOverlay = rootElement.Q<VisualElement>("JumpscareOverlay");
        gameOverElement = rootElement.Q<VisualElement>("GameOver");
        winElement = rootElement.Q<VisualElement>("WinScreen");

        if (backButton != null) backButton.clicked -= OnPlay;
        if (exitButton != null) exitButton.clicked -= OnExit;
        if (restartButton != null) restartButton.clicked -= OnRestart;
        if (gameOverExitButton != null) gameOverExitButton.clicked -= OnExit;
        if (winRestartButton != null) winRestartButton.clicked -= OnRestart;
        if (winExitButton != null) winExitButton.clicked -= OnExit;

        backButton = rootElement.Q<Button>("Back");
        exitButton = rootElement.Q<Button>("Exit");

        if (gameOverElement != null)
        {
            restartButton = gameOverElement.Q<Button>("Restart");
            gameOverExitButton = gameOverElement.Q<Button>("Exit");

            if (restartButton != null) restartButton.clicked += OnRestart;
            if (gameOverExitButton != null) gameOverExitButton.clicked += OnExit;
        }

        if (winElement != null)
        {
            winRestartButton = winElement.Q<Button>("WinRestart");
            winExitButton = winElement.Q<Button>("WinExit");

            if (winRestartButton != null) winRestartButton.clicked += OnRestart;
            if (winExitButton != null) winExitButton.clicked += OnExit;
        }

        if (backButton != null) backButton.clicked += OnPlay;
        if (exitButton != null) exitButton.clicked += OnExit;

        if (hudElement != null)
        {
            pelletLabel = hudElement.Q<Label>("PelletText");
            generatorLabel = hudElement.Q<Label>("GeneratorText");
            livesLabel = hudElement.Q<Label>("LivesText");
            livesContainer = hudElement.Q<VisualElement>("LivesContainer");

            redPelletLabel = hudElement.Q<Label>("RedPelletText");
            pinkPelletLabel = hudElement.Q<Label>("PinkPelletText");
            cyanPelletLabel = hudElement.Q<Label>("CyanPelletText");
            orangePelletLabel = hudElement.Q<Label>("OrangePelletText");

            promptElement = hudElement.Q<VisualElement>("Prompt");
            promptLabel = hudElement.Q<Label>("PromptLabel");
            repairProgressBar = hudElement.Q<ProgressBar>("RepairProgressBar");

            UpdateUI();
            UpdateLivesUI();
        }

        if (jumpscareOverlay != null)
        {
            deathText = jumpscareOverlay.Q<Label>("DeathText");
            jumpscareOverlay.style.display = DisplayStyle.None;
        }

        if (gameOverElement != null)
        {
            gameOverElement.style.display = DisplayStyle.None;
        }

        if (winElement != null)
        {
            winElement.style.display = DisplayStyle.None;
        }
    }

    private void UpdateLivesUI()
    {
        if (GameManager.Instance == null)
            return;

        int lives = GameManager.Instance.currentLives;

        if (livesLabel != null)
        {
            livesLabel.text = $"LIVES: {lives}";
        }

        if (livesContainer != null)
        {
            for (int i = 0; i < livesContainer.childCount; i++)
            {
                VisualElement icon = livesContainer[i];
                icon.style.display = (i < lives) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }

    private void TriggerJumpscare(Transform ghostTransform)
    {
        StartCoroutine(JumpscareRoutine());
    }

    private IEnumerator JumpscareRoutine()
    {
        if (jumpscareAudioSource != null && settings != null && settings.jumpscareSound != null)
        {
            jumpscareAudioSource.PlayOneShot(settings.jumpscareSound);
        }

        if (jumpscareOverlay != null)
        {
            if (deathText != null)
            {
                deathText.text = GameManager.Instance.currentLives > 0 ? "YOU DIED" : "GAME OVER";
            }
            jumpscareOverlay.style.display = DisplayStyle.Flex;
        }

        yield return new WaitForSeconds(1.8f);

        if (jumpscareOverlay != null && GameManager.Instance.currentLives > 0)
        {
            jumpscareOverlay.style.display = DisplayStyle.None;
        }
    }

    private void TriggerGameOver()
    {
        if (hudElement != null) hudElement.style.display = DisplayStyle.None;
        if (jumpscareOverlay != null) jumpscareOverlay.style.display = DisplayStyle.None;

        if (gameOverElement != null)
        {
            gameOverElement.style.display = DisplayStyle.Flex;
        }

        controls.MouseLocked = false;
    }

    private void TriggerWinScreen()
    {
        if (hudElement != null) hudElement.style.display = DisplayStyle.None;
        if (jumpscareOverlay != null) jumpscareOverlay.style.display = DisplayStyle.None;

        if (winElement != null)
        {
            winElement.style.display = DisplayStyle.Flex;
        }

        controls.MouseLocked = false;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
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

        int needed = GameManager.Instance.pelletsNeeded;

        if (redPelletLabel != null) redPelletLabel.text = $"{GameManager.Instance.redPellets} / {needed}";
        if (pinkPelletLabel != null) pinkPelletLabel.text = $"{GameManager.Instance.pinkPellets} / {needed}";
        if (cyanPelletLabel != null) cyanPelletLabel.text = $"{GameManager.Instance.cyanPellets} / {needed}";
        if (orangePelletLabel != null) orangePelletLabel.text = $"{GameManager.Instance.orangePellets} / {needed}";

        if (generatorLabel != null)
        {
            generatorLabel.text = $"{GameManager.Instance.generatorsRepaired}";
        }
    }

    private void OnPlay()
    {
        pauseInput.TogglePause();
    }

    private void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnExit()
    {
        Time.timeScale = 1;
        if (mixer != null) mixer.SetFloat("MasterVolume", 0);
        SceneManager.LoadScene("Menu");
    }
}