using UnityEngine;
using UnityEngine.UI;

public class DualEyeGaze : MonoBehaviour
{
    public Transform leftEyeAnchor; // Assign LeftEyeAnchor from OVRCameraRig
    public Transform rightEyeAnchor; // Assign RightEyeAnchor from OVRCameraRig
    public GameObject leftGazeVisual; // LeftGazeVisual prefab (Neon Blue sphere)
    public GameObject rightGazeVisual; // RightGazeVisual prefab (Neon Pink sphere)
    public float rayLength = 10f; // Max ray distance
    public LayerMask interactionLayer; // Set to UI layer

    private OVREyeGaze leftEyeGaze;
    private OVREyeGaze rightEyeGaze;

    void Start()
    {
        leftEyeGaze = leftEyeAnchor.GetComponent<OVREyeGaze>();
        rightEyeGaze = rightEyeAnchor.GetComponent<OVREyeGaze>();
        Debug.Log("LeftEyeAnchor: " + (leftEyeAnchor != null));
        Debug.Log("RightEyeAnchor: " + (rightEyeAnchor != null));
        Debug.Log("LeftGazeVisual: " + (leftGazeVisual != null));
        Debug.Log("RightGazeVisual: " + (rightGazeVisual != null));
    }

    void Update()
    {
        bool leftHitUI = false;
        bool rightHitUI = false;
        Button hitButton = null;
        RaycastHit leftHit = new RaycastHit(); // Store hit info
        RaycastHit rightHit = new RaycastHit();

        // Left eye ray
        if (leftEyeGaze && leftEyeGaze.EyeTrackingEnabled)
        {
            Ray leftRay = new Ray(leftEyeAnchor.position, leftEyeAnchor.forward);
            if (Physics.Raycast(leftRay, out leftHit, rayLength, interactionLayer))
            {
                leftGazeVisual.transform.position = leftHit.point;
                leftGazeVisual.SetActive(true);
                if (leftHit.collider.GetComponent<Button>())
                {
                    leftHitUI = true;
                    hitButton = leftHit.collider.GetComponent<Button>();
                    Debug.Log("Left ray hit UI button: " + hitButton.name);
                }
            }
            else
            {
                leftGazeVisual.transform.position = leftEyeAnchor.position + leftEyeAnchor.forward * rayLength;
                leftGazeVisual.SetActive(true);
            }
        }
        else
        {
            leftGazeVisual.SetActive(false);
        }

        // Right eye ray
        if (rightEyeGaze && rightEyeGaze.EyeTrackingEnabled)
        {
            Ray rightRay = new Ray(rightEyeAnchor.position, rightEyeAnchor.forward);
            if (Physics.Raycast(rightRay, out rightHit, rayLength, interactionLayer))
            {
                rightGazeVisual.transform.position = rightHit.point;
                rightGazeVisual.SetActive(true);
                if (rightHit.collider.GetComponent<Button>() && rightHit.collider == leftHit.collider)
                {
                    rightHitUI = true;
                    Debug.Log("Right ray hit same UI button: " + hitButton.name);
                }
            }
            else
            {
                rightGazeVisual.transform.position = rightEyeAnchor.position + rightEyeAnchor.forward * rayLength;
                rightGazeVisual.SetActive(true);
            }
        }
        else
        {
            rightGazeVisual.SetActive(false);
        }

        // Trigger button click if both eyes hit the same button
        if (leftHitUI && rightHitUI && hitButton != null)
        {
            hitButton.onClick.Invoke();
            Debug.Log("Button clicked via gaze: " + hitButton.name);
        }
    }
}