using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientPlatformer : NetworkBehaviour
{
    //public Text scoreText;

    //[SerializeField]
    //private TextMeshProUGUI timeText;

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
    private int jumpWaitFrames = 1;
    public bool active = true;

    private void Start()
    {
        Debug.Log("test2");

        if (!IsClient) return;
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!active) return;
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

        if (!active)
        {
            inputVector = Vector3.zero;
            return;
        }

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