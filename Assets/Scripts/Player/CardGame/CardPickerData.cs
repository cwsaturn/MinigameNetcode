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

    private TextMeshProUGUI timeText;
    private PlayerScript playerScript;

    private bool playerActive = true;

    public int score = 0;

    public NetworkVariable<int> player0Score = new NetworkVariable<int>(-1);
    public NetworkVariable<int> player1Score = new NetworkVariable<int>(-1);
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(-1);
    public NetworkVariable<int> player3Score = new NetworkVariable<int>(-1);

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

    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerActive) return;

        if (collision.gameObject.tag == "Card")
        {
            Debug.Log("Card");

            collision.gameObject.GetComponent<Animator>().SetBool("flipped", true);  // Flip card
            collision.gameObject.GetComponent<Collider2D>().enabled = false;  // Disable trigger

            int temp = 0;
            int.TryParse(collision.gameObject.GetComponentInChildren<TMP_Text>().text, out temp);
            score += temp;   // Add value of flipped card to score

            // Update score for player

            //GetComponent<ClientPlatformer>().active = false;
            //playerActive = false;
        }
    }
}
