using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class Pellet : MonoBehaviour
{
    public PelletColor pelletColor = PelletColor.Red;
    private AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private async Awaitable OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddPellet(pelletColor);
            audio.Play();

            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            while (audio.isPlaying) await Awaitable.EndOfFrameAsync();

            Destroy(gameObject);
        }
    }
}