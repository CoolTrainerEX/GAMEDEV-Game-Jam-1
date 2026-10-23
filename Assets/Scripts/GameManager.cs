using UnityEngine;
using System;
using System.Collections;

public enum PelletColor { Red, Pink, Cyan, Orange }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public GameSettings settings;

    // Events
    public Action OnScoreChanged;
    public Action OnLivesChanged;
    public Action<Transform> OnPlayerHit; 
    public Action OnGameOver;

    [Header("Game State")]
    public int redPellets = 0;
    public int pinkPellets = 0;
    public int cyanPellets = 0;
    public int orangePellets = 0;
    public int pelletsNeeded = 25;
    public int generatorsRepaired = 0;

    [Header("Lives & Respawn")]
    public int currentLives = 3;
    public int maxLives = 3;
    public bool isDying = false;

    [Header("Spawn Positions")]
    public Transform playerTransform;
    public Transform[] ghostTransforms;
    public Transform spawnPoint;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;
    private Vector3[] initialGhostPositions;

    public Action OnGameWin;

    [Header("Win Condition")]
    public int totalGeneratorsNeeded = 4;
    public bool isGameWon = false;

    public Action<bool, string> OnPromptVisibilityChanged;
    public Action<bool, float> OnRepairProgressChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (settings != null)
        {
            maxLives = settings.maxLives;
        }
        currentLives = maxLives;
    }

    void Start()
    {
        if (spawnPoint != null)
        {
            initialPlayerPosition = spawnPoint.position;
            initialPlayerRotation = spawnPoint.rotation;
        }
        else if (playerTransform != null)
        {
            initialPlayerPosition = playerTransform.position;
            initialPlayerRotation = playerTransform.rotation;
        }

        if (ghostTransforms != null && ghostTransforms.Length > 0)
        {
            initialGhostPositions = new Vector3[ghostTransforms.Length];
            for (int i = 0; i < ghostTransforms.Length; i++)
            {
                if (ghostTransforms[i] != null)
                    initialGhostPositions[i] = ghostTransforms[i].position;
            }
        }
    }

    public void SetPromptVisibility(bool isVisible, string message = "")
    {
        OnPromptVisibilityChanged?.Invoke(isVisible, message);
    }

    public void UpdateRepairProgress(bool isVisible, float progressPercent)
    {
        OnRepairProgressChanged?.Invoke(isVisible, progressPercent);
    }

    public void AddPellet(PelletColor color)
    {
        switch (color)
        {
            case PelletColor.Red: redPellets++; break;
            case PelletColor.Pink: pinkPellets++; break;
            case PelletColor.Cyan: cyanPellets++; break;
            case PelletColor.Orange: orangePellets++; break;
        }

        OnScoreChanged?.Invoke();
    }

    public void AddGenerator()
    {
        generatorsRepaired++;
        OnScoreChanged?.Invoke();

        if (generatorsRepaired >= totalGeneratorsNeeded && !isGameWon)
        {
            isGameWon = true;
            OnGameWin?.Invoke();
        }
    }

    public void PlayerTouchedGhost(Transform ghostTransform)
    {
        if (isDying) return;
        StartCoroutine(HandlePlayerDeathRoutine(ghostTransform));
    }

    private IEnumerator HandlePlayerDeathRoutine(Transform ghostTransform)
    {
        isDying = true;

        if (playerTransform != null && ghostTransform != null)
        {
            Vector3 lookDir = (ghostTransform.position - playerTransform.position).normalized;
            lookDir.y = 0; // Keep horizon level
            if (lookDir != Vector3.zero)
            {
                playerTransform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        currentLives--;
        OnLivesChanged?.Invoke();
        OnPlayerHit?.Invoke(ghostTransform);

        yield return new WaitForSeconds(1.8f);

        if (currentLives > 0)
        {
            ResetPositions();
            isDying = false;
        }
        else
        {
            OnGameOver?.Invoke();
        }
    }

    public void ResetPositions()
    {
        if (playerTransform != null)
        {
            Vector3 targetPos = spawnPoint != null ? spawnPoint.position : initialPlayerPosition;
            Quaternion targetRot = spawnPoint != null ? spawnPoint.rotation : initialPlayerRotation;

            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerTransform.position = targetPos;
            playerTransform.rotation = targetRot;

            Physics.SyncTransforms();

            if (cc != null) 
                cc.enabled = true;
        }

        if (ghostTransforms != null)
        {
            for (int i = 0; i < ghostTransforms.Length; i++)
            {
                if (ghostTransforms[i] != null && initialGhostPositions != null && i < initialGhostPositions.Length)
                {
                    UnityEngine.AI.NavMeshAgent agent = ghostTransforms[i].GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (agent != null)
                        agent.Warp(initialGhostPositions[i]);
                    else
                        ghostTransforms[i].position = initialGhostPositions[i];
                }
            }
        }
    }
}