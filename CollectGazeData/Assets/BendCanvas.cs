using UnityEngine;

public class CurveCanvas : MonoBehaviour
{
    public float radius = 2f;
    public float angleRange = 60f;

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        var children = rectTransform.GetComponentsInChildren<RectTransform>();

        float totalWidth = rectTransform.rect.width * rectTransform.localScale.x;
        float anglePerUnit = angleRange / totalWidth;

        foreach (var child in children)
        {
            if (child == rectTransform) continue;
            float xPos = child.localPosition.x;
            float angle = xPos * anglePerUnit;
            float zPos = radius * (1 - Mathf.Cos(angle * Mathf.Deg2Rad));
            float newXPos = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            child.localPosition = new Vector3(newXPos, child.localPosition.y, zPos);
            child.localRotation = Quaternion.Euler(0, -angle, 0);
        }
    }
}