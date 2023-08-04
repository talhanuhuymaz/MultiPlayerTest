using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class OtherController : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Yön tuşlarıyla hareket edildiğinde karakterin yönünü değiştirme
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
}