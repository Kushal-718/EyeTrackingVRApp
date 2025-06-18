using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EyeTrackerTest2 : MonoBehaviour
{
    [SerializeField] private float rayDistance = 5.0f;
    [SerializeField] private float rayWidth = 0.02f; // Reduced from 0.1f for narrower ray
    [SerializeField] private LayerMask layersToInclude;
    [SerializeField] private Color rayColorDefaultState = Color.cyan;
    [SerializeField] private Color rayColorHoverState = new Color(0.8f, 0.2f, 1f, 0.8f);
    [SerializeField] private string eyeSide = "Left"; // Set to "Left" or "Right" in Inspector

    private LineRenderer lineRenderer;
    private List<EyeInteractableTeleportingSphere> eyeInteractables = new List<EyeInteractableTeleportingSphere>();
    private string logFilePath;
    private Dictionary<string, float> gazeStartTimes = new Dictionary<string, float>();
    private float currentGazeDuration;
    private string sessionID;
    private float timeToFirstHit = -1f;
    private float sphereAppearTime;
    private Vector3 currentSpherePos;

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

        sessionID = System.Guid.NewGuid().ToString();
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string logFolder = Application.isEditor ? Path.Combine(Application.dataPath, "Logs") : Application.persistentDataPath;
        logFilePath = Path.Combine(logFolder, $"EyeTrackingLog_Test1_{timestamp}.csv");

        try
        {
            Directory.CreateDirectory(logFolder);
            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "SessionID,Timestamp,RayOriginX,RayOriginY,RayOriginZ,RayDirX,RayDirY,RayDirZ,HitPosX,HitPosY,HitPosZ,HitObject,EyeSide,GazeDuration,TimeToFirstHit,SpherePosX,SpherePosY,SpherePosZ\n");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create log file at {logFilePath}: {e.Message}");
        }

        // Subscribe to sphere teleport events
        EyeInteractableTeleportingSphere sphere = FindObjectOfType<EyeInteractableTeleportingSphere>();
        if (sphere != null)
        {
            sphere.OnSphereTeleported += (pos) =>
            {
                sphereAppearTime = Time.time;
                timeToFirstHit = -1f;
                currentSpherePos = pos;
            };
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
            var eyeInteractable = hit.transform.GetComponent<EyeInteractableTeleportingSphere>();
            if (eyeInteractable != null && eyeInteractable.IsVisible())
            {
                Debug.Log($"Hovering {hit.transform.name} with {gameObject.name}");
                if (!eyeInteractables.Contains(eyeInteractable))
                {
                    UnSelect(true);
                    eyeInteractables.Add(eyeInteractable);
                    eyeInteractable.SetHovered(true);
                    string objectName = hit.transform.name;
                    if (!gazeStartTimes.ContainsKey(objectName))
                    {
                        gazeStartTimes[objectName] = Time.time;
                        if (timeToFirstHit < 0f)
                        {
                            timeToFirstHit = Time.time - sphereAppearTime;
                        }
                    }
                }
                currentGazeDuration = Time.time - gazeStartTimes[hit.transform.name];
            }
            else
            {
                Debug.LogWarning($"No EyeInteractableTeleportingSphere or not visible on {hit.transform.name}");
                UnSelect(true);
                currentGazeDuration = 0f;
            }
            Vector3 localHitPos = transform.InverseTransformPoint(hit.point);
            lineRenderer.SetPosition(1, localHitPos);
            LogRayData(transform.position, rayCastDirection, hit.point, true, hit.transform.name, currentGazeDuration, timeToFirstHit);
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            lineRenderer.SetPosition(1, Vector3.forward * rayDistance);
            UnSelect(true);
            currentGazeDuration = 0f;
            LogRayData(transform.position, rayCastDirection, Vector3.zero, false, "None", 0f, timeToFirstHit);
        }
    }

    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            interactable.SetHovered(false);
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

    private void LogRayData(Vector3 origin, Vector3 direction, Vector3 hitPos, bool hit, string hitObject, float gazeDuration, float timeToFirstHit)
    {
        float timestamp = Time.time;
        string hitPosX = hit ? hitPos.x.ToString("F4") : "-1";
        string hitPosY = hit ? hitPos.y.ToString("F4") : "-1";
        string hitPosZ = hit ? hitPos.z.ToString("F4") : "-1";
        string timeToFirstHitStr = timeToFirstHit >= 0f ? timeToFirstHit.ToString("F4") : "-1";
        string line = $"{sessionID},{timestamp:F4},{origin.x:F4},{origin.y:F4},{origin.z:F4},{direction.x:F4},{direction.y:F4},{direction.z:F4},{hitPosX},{hitPosY},{hitPosZ},{hitObject},{eyeSide},{gazeDuration:F4},{timeToFirstHitStr},{currentSpherePos.x:F4},{currentSpherePos.y:F4},{currentSpherePos.z:F4}";
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