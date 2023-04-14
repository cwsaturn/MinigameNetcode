using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class DefaultPlayer : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 50f;

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer playerSprite;

    [SerializeField]
    private Sprite[] spriteArray;

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //playerSprite = GetComponent<SpriteRenderer>();
        //playerSprite.color = playerColor.Value;

        ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
        if (clientId < 4)
            playerSprite.sprite = spriteArray[clientId];
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