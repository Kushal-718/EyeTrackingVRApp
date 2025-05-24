using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Meta.XR.EyeTracking;

public class EyeTrackingLogger : MonoBehaviour
{
    public GameObject gazeSphere;
    public float distance = 2f;
    public bool debugRay = true;

    private string logFilePath;
    private List<string> logData = new List<string>();

    void Start()
    {
        // File path to save the CSV
        logFilePath = Path.Combine(Application.persistentDataPath, "EyeTrackingLog.csv");
        logData.Add("Timestamp,Confidence,Origin.x,Origin.y,Origin.z,Dir.x,Dir.y,Dir.z,HitPoint.x,HitPoint.y,HitPoint.z");
    }

    void Update()
    {
        var gaze = EyeGaze;

        if (gaze.Confidence > 0.7f)
        {
            Vector3 origin = gaze.Origin;
            Vector3 direction = gaze.Direction.normalized;
            Vector3 hitPoint = origin + direction * distance;

            // Move the sphere
            if (gazeSphere != null)
                gazeSphere.transform.position = hitPoint;

            // Draw debug ray
            if (debugRay)
                Debug.DrawRay(origin, direction * distance, Color.green);

            // Log the data
            string entry = $"{Time.time},{gaze.Confidence},{origin.x},{origin.y},{origin.z},{direction.x},{direction.y},{direction.z},{hitPoint.x},{hitPoint.y},{hitPoint.z}";
            logData.Add(entry);
        }
    }

    private void OnApplicationQuit()
    {
        File.WriteAllLines(logFilePath, logData);
        Debug.Log("Eye tracking log saved to: " + logFilePath);
    }
}
