using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using static PlayerScoring;

public class Scoring : MonoBehaviour
{
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
        float playerxp = PlayerPrefs.GetFloat("xp", 0);

        GameObject[] universalPlayers = GameObject.FindGameObjectsWithTag("UniversalPlayer");

        playerList = new List<PlayerScoring>();

        foreach (GameObject universalPlayer in universalPlayers)
        {
            playerList.Add(universalPlayer.GetComponent<PlayerScoring>());
        }

        playerList.Sort((x, y) => x.TotalScore.CompareTo(y.TotalScore));
        playerList.Reverse();

        for (int player = 0; player < PlayerGUI.Length; player++)
        {
            if (player >= playerList.Count)
            {
                PlayerGUI[player].enabled = false;
                continue;
            }

            PlayerScoring playerScore = playerList[player];
            UniversalPlayer universalPlayer = playerScore.GetComponent<UniversalPlayer>();

            string playerText = universalPlayer.Username + " : ";
            playerText += playerScore.TotalScore + " + " + playerScore.RecentScore;
            PlayerPrefs.SetFloat("xp", playerxp + playerScore.TotalScore);

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
