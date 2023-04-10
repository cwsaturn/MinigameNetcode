using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private Collider2D collider2d;
    [SerializeField]
    private Rigidbody2D rigidbody2d;
    [SerializeField]
    private bool colored = true;

    public PlayerScoring playerScoring;

    private NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        playerFinished.OnValueChanged += OnFinishChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        playerFinished.OnValueChanged -= OnFinishChange;
    }

    public void OnFinishChange(bool previous, bool current)
    {
        if (playerFinished.Value)
        {
            playerSprite.enabled = false;
            text.enabled = false;
            if (collider2d != null)
            {
                collider2d.enabled = false;
            }
            if (rigidbody2d != null)
            {
                rigidbody2d.simulated = false;
            }
        }
    }

    public void SetColor(Color color)
    {
        if (!colored) return;
        playerSprite.color = color;
    }

    public void SetUsername(string username)
    {
        text.text = username;
    }

    [ServerRpc]
    public void FinishedServerRpc(float intermediateScore, ServerRpcParams rpcParams = default)
    {
        playerFinished.Value = true;
        playerSprite.enabled = false;
        playerScoring.SetPlayerFinished(intermediateScore);
        //FindObjectOfType<ServerScript>().CheckPlayersFinished();
    }

    // Start is called before the first frame update
    void Start()
    {
        //playerSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
