using System;
using System.Collections; 
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
    public int totalTargets = 20; 

    [SerializeField]
    private TargetSpawner targetSpawner; 
    private bool triggered = false; 
    private Collider2D colliderObject; 

    private Color originalColor; 

    private TargetPlayerScore targetPlayerScore; 

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        if(IsServer)
            targetSpawner = GameObject.Find("TargetSpawner").GetComponent<TargetSpawner>();
        originalColor = playerSprite.color; 
        targetPlayerScore = GameObject.Find("Canvas/Score").GetComponent<TargetPlayerScore>();
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

        if (Input.GetButtonDown("Jump"))
        {
            if(!IsLocalPlayer) return; 
            CrosshairShootEffectServerRpc();
            if(triggered)
            {
                triggered = false; 
                colliderObject.gameObject.GetComponent<TargetScript>().DestroyTargetServerRpc();
                score++;
                targetPlayerScore.UpdateScoreOnUI(score); 
                UpdateScoreOnTargetSpawnerServerRpc();
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return; 
        Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
        movementVector = movementVector.normalized * Time.deltaTime * movementSpeed;

        transform.position += movementVector;
        
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

    private IEnumerator CrosshairShootEffect()
    {
        playerSprite.color = Color.white; 
        yield return new WaitForSeconds(0.05f);
        playerSprite.color = originalColor;
    }

    [ServerRpc]
    public void CrosshairShootEffectServerRpc()
    {
        StartCoroutine(CrosshairShootEffect());
        CrosshairShootEffectClientRpc();
    }

    [ClientRpc]
    public void CrosshairShootEffectClientRpc()
    {
        StartCoroutine(CrosshairShootEffect());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        colliderObject = col; 
        triggered = true; 
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        colliderObject = null; 
        triggered = false; 
    }
}