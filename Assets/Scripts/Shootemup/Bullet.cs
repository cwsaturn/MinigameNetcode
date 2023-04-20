using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private Sprite[] spriteArray;

    SpriteRenderer spriteRenderer;

    public enum bulletType { yellow, red, magenta, green, cyan, blue }

    public bulletType type = bulletType.yellow;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case bulletType.red:
                spriteRenderer.sprite = spriteArray[1];
                break;
            case bulletType.magenta:
                spriteRenderer.sprite = spriteArray[2];
                break;
            case bulletType.green:
                spriteRenderer.sprite = spriteArray[3];
                break;
            case bulletType.cyan:
                spriteRenderer.sprite = spriteArray[4];
                break;
            case bulletType.blue:
                spriteRenderer.sprite = spriteArray[5];
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    private void destroyServerRpc(ServerRpcParams rpcParams = default)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.tag == "Tank")
        {
            Debug.Log("hit player");
            destroyServerRpc();
        }
    }
}
