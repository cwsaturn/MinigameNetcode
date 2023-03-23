using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class UniversalPlayer : NetworkBehaviour
{
    GameObject myPlayer;

    [SerializeField]
    private GameObject Platformer;
    public NetworkVariable<int> playerScore = new NetworkVariable<int>(0);
    //private NetworkVariable<int> playerScore = new NetworkVariable<int>(0);

    [SerializeField]
    private GameObject Player;

    //public int playerScore = 0;

    [ServerRpc]
    //[ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, string scene_name = "Scrap", ServerRpcParams rpcParams = default)
    {

        GameObject player_obj;

        if(scene_name == "Platformer")
        {
            player_obj = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        }

        else
        {
            player_obj = Instantiate(Player, Vector3.zero, Quaternion.identity);
        }
        player_obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true); // destroyWithScene = true 
        player_obj.GetComponent<NetworkObject>().ChangeOwnership(clientId);
        Debug.Log("UniversalPlayer: Spawned a player object for: " + clientId);

        if (scene_name == "Platformer")
        {
            PlatformerData playerData = player_obj.GetComponent<PlatformerData>();
            playerData.universalPlayer = this;
            playerData.ScoreSetServerRpc(playerScore.Value);

            Color playerColor = Color.white;
            switch (OwnerClientId)
            {
                case 0:
                    playerColor = Color.red;
                    break;
                case 1:
                    playerColor = Color.blue;
                    break;
                case 2:
                    playerColor = Color.green;
                    break;
                case 3:
                    playerColor = Color.yellow;
                    break;
            }

            playerData.ColorSet(playerColor);
        }
    }

    //only call with server
    public void AddScore(int score)
    {
        if (!IsServer) return;
        playerScore.Value += score;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Spawn appropriate prefab depending on scene 
    {
        if(NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Scene loaded");
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, scene.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject); 
        if (IsLocalPlayer)
        {
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Hook 'OnSceneLoaded' to Unity's 'SceneManager.sceneLoaded' variable 
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

}
