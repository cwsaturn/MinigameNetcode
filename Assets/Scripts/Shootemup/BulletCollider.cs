using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletCollider : NetworkBehaviour
{
    [SerializeField]
    Bullet bulletData;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = bulletData.Rigidbody2d.velocity;
        rb.position = bulletData.Rigidbody2d.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided with: " + collision.gameObject.name);

        if (!IsServer) return;

        GameObject obj = collision.gameObject;

        if (obj.tag == "Tank")
        {
            if (bulletData.owner != obj || bulletData.lifetime > 1.5f)
            {
                obj.GetComponent<ShootemPlayer>().ShotCollision(bulletData.Damage);
                bulletData.DestroySelf();
            }
        }
    }
}