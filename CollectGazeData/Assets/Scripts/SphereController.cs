using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float appearDuration = 3f; // Duration the sphere is visible
    public float disappearDuration = 1f; // Duration the sphere is invisible
    public float radius = 5f; // Radius for random positioning
    private Vector3 startPosition;
    private float timer = 0f;
    private bool isVisible = false;
    private Renderer sphereRenderer;

    void Start()
    {
        startPosition = transform.position; // Center point for random positions
        sphereRenderer = GetComponent<Renderer>();
        if (sphereRenderer == null)
        {
            Debug.LogError("Sphere does not have a Renderer component!");
        }
        // Start invisible
        SetVisibility(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isVisible)
        {
            // Sphere is visible; check if it's time to disappear
            if (timer >= appearDuration)
            {
                SetVisibility(false);
                timer = 0f;
            }
        }
        else
        {
            // Sphere is invisible; check if it's time to reappear
            if (timer >= disappearDuration)
            {
                // Move to a random position within the radius
                Vector3 randomOffset = Random.insideUnitSphere * radius;
                randomOffset.y = 0; // Keep it at the same height
                transform.position = startPosition + randomOffset;
                SetVisibility(true);
                timer = 0f;
                // Log the position and time when the sphere appears
                Debug.Log($"Sphere appeared at position: {transform.position} at time: {Time.time}");
            }
        }
    }

    void SetVisibility(bool visible)
    {
        isVisible = visible;
        if (sphereRenderer != null)
        {
            sphereRenderer.enabled = visible;
        }
    }

    public bool IsSphereVisible()
    {
        return isVisible;
    }
}