using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Obert : NetworkBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite sadObert;
    [SerializeField]
    Rigidbody2D rigidbody2d;
    float lifetime = 30f;
    NetworkVariable<float> health = new NetworkVariable<float>(30f);
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float redColor = Mathf.Max(0f, Mathf.Pow(health.Value / lifetime, 0.5f));

        if (health.Value < lifetime / 3f)
            spriteRenderer.sprite = sadObert;

        spriteRenderer.color = new Color(1f, redColor, redColor, Mathf.Min(redColor + 0.5f, 1f));

        if (health.Value <= 0f)
            destroyServerRpc();
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        health.Value -= Time.deltaTime;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rigidbody2d.velocity = velocity;
    }

    [ServerRpc(RequireOwnership = false)]
    public void playerCollisionServerRpc(ServerRpcParams rpcParams = default)
    {
        health.Value -= lifetime / 4f;
    }

    [ServerRpc]
    private void destroyServerRpc(ServerRpcParams rpcParams = default)
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
