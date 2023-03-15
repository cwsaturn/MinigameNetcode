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
    //private ServerScript serverScript;
    //private GameObject finishFlag;
    private UniversalPlayer universalPlayer;

    private NetworkVariable<int> playerScoreNet = new NetworkVariable<int>(0);
    private NetworkVariable<Color> playerColorNet = new NetworkVariable<Color>(Color.white);
    public NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);
    public Color PlayerColor
    { get { return playerColorNet.Value; } }


    [ServerRpc]
    void ColorSetServerRpc(float hue, ServerRpcParams rpcParams = default)
    {
        playerColorNet.Value = Color.HSVToRGB(hue, 1f, 1f);
    }

    [ServerRpc]
    void FinishedSetServerRpc(ServerRpcParams rpcParams = default)
    {
        playerFinished.Value = true;

        int playersFinished = 0;
        bool allFinished = true;

        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            bool finished = player.PlayerObject.GetComponent<PlatformerData>().playerFinished.Value;
            if (finished)
            {
                playersFinished += 1;
                Debug.Log("finished");
            }
            else
            {
                Debug.Log("not finished");
                allFinished = false;
            }
        }

        playerScoreNet.Value = 10 - playersFinished;
        Debug.Log("score: " + (10 - playersFinished));
    }

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        playerColorNet.OnValueChanged += OnColorChange;
        playerScoreNet.OnValueChanged += OnScoreChange;
    }


    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        playerColorNet.OnValueChanged -= OnColorChange;
        playerScoreNet.OnValueChanged -= OnScoreChange;
    }

    public void OnColorChange(Color previous, Color current)
    {
        playerSprite.color = playerColorNet.Value;
    }

    public void OnScoreChange(int previous, int current)
    {
        if (IsOwner)
        {
            Debug.Log("on score change: " + playerScoreNet.Value);
            universalPlayer.SetScorePlatformerServerRpc(playerScoreNet.Value);
        }
        //scoreText.text = playerScoreNet.Value.ToString();
    }

    public void SyncNetVariables()
    {
        //scoreText.text = playerScoreNet.Value.ToString();
        playerSprite.color = playerColorNet.Value;
    }

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        timeText = FindObjectOfType<TextMeshProUGUI>();
        universalPlayer = FindObjectOfType<UniversalPlayer>();
        //finishFlag = GameObject.FindGameObjectWithTag("Finish");
        //serverScript = FindObjectOfType<ServerScript>();
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
                //ScoreSetServerRpc();
                ColorSetServerRpc(Random.Range(0f, 1f));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsClient) return;

        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.tag == "Finish");

        if (collision.gameObject.tag == "Finish")
        {
            Debug.Log(timePassed);
            if (IsOwner)
            {
                FinishedSetServerRpc();
            }

        }
    }
}
