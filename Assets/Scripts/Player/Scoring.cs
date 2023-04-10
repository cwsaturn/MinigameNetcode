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

    [SerializeField]
    private TextMeshProUGUI[] PlayerGUI;

    // Start is called before the first frame update
    void Start()
    {
        SetScores();
    }

    void SetScores()
    {
        GameObject[] universalPlayers = GameObject.FindGameObjectsWithTag("UniversalPlayer");

        List<PlayerScoring> playerScorings = new List<PlayerScoring>();

        foreach (GameObject universalPlayer in universalPlayers)
        {
            playerScorings.Add(universalPlayer.GetComponent<PlayerScoring>());
        }

        playerScorings.Sort((x, y) => x.TotalScore.CompareTo(y.TotalScore));
        playerScorings.Reverse();

        for (int player = 0; player < PlayerGUI.Length; player++)
        {
            if (player >= playerScorings.Count)
            {
                PlayerGUI[player].enabled = false;
                continue;
            }

            PlayerScoring playerScore = playerScorings[player];
            UniversalPlayer universalPlayer = playerScore.GetComponent<UniversalPlayer>();

            string playerText = universalPlayer.Username + " : ";
            playerText += playerScore.TotalScore + " + " + playerScore.RecentScore;

            PlayerGUI[player].text = playerText;

            Color color = universalPlayer.PlayerColor;
            color.r = Mathf.Min(color.r + 0.7f, 1f);
            color.g = Mathf.Min(color.g + 0.7f, 1f);
            color.b = Mathf.Min(color.b + 0.7f, 1f);

            PlayerGUI[player].color = color;
        }

        /*
        size = 0;
        foreach (GameObject player in universalPlayers)
        {
            size = size + 1;
            PlayerScoring playerScore = player.GetComponent<PlayerScoring>();
            playerList.Add(playerScore);
            //Player1.text = "Player1: " + playerList[0].ToString();
        }

        Debug.Log(size);

        if(size == 1)
        {
            PlayerGUI[0].text = "Player 1: " + playerList[0].GetScore().Value;
        }

        else if(size == 2)
        {
            PlayerGUI[0].text = "Player 1: " + playerList[0].GetScore().Value;
            PlayerGUI[1].text = "Player 2: " + playerList[1].GetScore().Value;
        }

        else if(size == 3)
        {
            PlayerGUI[0].text = "Player 1: " + playerList[0].GetScore().Value;
            PlayerGUI[1].text = "Player 2: " + playerList[1].GetScore().Value;
            PlayerGUI[2].text = "Player 3: " + playerList[2].GetScore().Value;
        }

        else if(size == 3)
        {
            PlayerGUI[0].text = "Player 1: " + playerList[0].GetScore().Value;
            PlayerGUI[1].text = "Player 2: " + playerList[1].GetScore().Value;
            PlayerGUI[2].text = "Player 3: " + playerList[2].GetScore().Value;
            PlayerGUI[3].text = "Player 4: " + playerList[3].GetScore().Value;
        }
        */
    }
}
