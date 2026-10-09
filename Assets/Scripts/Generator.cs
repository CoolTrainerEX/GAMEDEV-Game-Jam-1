using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : MonoBehaviour
{
    [Header("Generator Settings")]
    public float timeToRepair = 10f; 
    public Key repairKey = Key.E; 
    
    [Header("Pellet Requirements")]
    public PelletColor requiredPelletColor = PelletColor.Red;
    
    [Header("Visuals")]
    public Light completionLight; 
    public GameObject roomLights;
    
    private float currentProgress = 0f;
    private bool isRepaired = false;
    private bool playerInRange = false;

    void Start()
    {
        if (completionLight != null) completionLight.enabled = false;
    }

    void Update()
    {
        if (isRepaired || !playerInRange) return;

        bool hasEnough = HasEnoughPellets();
        bool isHoldingKey = Keyboard.current != null && Keyboard.current[repairKey].isPressed;
        
        if (hasEnough)
        {
            if (isHoldingKey)
            {
                currentProgress += Time.deltaTime;

                GameManager.Instance.UpdateRepairProgress(true, currentProgress / timeToRepair);
                GameManager.Instance.SetPromptVisibility(false); 

                if (currentProgress >= timeToRepair)
                {
                    CompleteGenerator();
                }
            }
            else
            {
                GameManager.Instance.UpdateRepairProgress(false, 0f);
                GameManager.Instance.SetPromptVisibility(true, "Hold E to Repair");
            }
        }
        else
        {
            GameManager.Instance.UpdateRepairProgress(false, 0f);
            GameManager.Instance.SetPromptVisibility(true, $"Need {GameManager.Instance.pelletsNeeded} {requiredPelletColor} Pellets!");
        }
    }

    private bool HasEnoughPellets()
    {
        if (GameManager.Instance == null) return false;

        switch (requiredPelletColor)
        {
            case PelletColor.Red: return GameManager.Instance.redPellets >= GameManager.Instance.pelletsNeeded;
            case PelletColor.Pink: return GameManager.Instance.pinkPellets >= GameManager.Instance.pelletsNeeded;
            case PelletColor.Cyan: return GameManager.Instance.cyanPellets >= GameManager.Instance.pelletsNeeded;
            case PelletColor.Orange: return GameManager.Instance.orangePellets >= GameManager.Instance.pelletsNeeded;
            default: return false;
        }
    }

    void CompleteGenerator()
    {
        isRepaired = true;
        
        if (completionLight != null) completionLight.enabled = true;
        if (roomLights != null) roomLights.SetActive(true);
        
        GameManager.Instance.AddGenerator();
        GameManager.Instance.SetPromptVisibility(false);
        GameManager.Instance.UpdateRepairProgress(false, 0f);
        
        Debug.Log("Generator Fully Repaired!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRepaired)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GameManager.Instance.SetPromptVisibility(false);
            GameManager.Instance.UpdateRepairProgress(false, 0f);
        }
    }

    
}