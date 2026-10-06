using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;

    private AudioSource audio;
    private PlayerMovement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.IsMoving && !audio.isPlaying) audio.Play();
        else if (!movement.IsMoving && audio.isPlaying) audio.Stop();
    }
}
