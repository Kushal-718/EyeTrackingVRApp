using UnityEngine;
using UnityEngine.Android;

public class GamePermissionManager : MonoBehaviour
{
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("com.oculus.hardware.eyetracking"))
        {
            Permission.RequestUserPermission("com.oculus.hardware.eyetracking");
        }
        if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.SCENE"))
        {
            Permission.RequestUserPermission("com.oculus.permission.SCENE");
        }
    }
}