using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlatformerData : NetworkBehaviour
{
    //public Text scoreText;
    private float timePassed = 0f;

    private SpriteRenderer playerSprite;
    private TextMeshProUGUI timeText;
    private PlayerScript playerScript;

    [SerializeField]
    private TextMeshProUGUI scoreText;
    public PlayerScoring playerScoring;

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

    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        playerScript = GetComponent<PlayerScript>();
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
            playerScript.FinishedServerRpc(-timePassed);
            GetComponent<ClientPlatformer>().active = false;
            playerActive = false;
            timeText.text = timePassed.ToString();
        }
    }
}
