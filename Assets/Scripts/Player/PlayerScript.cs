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
    public Rigidbody2D Rigidbody2D
    { get { return rigidbody2d; } }
    [SerializeField]
    private bool colored = true;

    //In order to spectate, player must have a rigidbody set in PlayerScript
    [SerializeField]
    private bool spectatorGame = false;

    private bool spectating = false;
    private int playerSpectating = 0;
    private PlayerScript playerSpectatingObj = null;

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
                spectating = true;
                SwitchPlayerSpectating();
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

        //FindObjectOfType<ServerScript>().CheckPlayersFinished();
    }

    public void SwitchPlayerSpectating()
    {
        if (!IsLocalPlayer) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int previousSpectating = playerSpectating;
        playerSpectating = (playerSpectating + 1) % players.Length;

        while (previousSpectating != playerSpectating)
        {
            GameObject player = players[playerSpectating];
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (!playerScript.PlayerFinished)
            {
                Debug.Log("player found: " + playerSpectating);
                Transform playerTransform = playerScript.Rigidbody2D.transform;
                Camera.main.GetComponent<CameraScript>().setTarget(playerTransform);
                playerSpectatingObj = playerScript;
                return;
            }
            playerSpectating = (playerSpectating + 1) % players.Length;
        }
        playerSpectating = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        //playerSprite = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (!spectating) return;
        if (playerSpectating < 0) return;

        if (playerSpectatingObj == null)
        {
            SwitchPlayerSpectating();
            return;
        }
        
        if (playerSpectatingObj.PlayerFinished || Input.GetButtonDown("Jump"))
        {
            SwitchPlayerSpectating();
        }
    }
}
