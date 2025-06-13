using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(LoadNextScene);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("MovingObjectScene");
    }
}