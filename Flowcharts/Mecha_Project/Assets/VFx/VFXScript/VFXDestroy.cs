using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXDestroy : MonoBehaviour
{
    [SerializeField] float effectDuration;

    void Update()
    {
        Destroy(gameObject, effectDuration);
    }
}
