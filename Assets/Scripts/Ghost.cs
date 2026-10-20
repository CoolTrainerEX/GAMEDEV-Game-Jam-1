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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO Game over
            Debug.Log("Game over!");
        }
    }

    public async Awaitable Die()
    {
        var audio = GetComponent<AudioSource>();

        audio.PlayOneShot(settings.ghostDieSound);

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        while (audio.isPlaying) await Awaitable.EndOfFrameAsync();

        Destroy(gameObject);
    }
}
