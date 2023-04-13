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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int NumPlayers = players.Length;

        if(IsServer)
            SetSeedServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetSeedServerRpc()
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
        CardReceiveSeedClientRpc();
    }


    [ClientRpc]
    public void CardReceiveSeedClientRpc()
    {
        Debug.Log("card assign change");
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
