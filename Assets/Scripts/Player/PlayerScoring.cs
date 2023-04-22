using JetBrains.Annotations;
using System;
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

    public int TotalScore
    { get { return universalPlayer.playerScore.Value; } }
    private NetworkVariable<int> recentScore = new NetworkVariable<int>(0);
    public int RecentScore
    { get { return recentScore.Value; } }

    private bool calculatingScore;
    public bool CalculatingScore
    { get { return calculatingScore; } }

    private IEnumerator coroutine;

    private List<string> skipLastPlayerGames = new List<string>()
    {
        "Platformer2",
        "Kart",
        "Shootemup"
    };

    // Start is called before the first frame update
    void Start()
    {
        universalPlayer = GetComponent<UniversalPlayer>();
    }

    private void NewGame()
    {
        intermediateScore.Value = 0f;
        playerFinished.Value = false;
        calculatingScore = false;
    }

    public void SetPlayerFinished(float score)
    {
        if (!IsServer) return;
        playerFinished.Value = true;
        intermediateScore.Value = score;
        ScoringMain();
    }

    private void ScoringMain()
    {
        calculatingScore = false;
        playerList = new List<PlayerScoring>();
        finalScores = new List<float>();

        Debug.Log("Mark1");
        if (!IsServer) return;

        SetPlayerScoringList();
        int playersActive = PlayersLeft();

        if (skipLastPlayerGames.Contains(SceneManager.GetActiveScene().name))
        {
            Debug.Log("Mark2");
            //games that skip the last player
            if (playersActive > 1) return;
            calculatingScore = true;
            float total = SetFinalScores();
            SetFinalPlayer(total);
        }
        else
        {
            Debug.Log("Mark3");
            //games that do not skip the last player
            if (playersActive > 0) return;
            calculatingScore = true;
            SetFinalScores();
        }

        Debug.Log("Mark4");
        foreach (PlayerScoring player in playerList)
        {
            if (player.CalculatingScore && player != this)
            {
                NewGame();
                return;
            }
        }
        Debug.Log("Mark5");
        SendFinalScores();
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

    //returns players left
    private int PlayersLeft()
    {
        //Debug.Log("scoring list: " + playerList.ToString());

        int playersActive = 0;

        foreach (PlayerScoring player in playerList)
        {
            if (!player.playerFinished.Value)
            {
                playersActive++;
            }
        }
        return playersActive;
    }

    private float SetFinalScores()
    {
        float lastScore = 0f;
        //add intermediate scores
        foreach (PlayerScoring player in playerList)
        {
            if (player.playerFinished.Value)
            {
                float intermediateScore = player.intermediateScore.Value;
                finalScores.Add(intermediateScore);
                if (Mathf.Abs(intermediateScore) > Mathf.Abs(lastScore))
                {
                    lastScore = intermediateScore;
                }
            }
        }
        lastScore *= 1.1f;
        return lastScore;
    }

    //set the intermediate score of the last player
    private void SetFinalPlayer(float lastScore)
    {
        foreach (PlayerScoring player in playerList)
        {
            if (!player.playerFinished.Value)
            {
                player.intermediateScore.Value = lastScore;
                finalScores.Add(lastScore);
            }
        }
    }

    private void SendFinalScores()
    {
        finalScores.Sort();
        int playerCount = playerList.Count;

        foreach (PlayerScoring player in playerList)
        {
            int min = -1;
            int max = -1;

            float playerScore = player.intermediateScore.Value;
            for (int playerRank = 0; playerRank < playerCount; playerRank++)
            {
                if (playerScore == finalScores[playerRank])
                {
                    if (min == -1) min = playerRank;
                    max = playerRank;
                }
            }

            float average = ((float)max + (float)min) / 2f;

            player.AddScore(average, playerList.Count);
        }
        calculatingScore = false;
        NetworkManager.Singleton.SceneManager.LoadScene("MidgameLobby", LoadSceneMode.Single);
    }



    public void AddScore(float rank, float players)
    {
        if (!IsServer) return;

        if (players <= 1)
        {
            universalPlayer.AddScore(1);
            recentScore.Value = 1;
            intermediateScore.Value = 0f;
            playerFinished.Value = false;
            return;
        }

        int score = (int)(10f - (10f * rank) / (players - 1f));

        universalPlayer.AddScore(score);

        recentScore.Value = score;
        NewGame();
    }

    private void OnClientDisconnect(ulong action)
    {
        if (!IsServer || !IsOwnedByServer) return;

        coroutine = TryEndGame();
        StartCoroutine(coroutine);
    }

    IEnumerator TryEndGame()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ScoringMain();
    }

    void OnEnable()
    {
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    void OnDisable()
    {
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
