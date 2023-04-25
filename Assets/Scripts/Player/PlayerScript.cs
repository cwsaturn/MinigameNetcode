using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
    public Rigidbody2D Rigidbody2D
    { get { return rigidbody2d; } }
    [SerializeField]
    private bool preColored = false;

    //In order to spectate, player must have a rigidbody set in PlayerScript
    [SerializeField]
    private bool spectatorGame = false;
    private Spectator spectatorScript = new();

    public Image hat;
    public GameObject follower;

    public PlayerScoring playerScoring;

    private NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);
    public bool PlayerFinished
    { get { return playerFinished.Value; } }

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
            if (spectatorGame)
            {
                spectatorScript.spectating = true;
            }
        }
    }

    public void SetColor(Color color)
    {
        if (preColored) return;
        playerSprite.color = color;
    }

    public void SetUsername(string username)
    {
        text.text = username;
    }

    public void SetCosmetics(float xp)
    {
        if(hat != null)
        {
            if(PlayerPrefs.GetFloat("xp", 0) >= 200)
            {
                hat.enabled = true;
            }
            else
            {
                hat.enabled = false;
            }
        }
        if(follower != null)
        {
            if(PlayerPrefs.GetFloat("xp", 0) >= 300)
            {
                follower.SetActive(true);
            }
            else
            {
                follower.SetActive(false);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
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
    }

    public void ServerFinish(float intermediateScore)
    {
        if (!IsServer) return;
        playerFinished.Value = true;
        if (disappearOnFinish.Value)
            playerSprite.enabled = false;
        playerScoring.SetPlayerFinished(intermediateScore);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;
        
        if (spectatorGame)
        {
            spectatorScript.Update();
        }
    }
}
