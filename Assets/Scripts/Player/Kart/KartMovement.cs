using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KartMovement : NetworkBehaviour
{
    public bool active = true;
    
    [SerializeField]
    Rigidbody2D kart;

    [SerializeField]
    float linearSpeed = 5f;
    [SerializeField]
    float turningSpeed = 5f;

    private float verticalInput = 0f;
    private float horizontalInput = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsClient) return;
        kart = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsClient) return;

        if (!active)
        {
            verticalInput = 0f; 
            horizontalInput = 0f;
            return;
        }

        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = -Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (!active) return;
        if (!IsClient) return;

        float accelInput = verticalInput * linearSpeed;

        Vector3 kartForce = kart.transform.right * accelInput;

        kart.AddForce(kartForce, ForceMode2D.Force);

        float torqueInput = horizontalInput * turningSpeed;

        kart.AddTorque(torqueInput, ForceMode2D.Force);
    }
}
