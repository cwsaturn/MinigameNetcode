using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkTransformTest : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 5f;

    void Update()
    {
        if (IsClient)
        {
            Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
            movementVector = movementVector.normalized * Time.deltaTime * movementSpeed;

            transform.position += movementVector;
        }
    }
}