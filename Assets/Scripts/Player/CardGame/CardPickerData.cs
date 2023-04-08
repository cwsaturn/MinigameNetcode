using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CardPickerData : NetworkBehaviour
{
    //public Text scoreText;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private SpriteRenderer playerSprite;

    private float timePassed = 0f;

    private TextMeshProUGUI timeText;
    private PlayerScript playerScript;

    private bool playerActive = true;

    private void Awake()
    {
        playerScript = GetComponent<PlayerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);
            //timeText = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
        }

        //SyncNetVariables();
    }

    private void SetTime(float time)
    {
        //timeText.text = "Time: " + time.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;

        timePassed += Time.deltaTime;

        if (IsOwner)
            SetTime(timePassed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (!IsOwner) return;

        //if (!playerActive) return;

        if (collision.gameObject.tag == "Card")
        {
            Debug.Log("Card");

            collision.gameObject.GetComponent<Animator>().SetBool("flipped", true);
            collision.gameObject.GetComponent<Collider2D>().enabled = false;


            //playerScript.FinishedServerRpc(timePassed);
            //GetComponent<ClientPlatformer>().active = false;
            //playerActive = false;
            //SetTime(timePassed);
        }
    }
}
