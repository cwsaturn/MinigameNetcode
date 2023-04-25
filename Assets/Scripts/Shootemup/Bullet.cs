using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private Sprite[] spriteArray;
    [SerializeField]
    private Rigidbody2D rigidbody2d;
    public Rigidbody2D Rigidbody2d
    { get { return rigidbody2d; } }

    SpriteRenderer spriteRenderer;

    public enum bulletType { yellow, red, magenta, green, cyan, blue }

    //public bulletType type = bulletType.yellow;

    public NetworkVariable<bulletType> type = new NetworkVariable<bulletType>(bulletType.yellow);

    private float damage = 20;
    public float Damage
    { get { return damage; } }

    private float collisionHealthDrop = 10f;

    public GameObject owner;

    NetworkVariable<float> health = new NetworkVariable<float>(30f);

    public float lifetime = 0f;

    // Start is called before the first frame update
    /*
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
    */

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetBulletType();
    }

    private void Start()
    {
        SetBulletType();
    }

    private void SetBulletType()
    {
        switch (type.Value)
        {
            case bulletType.red:
                spriteRenderer.sprite = spriteArray[1];
                damage = 40;
                collisionHealthDrop = 30f;
                break;
            case bulletType.magenta:
                spriteRenderer.sprite = spriteArray[2];
                damage = 12;
                transform.localScale = 0.75f * Vector3.one;
                break;
            case bulletType.green:
                spriteRenderer.sprite = spriteArray[3];
                damage = 12;
                collisionHealthDrop = 5;
                break;
            case bulletType.cyan:
                spriteRenderer.sprite = spriteArray[4];
                damage = 10;
                gameObject.layer = 8;
                break;
            case bulletType.blue:
                spriteRenderer.sprite = spriteArray[5];
                damage = 30;
                transform.localScale = 2 * Vector3.one;
                break;
            default:
                break;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        rigidbody2d.velocity = velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsServer) return;
        lifetime += Time.deltaTime;
        health.Value -= Time.deltaTime;
        if (health.Value <= 0f)
            DestroySelf();
    }

    public void DestroySelf()
    {
        if (!IsServer) return;
        Destroy(gameObject);
    }

    [ServerRpc]
    public void destroyServerRpc()
    {
        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        destroyServerRpc();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Hook 'OnSceneLoaded' to Unity's 'SceneManager.sceneLoaded' variable 
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        health.Value -= collisionHealthDrop;
    }
}
