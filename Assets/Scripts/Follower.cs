using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Follower : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform target;
    private float moveSpeed;
    Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        moveSpeed = 10f;
    }

    private void Update()
    {
        if(target)
        {
            Vector3 direction = (target.position - transform.position). normalized;
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //rb.rotation = angle;
            moveDirection = direction;
        }
    }

    private void FixedUpdate()
    {
        if(target)
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }
}
