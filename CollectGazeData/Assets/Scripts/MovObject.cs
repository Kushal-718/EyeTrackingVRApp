using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 2f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;
        //transform.position = startPosition + new Vector3(offset, 0, 0);
        transform.position = new Vector3(offset, startPosition.y, startPosition.z);
    }
}