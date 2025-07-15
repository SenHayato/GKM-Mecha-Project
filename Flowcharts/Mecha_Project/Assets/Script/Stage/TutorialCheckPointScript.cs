using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class TurotialCheckPointScript : MonoBehaviour
{
    [Header("Attribut")]
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;
    [SerializeField] private GameObject nextChekpoint;
    [SerializeField] private float checkPointDuration; //Destroy Duration
    [SerializeField] private Collider pointCollider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private GameObject uiMap;

    [Header("Reference")]
    [SerializeField] HUDGameManager HUDManager;

    [Header("CheckPoint Load")]
    [SerializeField] TMP_SpriteAsset spriteAsset;
    //[SerializeField] TextMeshProUGUI questText;
    //[SerializeField] private Material pointMaterial;

    //Checker
    //GameObject[] enemies;

    private void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
        HUDManager = FindAnyObjectByType<HUDGameManager>();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pointCollider = GetComponent<Collider>();
        visualEffect = GetComponent<VisualEffect>();
    }

    public void EnemyChecker()
    {
        int enemies = FindObjectsOfType<EnemyModel>().Length;
        if (enemies >= 1)
        {
            visualEffect.enabled = false;
            pointCollider.enabled = false;
        }
        else
        {
            visualEffect.enabled = true;
            pointCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            visualEffect.enabled = false;
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
    
    //void CheckPointText()
    //{
    //    switch (checkPointNumber)
    //    {
    //        case CheckPointNumber.CheckPoint1:
    //            questText.spriteAsset = spriteAsset;
    //            questText.text = "Use <spriteAsset name=\"L3\"> to move";
    //            break;
    //        case CheckPointNumber.CheckPoint2:
    //            questText.spriteAsset = spriteAsset;
    //            questText.text = "Use <spriteAsset name=\"L3\"> to move";
    //            break;
    //    }
    //}

    private void Update()
    {
        if (visualEffect.enabled)
        {
            uiMap.SetActive(true);
        }
        else
        {
            uiMap.SetActive(false);
        }

        EnemyChecker();
    }

    public enum CheckPointNumber
    {
        CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6, CheckPoint7
    }
}
