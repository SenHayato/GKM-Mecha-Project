using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlashObj : MonoBehaviour
{
    //Object akan diduplicate bukan di instantiate
    [Header("Referensi")]
    [SerializeField] CharacterController charController;
    [SerializeField] float speed;
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] float destroyTime;

    [Header("Ground Slash Attribut")]
    [SerializeField] int damageValue;
    [SerializeField] GameObject slashHitEffect;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;



    // Start is called before the first frame update

    void Start()
    {
        damageValue += enemyModel.attackPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPost = other.transform.position;
        hitPost.y = 1.2f;
        Instantiate(slashHitEffect, hitPost, Quaternion.identity);
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(damageValue);
            }
        }
    }

    void MoveForward()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.1f, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        Vector3 move = transform.forward * speed;
        velocity.y += gravity * Time.deltaTime;
        Vector3 totalMove = move * Time.deltaTime + velocity * Time.deltaTime;
        charController.Move(totalMove);
    }
        
    private void Update()
    {
        MoveForward();
        Destroy(gameObject, destroyTime);
    }
}
