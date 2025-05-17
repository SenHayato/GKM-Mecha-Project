using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [SerializeField] GameObject missileObj;
    [SerializeField] GameObject warningSign;

    [Header("Referensi")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider missileCollider;
    [SerializeField] GameObject missileDamage;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        missileCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        warningSign.SetActive(true);
        missileCollider.enabled = true;
        meshRenderer.enabled = true;
        missileDamage.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        warningSign.SetActive(false);
        missileCollider.enabled = false;
        meshRenderer.enabled = false;
        missileDamage.SetActive(true);
        Destroy(missileObj, 2f);
    }

    private void OnDestroy()
    {
        Debug.Log("Meledak");
    }
}
