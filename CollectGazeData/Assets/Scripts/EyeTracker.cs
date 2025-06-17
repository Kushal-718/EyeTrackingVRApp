using UnityEngine;

public class EyeTracker : MonoBehaviour
{
    public GameObject movingSphere; // Optional: Only needed in MainScene2
    private Transform leftEyeTransform;
    private Transform rightEyeTransform;
    private Renderer sphereRenderer;
    private Color originalColor;
    private Color gazedColor = Color.green;

    void Start()
    {
        // Find left and right eye transforms
        leftEyeTransform = GameObject.Find("OVRCameraRig/TrackingSpace/LeftEyeAnchor").transform;
        rightEyeTransform = GameObject.Find("OVRCameraRig/TrackingSpace/RightEyeAnchor").transform;

        if (leftEyeTransform == null || rightEyeTransform == null)
        {
            Debug.LogError("Left or Right Eye Anchor not found!");
        }

        // Initialize sphere renderer only if movingSphere is assigned
        if (movingSphere != null)
        {
            sphereRenderer = movingSphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                originalColor = sphereRenderer.material.color;
            }
            else
            {
                Debug.LogWarning("Moving Sphere does not have a Renderer component!");
            }
        }
    }

    void Update()
    {
        if (leftEyeTransform == null || rightEyeTransform == null) return;

        Ray leftRay = new Ray(leftEyeTransform.position, leftEyeTransform.forward);
        Ray rightRay = new Ray(rightEyeTransform.position, rightEyeTransform.forward);

        RaycastHit leftHit, rightHit;
        bool isGazingAtSphere = false;

        // Check left eye ray
        if (Physics.Raycast(leftRay, out leftHit))
        {
            Debug.Log($"Left Eye Gaze Point: {leftHit.point}");
            GameObject hitObject = leftHit.collider.gameObject;
            if (movingSphere != null && hitObject == movingSphere)
            {
                isGazingAtSphere = true;
            }
            GazeButton gazeButton = hitObject.GetComponent<GazeButton>();
            if (gazeButton != null)
            {
                gazeButton.OnGazeEnter();
            }
        }

        // Check right eye ray
        if (Physics.Raycast(rightRay, out rightHit))
        {
            Debug.Log($"Right Eye Gaze Point: {rightHit.point}");
            GameObject hitObject = rightHit.collider.gameObject;
            if (movingSphere != null && hitObject == movingSphere)
            {
                isGazingAtSphere = true;
            }
            GazeButton gazeButton = hitObject.GetComponent<GazeButton>();
            if (gazeButton != null)
            {
                gazeButton.OnGazeEnter();
            }
        }

        // Reset buttons if not gazed at
        foreach (GazeButton button in FindObjectsOfType<GazeButton>())
        {
            if (leftHit.collider == null || leftHit.collider.gameObject != button.gameObject &&
                rightHit.collider == null || rightHit.collider.gameObject != button.gameObject)
            {
                button.OnGazeExit();
            }
        }

        // Log sphere position only if movingSphere is assigned
        if (movingSphere != null)
        {
            Debug.Log($"Moving Sphere Position: {movingSphere.transform.position}");
        }

        // Change sphere color if gazed at (only if movingSphere is assigned)
        if (isGazingAtSphere && sphereRenderer != null)
        {
            sphereRenderer.material.color = gazedColor;
        }
        else if (sphereRenderer != null)
        {
            sphereRenderer.material.color = originalColor;
        }
    }

    void OnDrawGizmos()
    {
        if (leftEyeTransform != null && rightEyeTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(leftEyeTransform.position, leftEyeTransform.forward * 10f);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rightEyeTransform.position, rightEyeTransform.forward * 10f);
        }
    }
}