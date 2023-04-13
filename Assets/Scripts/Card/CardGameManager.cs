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



    // Start is called before the first frame update
    void Start()
    {
        if(IsClient)
            return;

        
        randomSeverSeed.Value = Random.Range(-9999, 9999);


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int NumPlayers = players.Length;

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

    // Update is called once per frame
    void Update()
    {
        
    }


    [ServerRpc]
    void ScoreSetServerRpc(ServerRpcParams rpcParams = default)
    {
        randomSeverSeed.Value++;
    }


    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        randomSeverSeed.OnValueChanged += OnRandSeedChange;
    }


    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        randomSeverSeed.OnValueChanged -= OnRandSeedChange;
    }

    public void OnRandSeedChange(int previous, int current)
    {
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
}
