using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformTest : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;

    private Rigidbody2D playerRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (IsClient)
        {
            Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
            //movementVector = movementVector.normalized * Time.deltaTime * movementSpeed;
            if (movementVector.magnitude > 1)
                movementVector = movementVector.normalized;

            movementVector *= movementSpeed;

            //transform.position += movementVector;
            playerRigidbody.AddForce(movementVector, ForceMode2D.Force);
        }
    }
}