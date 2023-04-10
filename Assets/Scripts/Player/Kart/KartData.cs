using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class KartData : NetworkBehaviour
{
    private float timePassed = 0f;

    private int previousCheckpoint = 0;
    private int progress = 0;

    private int checkpointNum = 3;
    private int laps = 3;

    float itemDistance = 1.7f;
    float itemSpeed = 10f;

    private TextMeshProUGUI timeText;
    private TextMeshProUGUI lapText;
    private Image itemUI;
    [SerializeField]
    private PlayerScript playerScript;
    [SerializeField]
    private Rigidbody2D rigidbody;

    [SerializeField]
    GameObject canvas;

    private bool playerActive = true;

    [SerializeField]
    private GameObject blockingItem;

    private bool hasItem = false;
    private bool buttonPressed = false;
    private float itemWait = 2f;
    private float itemTime = 0f;

    [SerializeField]
    private Sprite[] itemSpriteArray;

    private void Awake()
    {
        //playerScript = GetComponent<PlayerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);
            timeText = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
            lapText = GameObject.Find("Canvas/Lap").GetComponent<TextMeshProUGUI>();
            itemUI = GameObject.Find("Canvas/Item").GetComponent<Image>();

            lapText.text = "Lap 1 / " + laps.ToString();
        }

        //SyncNetVariables();
    }

    void localFinish()
    {
        playerScript.FinishedServerRpc(timePassed);
        GetComponent<KartMovement>().active = false;
        playerActive = false;
        timeText.text = timePassed.ToString();
    }

    [ServerRpc]
    void CreateItemServerRpc(Vector3 position, Vector3 velocity, ServerRpcParams rpcParams = default)
    {
        GameObject obert = Instantiate(blockingItem, position, Quaternion.identity);
        obert.GetComponent<NetworkObject>().Spawn();
        obert.GetComponent<Obert>().SetVelocity(velocity);
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        itemTime += Time.deltaTime;

        if (itemTime > itemWait)
        {
            itemUI.sprite = itemSpriteArray[1];
        }

        if (buttonPressed)
        {
            buttonPressed = false;
            if (hasItem && itemTime > itemWait)
            {
                itemUI.enabled = false;
                hasItem = false;
                CreateItem();
            }
        }
    }

    private void CreateItem()
    {
        Vector3 position = rigidbody.transform.position;

        Vector3 backwardVector = rigidbody.transform.rotation * Vector3.left;

        Vector3 delta = backwardVector * itemDistance;

        float dotProd = Vector3.Dot(rigidbody.velocity, backwardVector);

        Vector3 itemVector = (Mathf.Max(dotProd, 0) + itemSpeed ) * backwardVector;
        
        CreateItemServerRpc(position + delta, itemVector);
    }

    // Update is called once per frame
    void Update()
    {
        canvas.transform.position = transform.position;

        if (!IsOwner) return;
        if (!playerActive) return;

        if (Input.GetButtonDown("Jump"))
            buttonPressed = true;

        timePassed += Time.deltaTime;

        if (IsOwner)
        {
            timeText.text = timePassed.ToString();
        } 
    }

    private void checkpointCollision(Collider2D collision)
    {
        int index = collision.gameObject.GetComponent<Checkpoint>().Index;

        //do nothing if same checkpoint
        if (index == previousCheckpoint) return;

        if (index == (previousCheckpoint + 1) % checkpointNum)
            progress += 1;
        else
            progress -= 1;

        previousCheckpoint = index;

        if (progress / checkpointNum >= laps)
            localFinish();

        if (IsOwner)
        {
            int currentLap = Mathf.Min(1 + (progress / checkpointNum), laps);
            lapText.text = "Lap " + currentLap + " / " + laps.ToString();
        }
    }

    private void itemBoxCollision(Collider2D collision)
    {
        ItemBox itemBox = collision.gameObject.GetComponent<ItemBox>();
        if (itemBox.localOn)
        {
            itemBox.PlayerCollision();
            if (!hasItem)
            {
                hasItem = true;
                itemUI.enabled = true;
                itemUI.sprite = itemSpriteArray[0];
                itemTime = 0f;
                Debug.Log("itembox");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner) return;

        if (collision.gameObject.tag == "Obert")
        {
            collision.gameObject.GetComponent<Obert>().playerCollisionServerRpc();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        if (!playerActive) return;

        if (collision.gameObject.tag == "Checkpoint")
        {
            checkpointCollision(collision);
        }
        else if (collision.gameObject.tag == "ItemBox")
        {
            itemBoxCollision(collision);
        }
    }
}
