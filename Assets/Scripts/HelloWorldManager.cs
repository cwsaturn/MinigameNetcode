using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        void Start()
        {
            string IsPlayerHost = PlayerPrefs.GetString("IsHost");

            if (IsPlayerHost == "True")
            {
                NetworkManager.Singleton.StartHost();
            }

            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.EndArea();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

    }
}