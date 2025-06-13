using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    private Image buttonImage;
    public Color normalColor = new Color32(0, 209, 255, 255); // Neon Blue
    public Color hoverColor = new Color32(255, 0, 160, 255); // Neon Pink

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
    }

    public void OnGazeEnter()
    {
        buttonImage.color = hoverColor;
    }

    public void OnGazeExit()
    {
        buttonImage.color = normalColor;
    }
}
