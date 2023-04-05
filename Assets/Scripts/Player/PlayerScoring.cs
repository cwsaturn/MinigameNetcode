using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoring : NetworkBehaviour
{
    public NetworkVariable<float> intermediateScore = new NetworkVariable<float>(0f);
    public NetworkVariable<bool> playerFinished = new NetworkVariable<bool>(false);

    private UniversalPlayer universalPlayer;

    private List<float> finalScores = new List<float>();
    private List<PlayerScoring> playerList = new List<PlayerScoring>();


    // Start is called before the first frame update
    void Start()
    {
        universalPlayer = GetComponent<UniversalPlayer>();
    }

    public void NewGame()
    {
        intermediateScore.Value = 0f;
        playerFinished.Value = false;
    }

    public void SetPlayerFinished(float score)
    {
        if (!IsServer) return;

        playerFinished.Value = true;
        intermediateScore.Value = score;

        SetPlayerScoringList();
        bool gameComplete = CheckAllDone();

        if (!gameComplete) return;

        SetFinalScores();
    }

    private void SetPlayerScoringList()
    {
        GameObject[] universalPlayers = GameObject.FindGameObjectsWithTag("UniversalPlayer");

        if (universalPlayers.Length == 0)
        {
            Debug.Log("length zero ??");
            return;
        }

        foreach (GameObject player in universalPlayers)
        {
            PlayerScoring playerScore = player.GetComponent<PlayerScoring>();
            playerList.Add(playerScore);
        }
    }

    private bool CheckAllDone()
    {
        Debug.Log("scoring list: " + playerList.ToString());

        foreach (PlayerScoring player in playerList)
        {
            if (!player.playerFinished.Value) return false;

            finalScores.Add(player.intermediateScore.Value);
        }
        return true;
    }

    private void SetFinalScores()
    {
        finalScores.Sort();
        int playerCount = playerList.Count;

        

        foreach (PlayerScoring player in playerList)
        {
            float playerScore = player.intermediateScore.Value;
            for (int playerRank = 0; playerRank < playerCount; playerRank++)
            {
                if (playerScore <= finalScores[playerRank])
                {
                    player.AddScore(playerRank + 1, playerCount);
                    break;
                }
            }
        }

        playerList.Clear();

        NetworkManager.Singleton.SceneManager.LoadScene("MidgameLobby", LoadSceneMode.Single);
    }

    public void AddScore(int rank, int players)
    {
        if (!IsServer) return;

        if (players <= 0) return;

        int score = 10 * (players - rank + 1) / players;

        universalPlayer.AddScore(score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
