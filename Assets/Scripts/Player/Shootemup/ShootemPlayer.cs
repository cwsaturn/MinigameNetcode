using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShootemPlayer : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 100f;

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer playerSprite;

    [SerializeField]
    private Sprite[] spriteArray;

    [SerializeField]
    private Healthbar healthbar;

    [SerializeField]
    private float maxHealth = 100f;
    NetworkVariable<float> health = new NetworkVariable<float>(100f);

    [SerializeField]
    private float timeAlive = 0f;

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Canvas myCanvas;
    [SerializeField]
    private PlayerScript playerScript;
    [SerializeField]
    private NetworkObject networkObject;

    private float shotDistance = 1.5f;
    private float shotSpeed = 10f;

    bool mouseClick = false;

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        health.OnValueChanged += OnHealthChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        health.OnValueChanged -= OnHealthChange;
    }

    public void OnHealthChange(float previous, float current)
    {
        healthbar.UpdateHealth(health.Value / maxHealth);

        if (!IsLocalPlayer) return;

        if (health.Value <= 0f)
        {
            playerScript.FinishedServerRpc(-timeAlive);
        }
    }

    [ServerRpc]
    void SetHealthServerRpc(float newHealth, ServerRpcParams rpcParams = default)
    {
        health.Value = newHealth;
    }

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ulong clientId = networkObject.OwnerClientId;
        if (clientId < 4)
        {
            playerSprite.sprite = spriteArray[clientId];
        }

        if (!IsLocalPlayer) return;
        Camera.main.GetComponent<CameraScript>().setTarget(transform);
        SetHealthServerRpc(maxHealth);
        projectile.GetComponent<Bullet>().type = Bullet.bulletType.cyan;
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        timeAlive += Time.deltaTime;

        Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
        if (movementVector.magnitude > 1)
            movementVector = movementVector.normalized;

        movementVector *= movementSpeed;
        playerRigidbody.AddForce(movementVector, ForceMode2D.Force);

        if (mouseClick)
        {
            //playerRigidbody.transform.LookAt(Input.mousePosition);
            Vector3 midscreenVector = new Vector3(Screen.width, Screen.height, 0f) / 2f;
            Vector3 dir = Input.mousePosition - midscreenVector;
            Debug.Log(dir);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            FireShot();
            mouseClick = false;
        }
    }

    private void Update()
    {
        myCanvas.transform.position = transform.position;

        if (!IsLocalPlayer) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseClick = true;
        }
    }

    public void ShotCollision(float damage)
    {
        if (!IsServer) return;
        health.Value -= damage;
    }

    private void FireShot()
    {
        Vector3 position = playerRigidbody.transform.position;

        Vector3 forwardVector = playerRigidbody.transform.rotation * Vector3.up;

        Vector3 delta = forwardVector * shotDistance;

        //float dotProd = Vector3.Dot(playerRigidbody.velocity, forwardVector);

        Vector3 itemVector = shotSpeed * forwardVector + (Vector3)playerRigidbody.velocity;

        CreateItemServerRpc(position + delta, itemVector);
    }

    [ServerRpc]
    void CreateItemServerRpc(Vector3 position, Vector3 velocity, ServerRpcParams rpcParams = default)
    {
        GameObject shot = Instantiate(projectile, position, Quaternion.identity);
        shot.GetComponent<NetworkObject>().Spawn();
        shot.GetComponent<Bullet>().SetVelocity(velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }


}
