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
            if (playerSprite != null) playerSprite.enabled = false;
            text.enabled = false;
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
    public void FinishedServerRpc(float intermediateScore, ServerRpcParams rpcParams = default)
    {
        playerFinished.Value = true;
        //playerSprite.enabled = false;
        playerScoring.SetPlayerFinished(intermediateScore);
        //FindObjectOfType<ServerScript>().CheckPlayersFinished();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
