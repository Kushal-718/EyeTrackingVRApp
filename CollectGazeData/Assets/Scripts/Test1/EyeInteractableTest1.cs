using UnityEngine;

public class EyeInteractableTest1 : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHovered = false;
    private AudioSource audioSource;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer found on " + gameObject.name);
        }
        else
        {
            originalColor = objectRenderer.material.color;
        }
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on " + gameObject.name);
        }
    }

    public void SetHovered(bool hovered, Color hoverColor)
    {
        if (objectRenderer == null) return;
        isHovered = hovered;
        objectRenderer.material.color = hovered ? hoverColor : originalColor;
        if (hovered && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}