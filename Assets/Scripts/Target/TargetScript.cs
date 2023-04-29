using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TargetScript : NetworkBehaviour
{

    private TargetSpawner targetSpawner; 
    private Rigidbody2D rigidbody2d; 
    private Collider2D colliderObject;
    private float maxSize = 6.0f;
    private float growthRate = 1.0f;
    private float scale = 1.0f; 
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.WakeUp();
        targetSpawner = GameObject.Find("TargetSpawner").GetComponent<TargetSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(scale < maxSize)
        {
            transform.localScale = new Vector3(1.0f,1.0f,1.0f) * scale;
            scale += growthRate * Time.deltaTime * 5;
        }

        if(targetSpawner.finishGame.Value)
        {
            DestroyTargetServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyTargetServerRpc()
    {
        Destroy(gameObject);
    }

}
