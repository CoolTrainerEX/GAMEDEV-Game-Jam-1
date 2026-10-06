using UnityEngine;

public class Pellet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddPellet();
            Destroy(gameObject);
        }
    }
}