using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetCrosshair : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 50f;

    [SerializeField]
    private PlayerScript playerScript; 

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer playerSprite;

    [SerializeField]
    private int score = 0; 
    public int totalTargets = 5; 

    [SerializeField]
    private TargetSpawner targetSpawner; 

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        if(IsServer)
            targetSpawner = GameObject.Find("TargetSpawner").GetComponent<TargetSpawner>();
    }


    private void Update()
    {
        if(IsServer)
        {
            if(targetSpawner.finishGame.Value)
            {
                FinishGameClientRpc();
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; 
        Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
        //movementVector = movementVector.normalized * Time.deltaTime * movementSpeed;
        if (movementVector.magnitude > 1)
            movementVector = movementVector.normalized;

        movementVector *= movementSpeed;
        //transform.position += movementVector;
        playerRigidbody.AddForce(movementVector, ForceMode2D.Force);
        
    }

    [ClientRpc]
    public void UpdateTargetCrosshairScoreClientRpc(ulong passed_ownerClientId)
    {
        if(!IsOwner) return; 
        score++; 
        UpdateScoreOnTargetSpawnerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreOnTargetSpawnerServerRpc()
    {
        targetSpawner.UpdateHitTargetsServerRpc();
    }


    [ClientRpc]
    public void FinishGameClientRpc()
    {
        if(!IsOwner) return; 
        playerScript.FinishedServerRpc(score * -1);
        
    }

}