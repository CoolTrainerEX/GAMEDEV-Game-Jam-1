using UnityEngine;

public class LightRaycast : MonoBehaviour
{
    public float maxDistance = 20f;
    public LayerMask hitLayer;

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * maxDistance, Color.yellow);
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, hitLayer))
        {
            // Debug.Log($"Light hit: {hit.collider.name}", hit.collider.gameObject);
        }
    }
}
