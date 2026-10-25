using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Light))]
public class LightDropoffFeature : MonoBehaviour
{
    [Header("Raycast Distance Settings")]
    [Tooltip("Distance at which the flashlight starts dimming.")]
    [SerializeField] private float maxDistance = 5.0f;

    [Tooltip("Distance at which the flashlight reaches its lowest intensity.")]
    [SerializeField] private float minDistance = 0.5f;

    [Header("Layer Filtering")]
    [Tooltip("Layers that the raycast will ignore (e.g., Player, Triggers, TransparentFX).")]
    [SerializeField] private LayerMask ignoreLayers;

    [Header("Intensity Settings")]
    [Tooltip("Full intensity when no nearby obstacle is hit (Lumens/Candela in HDRP).")]
    [SerializeField] private float maxIntensity = 5000.0f;

    [Tooltip("Minimum intensity when right up against an object.")]
    [SerializeField] private float minIntensity = 200.0f;

    [Header("Smoothing")]
    [Tooltip("Speed of the intensity transition to prevent abrupt lighting snaps.")]
    [SerializeField] private float smoothSpeed = 10.0f;

    [Header("Raycast Origin (Optional)")]
    [Tooltip("Assign the Main Camera or Player Head here if the flashlight follows camera view. Leave empty to use flashlight transform.")]
    [SerializeField] private Transform raycastOrigin;

    private Light targetLight;
    private HDAdditionalLightData hdLightData;
    private float currentIntensity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        targetLight = GetComponent<Light>();
        hdLightData = GetComponent<HDAdditionalLightData>();
        currentIntensity = maxIntensity;
    }

    // Update is called once per frame
    private void Update()
    {
        Transform originTransform = raycastOrigin != null ? raycastOrigin : transform;
        Ray ray = new Ray(originTransform.position, originTransform.forward);

        int layerMask = ~ignoreLayers;

        float targetIntensity = maxIntensity;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            float t = Mathf.InverseLerp(minDistance, maxDistance, hit.distance);
            targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothSpeed);

        if (hdLightData != null)
        {
            hdLightData.intensity = currentIntensity;
        }
        else
        {
            targetLight.intensity = currentIntensity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw debug line in Scene View for easy visual setup
        Transform originTransform = raycastOrigin != null ? raycastOrigin : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(originTransform.position, originTransform.forward * maxDistance);
    }
}
