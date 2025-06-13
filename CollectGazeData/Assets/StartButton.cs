using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}