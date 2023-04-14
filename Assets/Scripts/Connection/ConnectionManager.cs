using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; 
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{

    private int maxPlayers = 4; 

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true; 
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCallback; 
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    //Ref: https://docs-multiplayer.unity3d.com/netcode/current/basics/connection-approval/index.html
    private void ApprovalCallback(NetworkManager.ConnectionApprovalRequest approvalRequest, NetworkManager.ConnectionApprovalResponse approvalResponse)
    {
        if(NetworkManager.Singleton.ConnectedClientsList.Count < maxPlayers && SceneManager.GetActiveScene().name == "StartLobby")
        {
            Debug.Log("ConnectionManager: Successful Approval");
            approvalResponse.Approved = true; 
            approvalResponse.CreatePlayerObject = true; 
        }

        else
        {
            approvalResponse.Approved = false;
        }

    }

    private void OnClientDisconnectCallback(ulong obj) // Close game for denied connections 
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.Log("ConnectionManager: Denied Approval");
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
