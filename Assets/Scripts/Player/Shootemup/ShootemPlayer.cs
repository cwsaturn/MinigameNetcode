using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
    NetworkVariable<bool> invincibility = new NetworkVariable<bool>(true);
    private float invincibleTime = 5f;

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
    private float fireRate = 0.5f;
    private float defaultShotSpeed = 10f;
    private float defaultFireRate = 0.5f;
    private Bullet.bulletType bulletType = Bullet.bulletType.yellow;
    
    private float shotTimer = 0f;

    private bool mouseClick = false;
    private bool playerActive = true;

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        health.OnValueChanged += OnHealthChange;
        invincibility.OnValueChanged += OnInvincibilityChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        health.OnValueChanged -= OnHealthChange;
        invincibility.OnValueChanged -= OnInvincibilityChange;
    }

    private void OnHealthChange(float previous, float current)
    {
        healthbar.UpdateHealth(health.Value / maxHealth);

        if (!IsLocalPlayer) return;

        if (health.Value <= 0f)
        {
            playerActive = false;
            playerScript.FinishedServerRpc(-timeAlive);
        }
    }

    private void OnInvincibilityChange(bool previous, bool currnet)
    {
        playerSprite.color = Color.white;
    }

    [ServerRpc]
    void SetHealthServerRpc(float newHealth, ServerRpcParams rpcParams = default)
    {
        health.Value = newHealth;
    }

    [ServerRpc]
    void TurnVincibleServerRpc(ServerRpcParams rpcParams = default)
    {
        invincibility.Value = false;
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
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer) return;
        if (!playerActive) return;

        timeAlive += Time.deltaTime;
        shotTimer -= Time.deltaTime;

        if (timeAlive > invincibleTime)
        {
            TurnVincibleServerRpc();
        }

        Vector3 movementVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
        if (movementVector.magnitude > 1)
            movementVector = movementVector.normalized;

        movementVector *= movementSpeed;
        playerRigidbody.AddForce(movementVector, ForceMode2D.Force);

        if (mouseClick && shotTimer <= 0f)
        {
            //playerRigidbody.transform.LookAt(Input.mousePosition);
            Vector3 midscreenVector = new Vector3(Screen.width, Screen.height, 0f) / 2f;
            Vector3 dir = Input.mousePosition - midscreenVector;
            Debug.Log(dir);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            FireShot();
            shotTimer = fireRate;
        }
    }

    private void Update()
    {
        myCanvas.transform.position = transform.position;

        if (!IsLocalPlayer) return;

        if (Input.GetMouseButton(0))
        {
            mouseClick = true;
        }
        else
        {
            mouseClick = false;
        }
    }

    public void ShotCollision(float damage)
    {
        if (!IsServer) return;
        if (invincibility.Value) return;
        health.Value -= damage;
    }

    private void FireShot()
    {
        Vector3 position = playerRigidbody.transform.position;

        Vector3 forwardVector = playerRigidbody.transform.rotation * Vector3.up;

        Vector3 delta = forwardVector * shotDistance;

        //float dotProd = Vector3.Dot(playerRigidbody.velocity, forwardVector);

        Vector3 itemVector = shotSpeed * forwardVector + (Vector3)playerRigidbody.velocity;

        CreateItemServerRpc(bulletType, position + delta, itemVector);
    }

    [ServerRpc]
    void CreateItemServerRpc(Bullet.bulletType sentType, Vector3 position, Vector3 velocity, ServerRpcParams rpcParams = default)
    {
        projectile.GetComponent<Bullet>().type.Value = sentType;
        projectile.GetComponent<Bullet>().owner = gameObject;

        GameObject shot = Instantiate(projectile, position, Quaternion.identity);
        shot.GetComponent<NetworkObject>().Spawn();
        shot.GetComponent<Bullet>().SetVelocity(velocity);
    }

    private void itemBoxCollision(Collider2D collision)
    {
        Gem gem = collision.gameObject.GetComponent<Gem>();
        if (gem.localOn)
        {
            gem.PlayerCollision();
            //projectile.GetComponent<Bullet>().type = gem.BulletType;
            SetShotTypeVariables(gem.BulletType);
        }
    }

    private void SetShotTypeVariables(Bullet.bulletType type)
    {
        fireRate = defaultFireRate;
        shotSpeed = defaultShotSpeed;
        bulletType = type;

        switch (type)
        {
            case Bullet.bulletType.yellow:
                break;
            case Bullet.bulletType.red:
                fireRate = defaultFireRate * 1.5f;
                break;
            case Bullet.bulletType.magenta:
                fireRate = defaultFireRate / 1.5f;
                break;
            case Bullet.bulletType.green:
                break;
            case Bullet.bulletType.cyan:
                break;
            case Bullet.bulletType.blue:
                shotSpeed = defaultShotSpeed / 1.2f;
                fireRate = defaultFireRate * 1.2f;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLocalPlayer) return;

        if (collision.gameObject.tag == "ItemBox")
        {
            itemBoxCollision(collision);
        }
    }


}
