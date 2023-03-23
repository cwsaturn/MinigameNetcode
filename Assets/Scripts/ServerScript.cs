using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerScript : NetworkBehaviour
{
    private void Start()
    {

    }

    //call from server
    public void CheckPlayersFinished()
    {
        if (!IsServer) return;

        List<float> finalTimes = new List<float>();
        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlatformerData playerData = player.PlayerObject.GetComponent<PlatformerData>();
            if (!playerData.PlayerFinished)
                return;

            finalTimes.Add(playerData.FinalTime);
        }

        //All are done
        finalTimes.Sort();
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlatformerData playerData = player.PlayerObject.GetComponent<PlatformerData>();
            float playerTime = playerData.FinalTime;
            for (int playerRank = 0; playerRank < finalTimes.Count; playerRank++)
            {
                if (playerTime <= finalTimes[playerRank])
                {
                    playerData.AddFinalScore(playerRank + 1, playerCount);
                    break;
                }
            }
            NetworkManager.Singleton.SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
        }
    }

    void Update()
    {
    }
}