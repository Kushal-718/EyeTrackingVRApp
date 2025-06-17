using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GazeButton : MonoBehaviour
{
    private float gazeTimer = 0f;
    private float gazeDuration = 2f; // 2 seconds to select
    private bool isGazed = false;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (isGazed)
        {
            gazeTimer += Time.deltaTime;
            if (gazeTimer >= gazeDuration)
            {
                button.onClick.Invoke();
                gazeTimer = 0f;
            }
        }
        else
        {
            gazeTimer = 0f;
        }
    }

    public void OnGazeEnter()
    {
        isGazed = true;
    }

    public void OnGazeExit()
    {
        isGazed = false;
    }

    public void OnButtonSelected()
    {
        Debug.Log("Button selected! Loading MainScene...");
        SceneManager.LoadScene("MainScene_2");
    }
}