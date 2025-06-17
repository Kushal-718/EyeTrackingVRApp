using UnityEngine;

public class DualEyeGaze : MonoBehaviour
{
    public Transform leftEyeAnchor;
    public Transform rightEyeAnchor;
    public Transform centerEyeAnchor;
    public GameObject leftGazeVisual;
    public GameObject rightGazeVisual;
    public float rayLength = 5f;

    private OVREyeGaze leftEyeGaze;
    private OVREyeGaze rightEyeGaze;
    private Camera mainCamera;

    void Start()
    {
        leftEyeGaze = leftEyeAnchor.GetComponent<OVREyeGaze>();
        rightEyeGaze = rightEyeAnchor.GetComponent<OVREyeGaze>();
        mainCamera = Camera.main;
        Debug.Log("LeftEyeAnchor: " + (leftEyeAnchor != null));
        Debug.Log("RightEyeAnchor: " + (rightEyeAnchor != null));
        Debug.Log("CenterEyeAnchor: " + (centerEyeAnchor != null));
        Debug.Log("LeftGazeVisual: " + (leftGazeVisual != null));
        Debug.Log("RightGazeVisual: " + (rightGazeVisual != null));
        Debug.Log("LeftEyeGaze Component: " + (leftEyeGaze != null));
        Debug.Log("RightEyeGaze Component: " + (rightEyeGaze != null));
        Debug.Log("MainCamera: " + (mainCamera != null));

        if (leftGazeVisual != null) leftGazeVisual.SetActive(true);
        if (rightGazeVisual != null) rightGazeVisual.SetActive(true);
    }

    void Update()
    {
        Vector3 leftGazeDirection;
        Vector3 rightGazeDirection;
        Vector3 leftRayOrigin;
        Vector3 rightRayOrigin;

        bool useMainCameraFallback = false;

        if (leftEyeAnchor == null || rightEyeAnchor == null || centerEyeAnchor == null)
        {
            useMainCameraFallback = true;
        }
        else
        {
            Vector3 centerPos = centerEyeAnchor.position;
            Vector3 centerForward = centerEyeAnchor.forward;
            if (float.IsNaN(centerPos.x) || float.IsNaN(centerPos.y) || float.IsNaN(centerPos.z) ||
                float.IsNaN(centerForward.x) || float.IsNaN(centerForward.y) || float.IsNaN(centerForward.z))
            {
                Debug.Log("CenterEyeAnchor has NaN values, using main camera.");
                useMainCameraFallback = true;
            }
        }

        if (useMainCameraFallback)
        {
            Debug.Log("Anchor missing or invalid, using main camera.");
            if (mainCamera != null)
            {
                // Use main camera position as origin
                leftRayOrigin = mainCamera.transform.position;
                rightRayOrigin = mainCamera.transform.position;
                // Ensure direction is outward (camera forward in XR Simulation)
                leftGazeDirection = mainCamera.transform.forward;
                rightGazeDirection = mainCamera.transform.forward;
            }
            else
            {
                Debug.Log("Main camera missing.");
                return;
            }
        }
        else
        {
            if (leftEyeGaze != null && rightEyeGaze != null)
            {
                // Use eye anchor positions as origins
                leftRayOrigin = leftEyeAnchor.position;
                rightRayOrigin = rightEyeAnchor.position;
                leftGazeDirection = leftEyeAnchor.forward;
                rightGazeDirection = rightEyeAnchor.forward;
                Debug.Log("Using OVREyeGaze for gaze direction.");
            }
            else
            {
                // Use center eye anchor as fallback
                leftRayOrigin = centerEyeAnchor.position;
                rightRayOrigin = centerEyeAnchor.position;
                leftGazeDirection = centerEyeAnchor.forward;
                rightGazeDirection = centerEyeAnchor.forward;
                Debug.Log("Using head-based gaze (CenterEyeAnchor).");
            }
        }

        Debug.Log($"Left Ray Origin: {leftRayOrigin}, Direction: {leftGazeDirection}");
        Debug.Log($"Right Ray Origin: {rightRayOrigin}, Direction: {rightGazeDirection}");

        // Draw debug rays (ensure they go outward from eyes)
        Debug.DrawRay(leftRayOrigin, leftGazeDirection * rayLength, Color.blue, 1f);
        Debug.DrawRay(rightRayOrigin, rightGazeDirection * rayLength, Color.magenta, 1f);

        Ray leftRay = new Ray(leftRayOrigin, leftGazeDirection);
        if (Physics.Raycast(leftRay, out RaycastHit leftHit, rayLength))
        {
            if (leftGazeVisual != null)
            {
                leftGazeVisual.transform.position = leftHit.point;
                leftGazeVisual.SetActive(true);
                Debug.Log($"Left Gaze Hit: Object = {leftHit.collider.gameObject.name}, Position = {leftHit.point}");
            }
        }
        else
        {
            if (leftGazeVisual != null)
            {
                leftGazeVisual.transform.position = leftRayOrigin + leftGazeDirection * rayLength;
                leftGazeVisual.SetActive(true);
                Debug.Log("Left Gaze Hit: None");
            }
        }

        Ray rightRay = new Ray(rightRayOrigin, rightGazeDirection);
        if (Physics.Raycast(rightRay, out RaycastHit rightHit, rayLength))
        {
            if (rightGazeVisual != null)
            {
                rightGazeVisual.transform.position = rightHit.point;
                rightGazeVisual.SetActive(true);
                Debug.Log($"Right Gaze Hit: Object = {rightHit.collider.gameObject.name}, Position = {rightHit.point}");
            }
        }
        else
        {
            if (rightGazeVisual != null)
            {
                rightGazeVisual.transform.position = rightRayOrigin + rightGazeDirection * rayLength;
                rightGazeVisual.SetActive(true);
                Debug.Log("Right Gaze Hit: None");
            }
        }
    }
}