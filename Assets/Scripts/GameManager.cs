using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int pelletsCollected = 0;
    public int totalPelletsNeeded = 25;
    public int generatorsRepaired = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;
    }

    public void AddPellet()
    {
        pelletsCollected++;
        UpdateUI();

        if (pelletsCollected >= totalPelletsNeeded)
        {
            Debug.Log("All pellets collected! Open the exit doors!");
        }
    }

    void UpdateUI()
    {
        // UI code here
    }

    public void AddGenerator()
    {
        generatorsRepaired++;
        Debug.Log($"Generators repaired: {generatorsRepaired}");
    }
}