using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{
    private SpriteRenderer sprite;

    [SerializeField]
    private Sprite[] spriteArray;

    private NetworkVariable<bool> on = new NetworkVariable<bool>(true);
    public bool On
    { get { return on.Value; } }

    private float timeLeft = 0f;
    private float restockTime = 2f;

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
            sprite.sprite = spriteArray[0];
            localOn = true;
            return;
        }

        sprite.sprite = spriteArray[1];
        localOn = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerCollisionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        on.Value = false;
        timeLeft = restockTime;
        sprite.sprite = spriteArray[1];
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
        sprite.sprite = spriteArray[0];
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
            sprite.sprite = spriteArray[0];
        }
    }
}
