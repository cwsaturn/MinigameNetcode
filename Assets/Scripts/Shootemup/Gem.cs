using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Gem : NetworkBehaviour
{
    private SpriteRenderer sprite;

    [SerializeField]
    private Sprite[] onSprites;

    [SerializeField]
    private Sprite[] offSprites;

    [SerializeField]
    private int gemtype = 0;

    private Bullet.bulletType bulletType;
    public Bullet.bulletType BulletType
    { get { return bulletType; } }

    private NetworkVariable<bool> on = new NetworkVariable<bool>(true);
    public bool On
    { get { return on.Value; } }

    private float timeLeft = 0f;
    private float restockTime = 10f;

    public bool localOn = true;


    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        on.OnValueChanged += OnBoxChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        on.OnValueChanged -= OnBoxChange;
    }

    public void OnBoxChange(bool previous, bool current)
    {
        if (current)
        {
            sprite.sprite = onSprites[gemtype];
            localOn = true;
            return;
        }

        sprite.sprite = offSprites[gemtype];
        localOn = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerCollisionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        on.Value = false;
        timeLeft = restockTime;
        sprite.sprite = offSprites[gemtype];
    }

    public void PlayerCollision()
    {
        localOn = false;
        PlayerCollisionServerRpc();
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = onSprites[gemtype];

        switch (gemtype)
        {
            case 0:
                bulletType = Bullet.bulletType.yellow;
                break;
            case 1:
                bulletType = Bullet.bulletType.red;
                break;
            case 2:
                bulletType = Bullet.bulletType.magenta;
                break;
            case 3:
                bulletType = Bullet.bulletType.green;
                break;
            case 4:
                bulletType = Bullet.bulletType.cyan;
                break;
            case 5:
                bulletType = Bullet.bulletType.blue;
                break;
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsServer) return;

        if (on.Value) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            on.Value = true;
            sprite.sprite = onSprites[gemtype];
        }
    }
}
