using UnityEngine;
using System; 

public enum PelletColor { Red, Pink, Cyan, Orange }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 1. Create the Event
    public Action OnScoreChanged;

    [Header("Game State")]
    public int redPellets = 0;
    public int pinkPellets = 0;
    public int cyanPellets = 0;
    public int orangePellets = 0;
    public int pelletsNeeded = 25; 
    public int generatorsRepaired = 0;

    public Action<bool, string> OnPromptVisibilityChanged;
    public Action<bool, float> OnRepairProgressChanged;

    public void SetPromptVisibility(bool isVisible, string message = "")
    {
        OnPromptVisibilityChanged?.Invoke(isVisible, message);
    }

    public void UpdateRepairProgress(bool isVisible, float progressPercent)
    {
        OnRepairProgressChanged?.Invoke(isVisible, progressPercent);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
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
    }
}