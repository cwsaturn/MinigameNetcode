using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CardGameManager : NetworkBehaviour
{
    public GameObject[] cards;

    public int[] cardValues;

    public NetworkVariable<int> randomSeverSeed = new NetworkVariable<int>(0);
    public NetworkVariable<int> player0Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> player3Score = new NetworkVariable<int>(0);

    public NetworkVariable<int> activePlayersID = new NetworkVariable<int>(0);

    public int fullTurnsTaken = 0;
    private const int maxTurns = 3;

    public TextMeshProUGUI currentPlayerText;


    // Start is called before the first frame update
    void Start()
    {
        if(IsServer)
            ServerSetSeed();
        
        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            CardPickerData cardPickerData = player.GetComponent<CardPickerData>();
            cardPickerData.cardGameManager = this;
        }*/
    }


    //[ServerRpc(RequireOwnership = false)]
    public void ServerSetSeed()
    {
        randomSeverSeed.Value = Random.Range(-9999, 9999);

        Random.InitState(randomSeverSeed.Value);    // seed random num
        cardValues = new int[cards.Length];
        int i = 0;
        foreach(GameObject card in cards)
        {
            TMP_Text cardText = card.GetComponentInChildren<TMP_Text>();
            int cardVal = Random.Range(0, 30);  // 0 through 29
            cardValues[i] = cardVal;
            cardText.text = cardVal.ToString();
            i++;
        }

        Debug.Log("seed set");
    }


    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        randomSeverSeed.OnValueChanged += OnSeedChange;
        player0Score.OnValueChanged += OnScoreChange;
        player1Score.OnValueChanged += OnScoreChange;
        player2Score.OnValueChanged += OnScoreChange;
        player3Score.OnValueChanged += OnScoreChange;
        activePlayersID.OnValueChanged += OnTurnChange;

        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            CardPickerData cardPickerData = player.GetComponent<CardPickerData>();
            cardPickerData.cardGameManager = this;
        }*/
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        randomSeverSeed.OnValueChanged -= OnSeedChange;
        player0Score.OnValueChanged -= OnScoreChange;
        player1Score.OnValueChanged -= OnScoreChange;
        player2Score.OnValueChanged -= OnScoreChange;
        player3Score.OnValueChanged -= OnScoreChange;
        activePlayersID.OnValueChanged -= OnTurnChange;
    }


    public void OnSeedChange(int previous, int current)
    {
        Debug.Log("card seed change!!! SEED: " + current);
        Random.InitState(randomSeverSeed.Value);    // seed random num

        cardValues = new int[cards.Length];
        int i = 0;
        foreach(GameObject card in cards)
        {
            TMP_Text cardText = card.GetComponentInChildren<TMP_Text>();
            int cardVal = Random.Range(0, 30);  // 0 through 29
            cardValues[i] = cardVal;
            cardText.text = cardVal.ToString();
            i++;
        }
    }


    public void OnScoreChange(int previous, int current)
    {
        Debug.Log("someone did a card pickup... updating text!");
        GameObject scoreObj = GameObject.FindGameObjectWithTag("ScoreText");
        TMP_Text scoreText = scoreObj.GetComponent<TMP_Text>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int numPlayers = players.Length;
        string scoreTextTemp = "";

        for(int i = 0; i < numPlayers; i++)
        {
            switch(i)
            {
                case 0:
                    scoreTextTemp += "Red Player: " + player0Score.Value + "\n";
                    break;
                case 1:
                    scoreTextTemp += "Blue Player: " + player1Score.Value + "\n";
                    break;
                case 2:
                    scoreTextTemp += "Green Player: " + player2Score.Value + "\n";
                    break;
                case 3:
                    scoreTextTemp += "Yellow Player: " + player3Score.Value + "\n";
                    break;
            }
        }

        scoreText.text = scoreTextTemp;

        if (IsServer)
        {
            activePlayersID.Value += 1;

            if (activePlayersID.Value + 1 > numPlayers)  // After a full cycle through all of the players, +1 since ID is zero indexed
            {
                fullTurnsTaken += 1;
                activePlayersID.Value = 0;
            }

            if (fullTurnsTaken >= maxTurns)  // End game after 3 fulls turns
            {
                // wait for 2 seconds before sending end message!
                StartCoroutine(Wait2SecondsEndGame(players));

            }
        }
    }

    public void OnTurnChange(int pervious, int current)
    {
        //currentPlayerText.text = "Current Player: " + (activePlayersID.Value + 1).ToString();
        switch(activePlayersID.Value)
        {
            case 0:
                currentPlayerText.text = "Red Player's Turn!";
                break;
            case 1:
                currentPlayerText.text = "Blue Player's Turn!";
                break;
            case 2:
                currentPlayerText.text = "Green Player's Turn!";
                break;
            case 3:
                currentPlayerText.text = "Yellow Player's Turn!";
                break;
        }
    }


    IEnumerator Wait2SecondsEndGame(GameObject[] players)
    {
        yield return new WaitForSecondsRealtime(.2f);

        for(int i = 0; i < players.Length; i++)
        {
            CardPickerData cpd = players[i].GetComponent<CardPickerData>();
            switch(i)
            {
                case 0:
                    cpd.FinishedGameClientRpc(-player0Score.Value);     // negative because lowest is the best in scoring func... but in our game highest is the best
                    break;
                case 1:
                    cpd.FinishedGameClientRpc(-player1Score.Value);
                    break;
                case 2:
                    cpd.FinishedGameClientRpc(-player2Score.Value);
                    break;
                case 3:
                    cpd.FinishedGameClientRpc(-player3Score.Value);
                    break;
            }
        }
    }
}
