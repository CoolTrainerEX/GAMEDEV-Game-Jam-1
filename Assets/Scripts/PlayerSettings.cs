using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    [Min(0)] public float speed = 3f;
    [Min(1)] public float sprintMultiplier = 1.5f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 25f;
    public float staminaRegenRate = 15f;
    public float regenDelay = 1f;            
    public float minStaminaToSprint = 20f;   
}