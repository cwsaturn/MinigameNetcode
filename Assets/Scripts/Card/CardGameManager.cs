using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    public GameObject[] cards;
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int NumPlayers = players.Length;

        Debug.Log("Num Players: " + NumPlayers);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
