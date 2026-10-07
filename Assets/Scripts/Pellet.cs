using UnityEngine;

public class Pellet : MonoBehaviour
{
    public PelletColor pelletColor = PelletColor.Red;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddPellet(pelletColor);
            Destroy(gameObject);
        }
    }
}