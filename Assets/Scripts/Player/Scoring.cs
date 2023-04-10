using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using static PlayerScoring;

public class Scoring : MonoBehaviour
{
private int size = 0;

    public NetworkVariable<int> playerScore = new NetworkVariable<int>(0);
    private List<PlayerScoring> playerList = new List<PlayerScoring>();

    public TextMeshProUGUI Player1;
    public TextMeshProUGUI Player2;
    public TextMeshProUGUI Player3;
    public TextMeshProUGUI Player4;

    // Start is called before the first frame update
    void Start()
    {
        SetScores();
    }

    void SetScores()
    {
        GameObject[] universalPlayers = GameObject.FindGameObjectsWithTag("UniversalPlayer");

        size = 0;
        foreach (GameObject player in universalPlayers)
        {
            size = size + 1;
            PlayerScoring playerScore = player.GetComponent<PlayerScoring>();
            playerList.Add(playerScore);
            Player1.text = "Player1: " + playerList[0].ToString();
        }

        Debug.Log(size);

        if(size == 1)
        {
            Player1.text = "Player 1: " + playerList[0].GetScore().Value;
        }

        else if(size == 2)
        {
            Player1.text = "Player 1: " + playerList[0].GetScore().Value;
            Player2.text = "Player 2: " + playerList[1].GetScore().Value;
        }

        else if(size == 3)
        {
            Player1.text = "Player 1: " + playerList[0].GetScore().Value;
            Player2.text = "Player 2: " + playerList[1].GetScore().Value;
            Player3.text = "Player 3: " + playerList[2].GetScore().Value;
        }

        else if(size == 3)
        {
            Player1.text = "Player 1: " + playerList[0].GetScore().Value;
            Player2.text = "Player 2: " + playerList[1].GetScore().Value;
            Player3.text = "Player 3: " + playerList[2].GetScore().Value;
            Player4.text = "Player 4: " + playerList[3].GetScore().Value;
        }
    }
}
