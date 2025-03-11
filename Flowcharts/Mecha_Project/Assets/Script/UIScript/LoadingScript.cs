using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] int progress;

    //flag
     [SerializeField] float timesBar;

    void Start()
    {
        progressBar.value = progress;
    }

    void LoadingMonitor()
    {
        progressBar.value = progress;
    }

    void Update()
    {
        LoadingMonitor();
    }
}
