using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1TriggerBorder : MonoBehaviour
{
    [SerializeField] GameObject[] enableObjects;
    [SerializeField] bool wasTrigger = false;
    [SerializeField] GameObject checkPoint;

    private void Start()
    {
        wasTrigger = false;
        foreach (var obj in enableObjects)
        {
            obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (checkPoint == null) //jika checkpoint habis
        {
            if (other.CompareTag("Player"))
            {
                if (!wasTrigger)
                {
                    wasTrigger = true;
                    foreach (var obj in enableObjects)
                    {
                        obj.SetActive(true);
                    }
                }
            }
        }
    }
}
