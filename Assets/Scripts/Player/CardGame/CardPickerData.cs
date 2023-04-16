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

    public CardGameManager cardGameManager;
    public int playerID;  // This is only correct on the server side
    

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
        }
        GameObject cardGameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        if(cardGameManagerObj != null)
            cardGameManager = cardGameManagerObj.GetComponent<CardGameManager>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer)
                return;
        
        Debug.Log("CURRENT PLAYER TOUCHING CARD: " + playerID.ToString());
        Debug.Log(" & ACTIVE PLAYER: " + cardGameManager.activePlayersID.Value.ToString());

        if (collision.gameObject.tag == "Card" && (playerID == cardGameManager.activePlayersID.Value))  // Only flip the card if you are currently the active player
        {
            // loop over all the cards to find what index of card was touched... to send a client rpc :)
            int cardIndexOfCardTouched = -9999999;

            for(int i = 0; i < cardGameManager.cards.Length; i++)
            {
                if(GameObject.ReferenceEquals(cardGameManager.cards[i], collision.gameObject))
                {
                    cardIndexOfCardTouched = i;
                    break;
                }
            }

            CardTouchedClientRPC(cardIndexOfCardTouched);

            Debug.Log("Card at " + cardIndexOfCardTouched.ToString() + " flipped!");
            collision.gameObject.GetComponent<Animator>().SetBool("flipped", true);  // Flip card
            collision.gameObject.GetComponent<Collider2D>().enabled = false;  // Disable trigger

            /*if(!IsServer)
                return;*/

            int cardPickedUp = 0;
            int.TryParse(collision.gameObject.GetComponentInChildren<TMP_Text>().text, out cardPickedUp);

            // Update score for player
            CardPickupServerSide(cardPickedUp);
        }
    }


    public void CardPickupServerSide(int cardVal)
    {
        Debug.Log("server card pickup");

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


    [ClientRpc]
    public void CardTouchedClientRPC(int cardIndex)
    {
        if(cardGameManager == null) // cant be null on client side due to client rpc for flipping later!
        {
            GameObject cardGameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
            if(cardGameManagerObj != null)
                cardGameManager = cardGameManagerObj.GetComponent<CardGameManager>();
        }

        Debug.Log("Card at " + cardIndex.ToString() + " flipped!");
        cardGameManager.cards[cardIndex].GetComponent<Animator>().SetBool("flipped", true);  // Flip card
        cardGameManager.cards[cardIndex].GetComponent<Collider2D>().enabled = false;  // Disable trigger
    }

}
