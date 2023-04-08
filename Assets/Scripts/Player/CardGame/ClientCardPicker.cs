using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientCardPicker : NetworkBehaviour
{
    //public Text scoreText;

    //[SerializeField]
    //private TextMeshProUGUI timeText;

    [SerializeField]
    private float movementSpeed = 50f;

    private Vector3 inputVector = Vector3.zero;

    private List<Vector3> normalVectors = new List<Vector3>();

    private Rigidbody2D playerRigidbody;

    public bool active = true;

    private void Start()
    {
        if (!IsClient) return;
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!active) return;
        if (!IsClient) return;
        Vector3 movementVector = inputVector * movementSpeed;

        playerRigidbody.AddForce(movementVector, ForceMode2D.Force);
    }


    private void Update()
    {
        if (!IsClient) return;

        if (!active)
        {
            inputVector = Vector3.zero;
            return;
        }

        inputVector = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");

        if (inputVector.magnitude > 1)
            inputVector = inputVector.normalized;

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
}