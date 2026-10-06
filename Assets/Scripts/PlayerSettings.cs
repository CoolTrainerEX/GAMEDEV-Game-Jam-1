using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement")]
    [Min(0)] public float speed = 2;
}
