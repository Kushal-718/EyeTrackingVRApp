using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeInteractableSample : MonoBehaviour
{
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isHovered = false;
    private Vector3 originalScale;
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer found on " + gameObject.name);
        }
        else
        {
            originalColor = objectRenderer.material.color;
            Debug.Log("Renderer found, original color: " + originalColor);
        }
    }

    public void SetHovered(bool hovered, Color hoverColor)
    {

        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer on " + gameObject.name);
            return;
        }
        isHovered = hovered;
        if (hovered)
        {
            objectRenderer.material.color = hoverColor;
            //transform.localScale = originalScale * 1.1f; // Slightly enlarge
            Debug.Log($"Setting {gameObject.name} color to hover: {hoverColor}");
        }
        else
        {
            objectRenderer.material.color = originalColor;
            //transform.localScale = originalScale; // Restore scale
            Debug.Log($"Restoring {gameObject.name} color to original: {originalColor}");
        }
    }
}