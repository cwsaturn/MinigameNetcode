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

    //public int score = 0;

    

    public int playerID;

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


            // only update score if localplayer
            if(!IsServer)
                return;

            int cardPickedUp = 0;
            int.TryParse(collision.gameObject.GetComponentInChildren<TMP_Text>().text, out cardPickedUp);

            // Update score for player
            CardPickupServerSide(cardPickedUp);
        }
    }


    public void CardPickupServerSide(int cardVal)
    {
        Debug.Log("server rpc card pickup");
        
        GameObject cardGameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        CardGameManager cardGameManager = cardGameManagerObj.GetComponent<CardGameManager>();

        switch(playerID)
        {
            case 0:
                cardGameManager.player0Score.Value += cardVal;
                break;
            case 1:
                cardGameManager.player1Score.Value += cardVal;
                break;
            case 2:
                cardGameManager.player2Score.Value += cardVal;
                break;
            case 3:
                cardGameManager.player3Score.Value += cardVal;
                break;
            default:
                Debug.Log("AAAAAAAAAAA it broke :(");
                Debug.Log(cardVal + " " + playerID);
                break;
        }
    }

}
