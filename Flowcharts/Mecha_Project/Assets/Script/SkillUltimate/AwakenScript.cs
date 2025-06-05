using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakenScript : MonoBehaviour
{
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] Animation anim;

    bool isAwaken = false;

    private void Start()
    {
        mechaPlayer = GetComponentInParent<MechaPlayer>();
    }

    void AwakeningLerp()
    {
        if (mechaPlayer.UsingAwakening && !isAwaken)
        {
            isAwaken = true;
            anim.Play("AwakeningMaterialFadeIn");
        }

        else if (!mechaPlayer.UsingAwakening && isAwaken)
        {
            isAwaken = false;
            anim.Play("AwakeningMaterialFadeOut");
        }
    }

    private void Update()
    {
        AwakeningLerp();
    }
}
