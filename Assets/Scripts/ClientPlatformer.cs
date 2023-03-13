using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientPlatformer : NetworkBehaviour
{
    public Text scoreText;

    [SerializeField]
    private TextMeshProUGUI timeText;

    private GameObject finishFlag;
    private ServerScript serverScript;

    [SerializeField]
    private float movementSpeed = 50f;
    [SerializeField]
    private float jumpPower = 50f;
    [SerializeField]
    private float normalGrav = 14f;
    [SerializeField]
    private float reducedGrav = 8f;

    private bool jumpReleased = true;

    private float timePassed = 0f;

    private Vector3 inputVector = Vector3.zero;

    private List<Vector3> normalVectors = new List<Vector3>();

    private Rigidbody2D playerRigidbody;

    private int jumpWait = 0;
    private int jumpWaitFrames = 1;

    private NetworkVariable<Color> playerColorNet = new NetworkVariable<Color>(Color.white);
    public Color PlayerColor
    { get { return playerColorNet.Value; } }

    private SpriteRenderer playerSprite;
    private NetworkVariable<int> playerScoreNet = new NetworkVariable<int>(0);

    public NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);
    public bool PlayerFinished
    { get { return playerFinished.Value; } }

    [ServerRpc]
    void ColorSetServerRpc(float hue, ServerRpcParams rpcParams = default)
    {
        playerColorNet.Value = Color.HSVToRGB(hue, 1f, 1f);
    }

    public void ScoreSetClient(int score)
    {
        FindObjectOfType<UniversalPlayer>().AddToScoreServerRpc(score);
        ScoreSetServerRpc(score);
    }

    [ServerRpc]
    public void ScoreSetServerRpc(int score)
    {
        playerScoreNet.Value = score;
    }

    [ServerRpc]
    void FinishedSetServerRpc(ServerRpcParams rpcParams = default)
    {
        playerFinished.Value = true;
        serverScript.PlayerFinishedServerRpc(NetworkManager.Singleton.LocalClientId);
    }


    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        timeText = FindObjectOfType<TextMeshProUGUI>();
        finishFlag = GameObject.FindGameObjectWithTag("Finish");
        serverScript = FindObjectOfType<ServerScript>();
    }


    private void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);

            Debug.Log("test1");

            float colorValue = Random.Range(0f, 1f);
            ColorSetServerRpc(colorValue);
        }

        Debug.Log("test2");

        if (!IsClient) return;
        playerRigidbody = GetComponent<Rigidbody2D>();
        SyncNetVariables();
    }


    void FixedUpdate()
    {
        if (!IsClient) return;
        Vector3 movementVector = inputVector * movementSpeed;

        playerRigidbody.AddForce(movementVector, ForceMode2D.Force);

        if (normalVectors.Count > 0 && jumpWait <= 0)
        {
            jumpWait = jumpWaitFrames;
        }

        if (jumpWait > 0)
        {
            --jumpWait;

            if (jumpWait <= 0)
            {
                Jump();
                jumpReleased = false;
            }
        }
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
        scoreText.text = playerScoreNet.Value.ToString();
    }

    public void SyncNetVariables()
    {
        scoreText.text = playerScoreNet.Value.ToString();
        playerSprite.color = playerColorNet.Value;
    }


    private void Update()
    {
        if (!IsClient) return;
        inputVector = Vector3.right * Input.GetAxisRaw("Horizontal");

        if (inputVector.magnitude > 1)
            inputVector = inputVector.normalized;

        if (Input.GetButton("Jump"))
        {
            playerRigidbody.gravityScale = reducedGrav;
        }
        else
        {
            playerRigidbody.gravityScale = normalGrav;
            jumpReleased = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (IsOwner)
            {
                //ScoreSetServerRpc();
                ColorSetServerRpc(Random.Range(0f, 1f));
            }
        }
        timePassed += Time.deltaTime;
        timeText.text = timePassed.ToString();
    }

    private void Jump()
    {
        Vector2 averageVector = Vector2.zero;
        foreach (Vector2 normal in normalVectors)
        {
            averageVector += normal;
        }
        normalVectors.Clear();

        averageVector.Normalize();

        //adds a vertical component to wall jumps
        averageVector.y += 1 - Mathf.Abs(averageVector.y);

        playerRigidbody.AddForce(averageVector * jumpPower, ForceMode2D.Impulse);
    }

    private void CollectNormals(Collision2D collision)
    {
        Vector2 averageNormal = Vector3.zero;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (!normalVectors.Contains(contact.normal))
            {
                normalVectors.Add(contact.normal);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsClient) return;
        if (Input.GetButton("Jump") && jumpReleased)
        {
            CollectNormals(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsClient) return;
        if (Input.GetButton("Jump") && jumpReleased)
        {
            CollectNormals(collision);
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