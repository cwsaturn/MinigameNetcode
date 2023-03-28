using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class UniversalPlayer : NetworkBehaviour
{
    //GameObject myPlayer;

    public NetworkVariable<int> playerScore = new NetworkVariable<int>(0);
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.gray);

    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject Platformer;

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

        if (scene_name == "Scrap")
        {
            PlayerInitiatedClientRpc();
        }

        if (scene_name == "Platformer")
        {
            PlatformerData playerData = player_obj.GetComponent<PlatformerData>();
            playerData.universalPlayer = this;
            PlayerInitiatedClientRpc();
        }
    }

    ///*
    [ClientRpc]
    private void PlayerInitiatedClientRpc(ClientRpcParams clientRpcParams = default)
    {
        SetPlayerColor();
        Debug.Log("client rpc");
        Debug.Log("color set to " + playerColor.Value.ToString());
    }
    //*/

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        playerColor.OnValueChanged += OnColorChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        playerColor.OnValueChanged -= OnColorChange;
    }

    public void OnColorChange(Color previous, Color current)
    {
        SetPlayerColor();
    }

    //only call with server
    public void AddScore(int score)
    {
        if (!IsServer) return;
        playerScore.Value += score;
    }

    [ServerRpc]
    private void SetColorServerRpc()
    {
        Debug.Log("setting color");
        switch (OwnerClientId)
        {
            case 0:
                playerColor.Value = Color.red;
                return;
            case 1:
                playerColor.Value = Color.blue;
                return;
            case 2:
                playerColor.Value = Color.green;
                return;
            case 3:
                playerColor.Value = Color.yellow;
                return;
            default:
                playerColor.Value = Random.ColorHSV(0f, 1f);
                return;
        }
    }

    private void SetPlayerColor()
    {
        GameObject player = FindPlayer();
        if (player != null)
        {
            player.GetComponent<SpriteRenderer>().color = playerColor.Value;
            Debug.Log("player found");
            Debug.Log("color set to " + playerColor.Value.ToString());
        }
    }

    private GameObject FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == OwnerClientId)
            {
                return player;
            }
        }
        return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Spawn appropriate prefab depending on scene 
    {
        if(NetworkManager.Singleton.IsClient)
        {
            if (!IsOwner) return;

            Debug.Log("Scene loaded");
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId, scene.name);
        }

        //SetPlayerColor();
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (IsLocalPlayer)
        {
            SetColorServerRpc();

            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        SetPlayerColor();
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
