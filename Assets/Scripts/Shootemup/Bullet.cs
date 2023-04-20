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

    public bulletType type = bulletType.yellow;

    private float damage = 10;
    public float Damage
    { get { return damage; } }

    float lifetime = 30f;
    NetworkVariable<float> health = new NetworkVariable<float>(30f);

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case bulletType.red:
                spriteRenderer.sprite = spriteArray[1];
                damage = 20;
                break;
            case bulletType.magenta:
                spriteRenderer.sprite = spriteArray[2];
                break;
            case bulletType.green:
                spriteRenderer.sprite = spriteArray[3];
                damage = 5;
                break;
            case bulletType.cyan:
                spriteRenderer.sprite = spriteArray[4];
                damage = 5;
                gameObject.layer = 8;
                break;
            case bulletType.blue:
                spriteRenderer.sprite = spriteArray[5];
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
}
