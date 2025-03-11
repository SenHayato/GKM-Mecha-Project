using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] int progress;

    //flag
    float progressFloat;

    void Start()
    {
        progressBar.value = progress;
    }

    void LoadingMonitor()
    {
        progressBar.value = progress;
        //progress += 1 * Time.deltaTime;
    }

    void Update()
    {
        LoadingMonitor();
    }
}
