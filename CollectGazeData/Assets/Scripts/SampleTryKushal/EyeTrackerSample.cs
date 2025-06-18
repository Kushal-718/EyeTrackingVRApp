using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine;


public class EyeTrackerSample : MonoBehaviour
{
    [SerializeField]
    private float rayDistance = 5.0f; // Increased for visibility

    [SerializeField]
    private float rayWidth = 0.1f; // Ensure ray is visible

    [SerializeField]
    private LayerMask layersToInclude;

    //[SerializeField]
    //private Color rayColorDefaultState = Color.cyan; // Default: Neon Cyan for left eye

    //[SerializeField]
    //private Color rayColorHoverState = new Color(0.8f, 0.2f, 1f, 0.8f); // Hover: Bright Purple
    [SerializeField] private Color rayColorDefaultState = new Color(0f, 1f, 1f, 0.4f); // Cyan, more transparent
    [SerializeField] private Color rayColorHoverState = new Color(0.8f, 0.2f, 1f, 0.4f); // Purple, more transparent


    [SerializeField]
    private Color objectHoverColor = Color.green;

    private LineRenderer lineRenderer;
    private List<EyeInteractableSample> eyeInteractables = new List<EyeInteractableSample>();
    private string logFilePath;

    [SerializeField] private string eyeSide = "Left"; // Set to "Left" or "Right" in Inspector

    //void Start()
    //{
    //    lineRenderer = GetComponent<LineRenderer>();
    //    if (lineRenderer == null)
    //    {
    //        Debug.LogError("LineRenderer component is missing on " + gameObject.name);
    //        return;
    //    }
    //    // Set material to support transparency
    //    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //    SetupRay();

    //    logFilePath = Path.Combine(Application.persistentDataPath, "EyeTrackingLog.csv");
    //    if (!File.Exists(logFilePath))
    //    {
    //        File.WriteAllText(logFilePath, "Timestamp,RayOriginX,RayOriginY,RayOriginZ,RayDirX,RayDirY,RayDirZ,HitPosX,HitPosY,HitPosZ\n");
    //    }
    //}
    //    lineRenderer = GetComponent<LineRenderer>();
    //    if (lineRenderer == null)
    //    {
    //        Debug.LogError("LineRenderer missing on " + gameObject.name);
    //        return;
    //    }
    //    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //    SetupRay();

    //    // Set log file path: Use Assets/Logs in Editor, persistentDataPath in builds
    //    string logFolder = Application.isEditor ? Path.Combine(Application.dataPath, "Logs") : Application.persistentDataPath;
    //    logFilePath = Path.Combine(logFolder, "EyeTrackingLogSample.csv");

    //    // Create directory if it doesn't exist
    //    try
    //    {
    //        Directory.CreateDirectory(logFolder);
    //        if (!File.Exists(logFilePath))
    //        {
    //            File.WriteAllText(logFilePath, "Timestamp,RayOriginX,RayOriginY,RayOriginZ,RayDirX,RayDirY,RayDirZ,HitPosX,HitPosY,HitPosZ,HitObject\n");
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Failed to create log file at {logFilePath}: {e.Message}");
    //    }
    //}


    //private LineRenderer lineRenderer;
    //private List<EyeInteractableSample> eyeInteractables = new List<EyeInteractableSample>();
    //private string logFilePath;
    private Dictionary<string, float> gazeStartTimes = new Dictionary<string, float>();
    private float currentGazeDuration;
    private string sessionID;

    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer missing on " + gameObject.name);
            return;
        }
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        SetupRay();

        // Generate session ID and log file path with timestamp
        sessionID = System.Guid.NewGuid().ToString();
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string logFolder = Application.isEditor ? Path.Combine(Application.dataPath, "Logs") : Application.persistentDataPath;
        logFilePath = Path.Combine(logFolder, $"EyeTrackingLog_{timestamp}.csv");

        // Create directory and log file
        try
        {
            Directory.CreateDirectory(logFolder);
            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "SessionID,Timestamp,RayOriginX,RayOriginY,RayOriginZ,RayDirX,RayDirY,RayDirZ,HitPosX,HitPosY,HitPosZ,HitObject,EyeSide,GazeDuration\n");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create log file at {logFilePath}: {e.Message}");
        }
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * rayDistance);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, rayCastDirection * rayDistance, Color.blue, 0.1f);
        bool hitDetected = Physics.Raycast(transform.position, rayCastDirection, out hit, rayDistance, layersToInclude);
        Debug.Log($"{gameObject.name} Raycast: Hit = {hitDetected}, LayerMask = {layersToInclude.value}");
        if (hitDetected)
        {
            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;
            Debug.Log($"Ray hit: {hit.transform.name} at {hit.point}");
            var eyeInteractable = hit.transform.GetComponent<EyeInteractableSample>();
            if (eyeInteractable != null)
            {
                Debug.Log($"Hovering {hit.transform.name} with {gameObject.name}");
                if (!eyeInteractables.Contains(eyeInteractable))
                {
                    UnSelect(true);
                    eyeInteractables.Add(eyeInteractable);
                    eyeInteractable.SetHovered(true, objectHoverColor);
                    string objectName = hit.transform.name;
                    if (!gazeStartTimes.ContainsKey(objectName))
                    {
                        gazeStartTimes[objectName] = Time.time;
                    }
                }
                currentGazeDuration = Time.time - gazeStartTimes[hit.transform.name];
            }
            else
            {
                Debug.LogWarning($"No EyeInteractableSample on {hit.transform.name}");
                UnSelect(true);
                currentGazeDuration = 0f;
            }
            Vector3 localHitPos = transform.InverseTransformPoint(hit.point);
            lineRenderer.SetPosition(1, localHitPos);
            LogRayData(transform.position, rayCastDirection, hit.point, true, hit.transform.name, currentGazeDuration);
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            lineRenderer.SetPosition(1, Vector3.forward * rayDistance);
            UnSelect(true);
            currentGazeDuration = 0f;
            LogRayData(transform.position, rayCastDirection, Vector3.zero, false, "None", 0f);
        }
    }

    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            interactable.SetHovered(false, Color.clear);
            string objectName = interactable.gameObject.name;
            if (gazeStartTimes.ContainsKey(objectName))
            {
                float duration = Time.time - gazeStartTimes[objectName];
                Debug.Log($"Gaze on {objectName} lasted {duration:F4} seconds");
                gazeStartTimes.Remove(objectName);
            }
        }
        if (clear)
        {
            eyeInteractables.Clear();
        }
    }

    private void LogRayData(Vector3 origin, Vector3 direction, Vector3 hitPos, bool hit, string hitObject, float gazeDuration)
    {
        float timestamp = Time.time;
        string hitPosX = hit ? hitPos.x.ToString("F4") : "-1";
        string hitPosY = hit ? hitPos.y.ToString("F4") : "-1";
        string hitPosZ = hit ? hitPos.z.ToString("F4") : "-1";
        string line = $"{sessionID},{timestamp:F4},{origin.x:F4},{origin.y:F4},{origin.z:F4},{direction.x:F4},{direction.y:F4},{direction.z:F4},{hitPosX},{hitPosY},{hitPosZ},{hitObject},{eyeSide},{gazeDuration:F4}";
        try
        {
            File.AppendAllText(logFilePath, line + "\n");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write to log file at {logFilePath}: {e.Message}");
        }
    }
}
//public class EyeTrackerSample : MonoBehaviour
//{
//    [SerializeField]
//    private float rayDistance = 5.0f; // Increased for visibility

//    [SerializeField]
//    private float rayWidth = 0.1f; // Ensure ray is visible

//    [SerializeField]
//    private LayerMask layersToInclude;

//    [SerializeField]
//    private Color rayColorDefaultState = Color.cyan; // Default: Neon Cyan for left eye

//    [SerializeField]
//    private Color rayColorHoverState = new Color(0.8f, 0.2f, 1f, 0.8f); // Hover: Bright Purple

//    [SerializeField]
//    private Color objectHoverColor = Color.green;

//    private LineRenderer lineRenderer;
//    private List<EyeInteractableSample> eyeInteractables = new List<EyeInteractableSample>();
//    private string logFilePath;

//    void Start()
//    {
//        lineRenderer = GetComponent<LineRenderer>();
//        if (lineRenderer == null)
//        {
//            Debug.LogError("LineRenderer component is missing on " + gameObject.name);
//            return;
//        }
//        // Set material to support transparency
//        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//        SetupRay();

//        logFilePath = Path.Combine(Application.persistentDataPath, "EyeTrackingLog.csv");
//        if (!File.Exists(logFilePath))
//        {
//            File.WriteAllText(logFilePath, "Timestamp,RayOriginX,RayOriginY,RayOriginZ,RayDirX,RayDirY,RayDirZ,HitPosX,HitPosY,HitPosZ\n");
//        }
//    }

//    void SetupRay()
//    {
//        lineRenderer.useWorldSpace = false;
//        lineRenderer.positionCount = 2;
//        lineRenderer.startWidth = rayWidth;
//        lineRenderer.endWidth = rayWidth;
//        lineRenderer.startColor = rayColorDefaultState;
//        lineRenderer.endColor = rayColorDefaultState;
//        lineRenderer.SetPosition(0, Vector3.zero);
//        lineRenderer.SetPosition(1, Vector3.forward * rayDistance);
//    }

//    // ... rest of the script remains unchanged ...
//}