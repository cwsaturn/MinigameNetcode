using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientPlatformer : NetworkBehaviour
{
    [SerializeField]
    private float movementSpeed = 50f;
    [SerializeField]
    private float jumpPower = 50f;
    [SerializeField]
    private float normalGrav = 14f;
    [SerializeField]
    private float reducedGrav = 8f;

    private bool jumpReleased = true;

    private Vector3 inputVector = Vector3.zero;

    private List<Vector3> normalVectors = new List<Vector3>();

    private Rigidbody2D playerRigidbody;

    private int jumpWait = 0;
    private int jumpWaitFrames = 2;

    private NetworkVariable<Color> playerColorNet = new NetworkVariable<Color>(Color.white);
    private SpriteRenderer playerSprite;

    [ServerRpc]
    void ColorSetServerRpc(float hue, ServerRpcParams rpcParams = default)
    {
        playerColorNet.Value = Color.HSVToRGB(hue, 1f, 1f);
    }


    private void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);

            float colorValue = Random.Range(0f, 1f);
            ColorSetServerRpc(colorValue);
        }


        if (!IsClient) return;
        playerRigidbody = GetComponent<Rigidbody2D>();

        //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.color = playerColor.Value;
    }

    void FixedUpdate()
    {
        playerSprite.color = playerColorNet.Value;

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
            ColorSetServerRpc(Random.Range(0f, 1f));
        }
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
}