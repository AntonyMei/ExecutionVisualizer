using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaptionScene : MonoBehaviour
{
    public string Q20SceneName;
    public string Q65SceneName;

    public void LoadQ20()
    {
        SceneManager.LoadScene(Q20SceneName);
    }
    public void LoadQ65()
    {
        SceneManager.LoadScene(Q65SceneName);
    }
}
