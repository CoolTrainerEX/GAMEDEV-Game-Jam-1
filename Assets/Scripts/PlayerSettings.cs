using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    [Min(0)] public float speed = 5f;
    [Min(1)] public float sprintMultiplier = 2f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 25f;
    public float regenDelay = 0.05f;            
    public float minStaminaToSprint = 20f;   
}