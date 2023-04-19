using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEditor.Progress;

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
    private GameObject projectile;
    [SerializeField]
    private Canvas myCanvas;
    [SerializeField]
    private PlayerScript playerScript;

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
            playerScript.FinishedServerRpc(1f);
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
        SetHealthServerRpc(maxHealth);
        //playerSprite = GetComponent<SpriteRenderer>();
        //playerSprite.color = playerColor.Value;

        //ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        if (clientId < 4)
        {
            playerSprite.sprite = spriteArray[clientId];
        }

        if (!IsLocalPlayer) return;
        Camera.main.GetComponent<CameraScript>().setTarget(transform);
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

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

    private void ShotCollision(GameObject shot)
    {
        SetHealthServerRpc(health.Value -= 20f);
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
        shot.GetComponent<Obert>().SetVelocity(velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsLocalPlayer) return;


        if (collision.gameObject.tag == "Obert")
        {
            ShotCollision(collision.gameObject);
        }
    }
}
