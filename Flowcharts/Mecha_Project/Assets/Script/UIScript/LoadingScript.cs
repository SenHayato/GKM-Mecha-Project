using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] int progress;

    //flag
    float timesBar;

    void Start()
    {
        progressBar.value = progress;
    }

    void LoadingMonitor()
    {
        progressBar.value = progress;
        for (float a = 0; a <= timesBar; a++)
        {

        }
    }

    void Update()
    {
        LoadingMonitor();
    }
}
