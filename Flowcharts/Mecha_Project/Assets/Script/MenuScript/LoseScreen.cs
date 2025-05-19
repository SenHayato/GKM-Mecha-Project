using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    public void BackToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
