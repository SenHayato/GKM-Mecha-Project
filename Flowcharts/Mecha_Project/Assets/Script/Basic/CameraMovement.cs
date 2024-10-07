using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    PlayerInput gameInput;
    InputAction lookAction;

    public Transform playerBody;
    public float camSpeed = 100f;
    public float xRotation = 0f; // Menyimpan rotasi sumbu X untuk mencegah kamera terbalik

    void Start()
    {
        gameInput = GetComponent<PlayerInput>();
        lookAction = gameInput.actions.FindAction("Look");
        //Cursor.lockState = CursorLockMode.Locked; // Mengunci kursor ke tengah layar
        //Cursor.visible = false; // Menyembunyikan kursor
    }

    void Update()
    {
        LookAround();
    }

    void LookAround()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>(); // Membaca input dari arrow keys atau mouse
        float horizontalInput = lookInput.x; // Input horizontal (left/right) dari arrow keys
        float verticalInput = lookInput.y; // Input vertikal (up/down) dari arrow keys

        // Jika input berasal dari arrow keys, sesuaikan kecepatan rotasi
        float mouseX = horizontalInput * camSpeed * Time.deltaTime;
        float mouseY = verticalInput * camSpeed * Time.deltaTime;

        xRotation -= mouseY; // Update rotasi sumbu X
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Mencegah rotasi kamera terbalik

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Rotasi kamera sumbu X
        playerBody.Rotate(Vector3.up * mouseX); // Rotasi tubuh player sumbu Y
    }
}
