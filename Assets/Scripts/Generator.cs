using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Generator : MonoBehaviour
{
    [Header("Generator Settings")]
    public float timeToRepair = 10f; 
    public Key repairKey = Key.E; 
    
    [Header("UI")]
    public Slider progressBar; 
    public Light completionLight; 
    
    public GameObject repairPromptUI; 

    private float currentProgress = 0f;
    private bool isRepaired = false;
    private bool playerInRange = false;

    void Start()
    {
        if (progressBar != null) progressBar.value = 0f;
        if (completionLight != null) completionLight.enabled = false;
        
        if (repairPromptUI != null) repairPromptUI.SetActive(false); 
    }

    void Update()
    {
        if (isRepaired) return;

        bool isHoldingKey = Keyboard.current != null && Keyboard.current[repairKey].isPressed;
        
        if (playerInRange && isHoldingKey)
        {
            currentProgress += Time.deltaTime;
            UpdateUI();

            if (repairPromptUI != null) repairPromptUI.SetActive(false);

            if (currentProgress >= timeToRepair)
            {
                CompleteGenerator();
            }
        }
        else if (playerInRange && !isHoldingKey)
        {
            if (repairPromptUI != null) repairPromptUI.SetActive(true);
        }
    }

    void CompleteGenerator()
    {
        isRepaired = true;
        
        if (completionLight != null) completionLight.enabled = true;
        if (progressBar != null) progressBar.gameObject.SetActive(false); 
        
        if (repairPromptUI != null) repairPromptUI.SetActive(false); 

        Debug.Log("Generator Fully Repaired!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRepaired)
        {
            playerInRange = true;
            if (progressBar != null) progressBar.gameObject.SetActive(true);
            
            if (repairPromptUI != null) repairPromptUI.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (progressBar != null) progressBar.gameObject.SetActive(false);
            
            if (repairPromptUI != null) repairPromptUI.SetActive(false); 
        }
    }

    void UpdateUI()
    {
        if (progressBar != null)
        {
            progressBar.value = currentProgress / timeToRepair;
        }
    }
}