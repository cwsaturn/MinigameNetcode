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
    private NetworkVariable<bool> playerScoreDone = new NetworkVariable<bool>(false);
    public bool PlayerScoreDone
    {
        get { return playerScoreDone.Value; }
    }

    private bool active = true;

    public Color PlayerColor
    { get { return playerColorNet.Value; } }


    [ServerRpc]
    void ColorSetServerRpc(float hue, ServerRpcParams rpcParams = default)
    {
        playerColorNet.Value = Color.HSVToRGB(hue, 1f, 1f);
    }

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

    //[ClientRpc]
    public void AddFinalScore(int rank)
    {
        if (!IsServer) return;
        universalPlayer.AddScore(11 - rank);
        //ScoreCompleteServerRpc();
    }
    //GAME FINISHED==========


    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        playerColorNet.OnValueChanged += OnColorChange;
        playerScoreNet.OnValueChanged += OnScoreChange;
        playerFinished.OnValueChanged += OnFinishChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        playerColorNet.OnValueChanged -= OnColorChange;
        playerScoreNet.OnValueChanged -= OnScoreChange;
        playerFinished.OnValueChanged -= OnFinishChange;
    }

    public void OnColorChange(Color previous, Color current)
    {
        playerSprite.color = playerColorNet.Value;
    }

    public void OnScoreChange(int previous, int current)
    {
        scoreText.text = playerScoreNet.Value.ToString();
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
        scoreText.text = playerScoreNet.Value.ToString();
        playerSprite.color = playerColorNet.Value;
    }

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        timeText = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);

            Debug.Log("test1");

            float colorValue = Random.Range(0f, 1f);
            ColorSetServerRpc(colorValue);
        }

        SyncNetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        timeText.text = timePassed.ToString();

        if (Input.GetButtonDown("Fire1"))
        {
            if (IsOwner)
            {
                ColorSetServerRpc(Random.Range(0f, 1f));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsClient) return;

        if (!active) return;

        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.tag == "Finish");

        if (collision.gameObject.tag == "Finish")
        {
            Debug.Log(timePassed);
            if (IsOwner)
            {
                FinishedSetServerRpc(timePassed);
                GetComponent<ClientPlatformer>().active = false;
                active = false;
            }
        }
    }
}
