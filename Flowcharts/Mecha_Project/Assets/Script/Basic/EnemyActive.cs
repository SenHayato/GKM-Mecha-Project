using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyActive : MonoBehaviour
{
    public GameObject PlayerObj;
    public Transform Player;
    public float speed;   
    public float stoppingDistance;

    private Animator anim;

    public void Start()
    {
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        Player = PlayerObj.GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }
    
    public void EnemyFollow()
    {
        float distance = Vector3.Distance(transform.position, Player.position); // Periksa jarak

        if (distance > stoppingDistance)
        {
            Vector3 direction = (Player.position - transform.position).normalized; // Hitung arah menuju player
            transform.position += speed * Time.deltaTime * direction;
            transform.LookAt(Player); //Mengatur rotasi musuh untuk menghadap ke arah player
            anim.SetFloat("Move", 1f);
        }
        else
        {
            anim.SetFloat("Move", 0f);
        }
    }
    void Update()
    {
        EnemyFollow();
    }
}
