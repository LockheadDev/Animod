using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    public void LoadScene(int num)
    {
        switch (num)
        {
            case 1:
                SceneManager.LoadScene("DemoScene", LoadSceneMode.Single);
                break;
            case 2:
                SceneManager.LoadScene("FireAndWaterScene",LoadSceneMode.Single);
                break;
            default:
                Debug.Log("Scene not found: " + num.ToString());
                break;
        }
    }
}
