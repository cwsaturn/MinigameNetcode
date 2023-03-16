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
    private NetworkVariable<int> currentGameScore = new NetworkVariable<int>(0);
    private NetworkVariable<int> playerScore = new NetworkVariable<int>(0);

    [SerializeField]
    private GameObject Player;

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
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetScorePlatformerServerRpc(int score, ServerRpcParams rpcParams = default)
    {
        currentGameScore.Value = score;
        Debug.Log("Universal Player Score : " + score);
        bool allFinished = true;
        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            bool finished = player.PlayerObject.GetComponent<PlatformerData>().PlayerFinished;
            if (!finished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
            //Debug.Log("!Switch Scene!");
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Level: " + level);
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
