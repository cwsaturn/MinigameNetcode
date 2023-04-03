using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlatformerData : NetworkBehaviour
{
    //public Text scoreText;
    private float timePassed = 0f;

    private SpriteRenderer playerSprite;
    private TextMeshProUGUI timeText;

    [SerializeField]
    private TextMeshProUGUI scoreText;
    //private ServerScript serverScript;
    //private GameObject finishFlag;
    public UniversalPlayer universalPlayer;

    private NetworkVariable<int> playerScoreNet = new NetworkVariable<int>(0);
    private NetworkVariable<float> finalTimeNet = new NetworkVariable<float>(0f);
    public float FinalTime
    {
        get { return finalTimeNet.Value; }
    }
    private NetworkVariable<Color> playerColorNet = new NetworkVariable<Color>(Color.white);
    private NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);
    public bool PlayerFinished
    {
        get { return playerFinished.Value; }
    }

    private bool playerActive = true;

    public Color PlayerColor
    { get { return playerColorNet.Value; } }


    public void ColorSet(Color color)
    {
        if (!IsServer) return;
        playerColorNet.Value = color;
    }

    //Set by universalPlayer
    [ServerRpc(RequireOwnership = false)]
    public void ScoreSetServerRpc(int score, ServerRpcParams rpcParams = default)
    {
        playerScoreNet.Value = score;
    }


    //GAME FINISHED==========
    [ServerRpc]
    void FinishedSetServerRpc(float finalTime, ServerRpcParams rpcParams = default)
    {
        finalTimeNet.Value = finalTime;
        playerFinished.Value = true;
        playerSprite.enabled = false;
        FindObjectOfType<ServerScript>().CheckPlayersFinished();
    }

    //only call from server
    public void AddFinalScore(int rank, int players)
    {
        if (!IsServer) return;

        if (players <= 0) return;

        int score = 10 * (players - rank + 1) / players;
        universalPlayer.AddScore(score);
        //ScoreCompleteServerRpc();
    }
    //GAME FINISHED==========


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
        }
    }

    public void SyncNetVariables()
    {
        //scoreText.text = playerScoreNet.Value.ToString();
        //playerSprite.color = playerColorNet.Value;
    }

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);
            timeText = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
        }

        //SyncNetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;

        timePassed += Time.deltaTime;

        if (IsOwner)
            timeText.text = timePassed.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        if (!playerActive) return;

        if (collision.gameObject.tag == "Finish")
        {
            FinishedSetServerRpc(timePassed);
            GetComponent<ClientPlatformer>().active = false;
            playerActive = false;
            timeText.text = timePassed.ToString();
        }
    }
}
