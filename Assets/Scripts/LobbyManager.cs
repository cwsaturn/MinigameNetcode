using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;

namespace HelloWorld
{
    public class LobbyManager : MonoBehaviour
    {
        void Start()
        {
            string IsPlayerHost = PlayerPrefs.GetString("IsHost");

            // https://docs-multiplayer.unity3d.com/netcode/current/components/networkmanager
            // Set IP and port 
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            PlayerPrefs.GetString("HostIpAddr"),  // The IP address is a string
            ushort.Parse(PlayerPrefs.GetString("HostPort")) // The port number is an unsigned short
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
    }
}