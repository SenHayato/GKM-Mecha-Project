using UnityEngine;
using UnityEngine.VFX;

public class TurotialCheckPointScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private CheckPointNumber checkPointNumber;
    [SerializeField] private string checkPointInfo;
    [SerializeField] private GameObject nextChekpoint;
    [SerializeField] private float checkPointDuration; //Destroy Duration
    [SerializeField] private Collider pointCollider;
    [SerializeField] private HUDGameManager HUDManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private GameObject uiMap;
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

    private void Update()
    {
        EnemyChecker();
    }

    public enum CheckPointNumber
    {
        CheckPoint1, CheckPoint2, CheckPoint3, CheckPoint4, CheckPoint5, CheckPoint6, CheckPoint7
    }
}
