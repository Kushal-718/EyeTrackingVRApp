using UnityEngine;

public class EyeInteractableTeleportingSphere : MonoBehaviour
{
    [SerializeField] private float appearDuration = 2f; // Time the object is visible
    [SerializeField] private float disappearDuration = 1f; // Time the object is invisible
    [SerializeField] private float radius = 5f; // Radius for random positioning
    [SerializeField] private Color hoverColor = Color.green; // Color when gazed at

    private Renderer objectRenderer;
    private AudioSource audioSource;
    private Color originalColor;
    private bool isHovered = false;
    private Vector3 startPosition;
    private float timer = 0f;
    private bool isVisible = false;
    public System.Action<Vector3> OnSphereTeleported;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        else
        {
            Debug.LogError("Renderer component not found on this GameObject!");
        }
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on " + gameObject.name);
        }
    }

    void Start()
    {
        startPosition = transform.position; // Store the initial position as the center
        SetVisibility(false); // Start invisible
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isVisible)
        {
            if (timer >= appearDuration)
            {
                SetVisibility(false);
                timer = 0f;
            }
        }
        else
        {
            if (timer >= disappearDuration)
            {
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * radius;
                randomOffset.y = 0; // Keep the height constant
                transform.position = startPosition + randomOffset;
                SetVisibility(true);
                timer = 0f;
                OnSphereTeleported?.Invoke(transform.position);
                Debug.Log($"Sphere teleported to position: {transform.position} at time: {Time.time}");
            }
        }
    }

    public void SetHovered(bool hovered)
    {
        if (objectRenderer == null) return;

        isHovered = hovered;
        if (hovered && isVisible)
        {
            objectRenderer.material.color = hoverColor;
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (isVisible)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    private void SetVisibility(bool visible)
    {
        isVisible = visible;
        if (objectRenderer != null)
        {
            objectRenderer.enabled = visible;
            if (visible)
            {
                objectRenderer.material.color = isHovered ? hoverColor : originalColor;
            }
        }
        // Ensure collider is disabled when invisible to prevent raycast hits
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = visible;
        }
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}