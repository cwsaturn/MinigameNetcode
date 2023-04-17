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

    private bool jumpPressed = false;

    private Vector3 inputVector = Vector3.zero;

    private List<Vector3> normalVectors = new List<Vector3>();

    private List<Collision2D> collisionList = new List<Collision2D>();

    private Rigidbody2D playerRigidbody;

    public bool active = true;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!active) return;
        if (!IsOwner) return;

        CollectNormals();

        Movement();
        
        if (jumpPressed)
        {
            if (collisionList.Count > 0)
            {
                Jump();
                jumpPressed = false;
            }
        }
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (!active)
        {
            inputVector = Vector3.zero;
            return;
        }

        inputVector = Vector3.right * Input.GetAxisRaw("Horizontal");

        if (inputVector.magnitude > 1)
            inputVector = inputVector.normalized;

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }

        if (Input.GetButton("Jump"))
        {
            playerRigidbody.gravityScale = reducedGrav;
        }
        else
        {
            playerRigidbody.gravityScale = normalGrav;
            jumpPressed = false;
        }

        //If on ground reduce gravity
        if (collisionList.Count > 0)
        {
            playerRigidbody.gravityScale = reducedGrav;
        }
    }

    private void Movement()
    {
        Vector3 movementVector = inputVector * movementSpeed;

        if (collisionList.Count == 0)
        {
            //In air
            playerRigidbody.AddForce(movementVector, ForceMode2D.Force);
            return;
        }

        //On ground or slope
        //Find the vector in direction of slope, add a portion of that vector to movement
        Vector3 maxYNormal = Vector3.down;
        foreach (Vector3 normal in normalVectors)
        {
            if (normal.y > maxYNormal.y)
            {
                maxYNormal = normal;
            }
        }

        float dotProduct = Vector3.Dot(maxYNormal, movementVector);
        Vector3 slopeVector = movementVector - maxYNormal * dotProduct;
        //Debug.DrawRay(playerRigidbody.position, slopeVector, Color.red, 0.2f);

        float steepness = Mathf.Max(maxYNormal.y, 0);

        Vector3 resultVelocity = slopeVector * steepness + movementVector * (1 - steepness);
        //Debug.DrawRay(playerRigidbody.position, resultVelocity, Color.cyan, 0.2f);
        playerRigidbody.AddForce(resultVelocity, ForceMode2D.Force);
    }

    private void Jump()
    {
        Vector2 averageVector = Vector2.zero;
        foreach (Vector2 normal in normalVectors)
        {
            averageVector += normal;
        }
        //normalVectors.Clear();

        averageVector.Normalize();

        //adds a vertical component to wall jumps
        averageVector.y += 1 - Mathf.Abs(averageVector.y);

        playerRigidbody.AddForce(averageVector * jumpPower, ForceMode2D.Impulse);
    }

    private void CollectNormals()
    {
        normalVectors.Clear();
        Vector2 averageNormal = Vector3.zero;

        foreach (Collision2D collision in collisionList)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (!normalVectors.Contains(contact.normal))
                {
                    normalVectors.Add(contact.normal);
                }
            }
        }
    }

    private void RemoveCollision(Collision2D collision)
    {
        for (int i = 0; i < collisionList.Count; i++)
        {
            if (collisionList[i].gameObject == collision.gameObject)
            {
                collisionList.RemoveAt(i);
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner) return;
        collisionList.Add(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsOwner) return;
        RemoveCollision(collision);
        collisionList.Add(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsOwner) return;
        RemoveCollision(collision);
    }
}