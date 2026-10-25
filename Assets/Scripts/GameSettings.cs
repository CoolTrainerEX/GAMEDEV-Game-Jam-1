using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public AudioClip ghostDieSound;
    public AudioClip jumpscareSound;
    public AudioClip playerHitSound;
    public int maxLives = 3;
}