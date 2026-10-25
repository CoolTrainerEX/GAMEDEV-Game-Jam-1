using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class Ghost : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private Transform target;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isDying)
        {
            agent.isStopped = true;
            return;
        }

        if (agent.isActiveAndEnabled && target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null && !GameManager.Instance.isDying)
            {
                GameManager.Instance.PlayerTouchedGhost(transform);
            }
        }
    }

    public async Awaitable Die()
    {
        var audio = GetComponent<AudioSource>();

        if (audio != null && settings != null && settings.ghostDieSound != null)
        {
            audio.PlayOneShot(settings.ghostDieSound);
        }

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        while (audio.isPlaying) await Awaitable.EndOfFrameAsync();

        Destroy(gameObject);
    }
}