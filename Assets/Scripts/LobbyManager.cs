using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Unity.Networking.Transport;
using Unity.Netcode.Transports.UTP;

namespace HelloWorld
{
    public class LobbyManager : MonoBehaviour
    {
        private int maxPlayers = 4; 

        void Start()
        {
            // Enable connection approval 
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true; 
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCallback; 
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

            string IsPlayerHost = PlayerPrefs.GetString("IsHost");

            // https://docs-multiplayer.unity3d.com/netcode/current/components/networkmanager
            // Set IP and port 
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            PlayerPrefs.GetString("HostIpAddr"),  // The IP address is a string
            (ushort)12345 // The port number is an unsigned short
            );

            if (IsPlayerHost == "True")
            {
                Debug.Log("LobbyManager: Start(), Host");
                NetworkManager.Singleton.StartHost();
            }

            else
            {
                Debug.Log("LobbyManager: Start(), Client");
                NetworkManager.Singleton.StartClient();
            }

            var clientId = NetworkManager.Singleton.LocalClientId;
        }

        /*

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButton("Platformer2", "Start Platformer");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("Kart", "Start Kart Game");
            GUILayout.EndArea();
        }

        static void StartButton(string sceneName, string buttonText)
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            if (mode == "Host")
            {
                if (GUILayout.Button(buttonText))
                { 
                    //SceneManager.LoadScene("Platformer", LoadSceneMode.Single);
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }
        }

        */


        //Ref: https://docs-multiplayer.unity3d.com/netcode/current/basics/connection-approval/index.html
        private void ApprovalCallback(NetworkManager.ConnectionApprovalRequest approvalRequest, NetworkManager.ConnectionApprovalResponse approvalResponse)
        {
            if(NetworkManager.Singleton.ConnectedClientsList.Count < maxPlayers)
            {
                Debug.Log("LobbyManager: Successful Approval");
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
                Debug.Log("LobbyManager: Denied Approval");
                //Application.Quit();
                NetworkManager.Singleton.Shutdown();
                SceneManager.LoadScene("MainMenu");
            }
        }


    }
}