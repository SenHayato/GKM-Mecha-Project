using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeningTime : MonoBehaviour
{
    public void TimeStop()
    {
        Time.timeScale = 0f;
    }

    public void TimeContinue()
    {
        Time.timeScale = 1f;
    }
}
