using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace HelloWorld
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        public GameObject PrefabToSpawn;

        private GameObject m_PrefabInstance;
        private NetworkObject m_SpawnedNetworkObject;

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

            var clientId = NetworkManager.Singleton.LocalClientId;

            GameObject go = Instantiate(PrefabToSpawn, Vector3.zero, Quaternion.identity);
            // go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

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