using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class Stage1CheckPointScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;
    [SerializeField] private GameObject nextChekpoint;
    [SerializeField] private float checkPointDuration; //Destroy Duration
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider pointCollider;
    [SerializeField] private HUDGameManager HUDManager;
    [SerializeField] private AudioSource audioSource;

    //flag
    //GameObject[] enemies;

    void Awake()
    {
        gameMaster = FindFirstObjectByType<GameMaster>();
        HUDManager = FindAnyObjectByType<HUDGameManager>();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
        pointCollider = GetComponent<Collider>();
    }

    public void EnemyChecker()
    {
        int enemies = FindObjectsOfType<EnemyModel>().Length;
        if (enemies >= 1)
        {
            meshRenderer.enabled = false;
            pointCollider.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
            pointCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            meshRenderer.enabled = false;
            gameMaster.QuestText = checkPointInfo;
            Destroy(gameObject, checkPointDuration);
            if (nextChekpoint != null)
            {
                HUDManager.questUIAnim.Play("QuestInfoIn");
            }
            else
            {
                HUDManager.questUIAnim.Play("QuestInfoOut");
            }
        }
    }

    private void OnDestroy()
    {
        if (nextChekpoint != null)
        {
            nextChekpoint.SetActive(true);
        }
        else
        {
            gameMaster.gameFinish = true;
            gameMaster.gameWin = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyChecker();
    }

    public enum CheckPointNumber
    {
        CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6
    }
}
