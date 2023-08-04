using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        // Only allow local player input if it's the owner of this object
        if (IsLocalPlayer)
        {
            MovePlayerLocally();
        }
    }

    private void MovePlayerLocally()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = +1f;
        }

        // Apply the movement locally client-side
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // Send the local player's movement input to the server
        MovePlayerServerRpc(moveDir);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 moveDir)
    {

        if (!IsServer) return;

        // Apply the movement received from the client to all clients 
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

}