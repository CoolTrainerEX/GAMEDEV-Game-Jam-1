using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : MonoBehaviour
{
    [Header("Generator Settings")]
    public float timeToRepair = 10f; 
    public Key repairKey = Key.E; 
    
    [Header("Visuals")]
    public Light completionLight; 
    
    private float currentProgress = 0f;
    private bool isRepaired = false;
    private bool playerInRange = false;

    void Start()
    {
        if (completionLight != null) completionLight.enabled = false;
    }

    void Update()
    {
        if (isRepaired) return;

        bool isHoldingKey = Keyboard.current != null && Keyboard.current[repairKey].isPressed;
        
        if (playerInRange && isHoldingKey)
        {
            currentProgress += Time.deltaTime;

            GameManager.Instance.UpdateRepairProgress(true, currentProgress / timeToRepair);

            GameManager.Instance.SetPromptVisibility(false); 

            if (currentProgress >= timeToRepair)
            {
                CompleteGenerator();
            }
        }
        else if (playerInRange && !isHoldingKey)
        {
            GameManager.Instance.UpdateRepairProgress(false, 0f);
            GameManager.Instance.SetPromptVisibility(true, "Hold E to Repair");
        }
    }

    void CompleteGenerator()
    {
        isRepaired = true;
        
        if (completionLight != null) completionLight.enabled = true;
        
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
            GameManager.Instance.SetPromptVisibility(true, "Hold E to Repair");
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