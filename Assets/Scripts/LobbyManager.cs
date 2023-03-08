using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HelloWorld
{
    public class LobbyManager : MonoBehaviour
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
            StartButton();
            GUILayout.EndArea();
        }

        static void StartButton()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            if (mode == "Host")
            {
                if (GUILayout.Button("Start Game")) SceneManager.LoadScene("Platformer", LoadSceneMode.Single);
            }
        }

    }
}