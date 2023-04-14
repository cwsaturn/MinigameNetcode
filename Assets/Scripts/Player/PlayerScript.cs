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
    private Collider2D collider;
    [SerializeField]
    private Rigidbody2D rigidbody;

    public PlayerScoring playerScoring;

    private NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);

    public NetworkVariable<bool> disappearOnFinish = new NetworkVariable<bool>(true);

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
            if (disappearOnFinish.Value)
            {
                playerSprite.enabled = false;
                text.enabled = false;
            }
            if (collider != null)
            {
                collider.enabled = false;
            }
            if (rigidbody != null)
            {
                rigidbody.simulated = false;
            }
        }
    }

    public void SetColor(Color color)
    {
        playerSprite.color = color;
    }

    public void SetUsername(string username)
    {
        text.text = username;
    }

    [ServerRpc]
    public void SetDisappearingServerRpc(bool disappear, ServerRpcParams rpcParams = default)
    {
        disappearOnFinish.Value = disappear;
    }

    [ServerRpc]
    public void FinishedServerRpc(float intermediateScore, ServerRpcParams rpcParams = default)
    {
        playerFinished.Value = true;
        if (disappearOnFinish.Value)
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
