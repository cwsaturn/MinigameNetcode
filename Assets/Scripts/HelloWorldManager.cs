using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        [SerializeField]
        public GameObject PrefabToSpawn;

        private GameObject m_PrefabInstance;
        private NetworkObject m_SpawnedNetworkObject;

        void Start()  // Spawn the chracters
        {
            /*// Initiate
            m_PrefabInstance = Instantiate(PrefabToSpawn);

            // Get the instance's NetworkObject and Spawn
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.Spawn();*/

            /*var clientId = NetworkManager.Singleton.LocalClientId;

            GameObject go = Instantiate(PrefabToSpawn, Vector3.zero, Quaternion.identity);
            // go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);*/


            // var clientId = NetworkManager.Singleton.LocalClientId;
            // NetworkLog.LogInfoServer("Potato");  // Not his log syntax doesn't work either
            // GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            // var clientIdList = NetworkManager.LocalClient.PlayerObject;

            // GameObject go = Instantiate(myPrefab, Vector3.zero, Quaternion.identity);
            // go.GetComponent<NetworkObject>().Spawn();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                //StatusLabels();

                //SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewPosition()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
            {
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
                {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                else
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}