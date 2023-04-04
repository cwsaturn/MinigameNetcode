using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class UniversalPlayer : NetworkBehaviour
{
    //GameObject myPlayer;

    public NetworkVariable<int> playerScore = new NetworkVariable<int>(0);
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.gray);
    private NetworkVariable<FixedString32Bytes> username = new NetworkVariable<FixedString32Bytes>("");

    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject Platformer;
    [SerializeField]
    private GameObject Driver;

    private GameObject currentPlayer;

    private PlayerScoring playerScoring;

    [ServerRpc]
    //[ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, string scene_name = "StartLobby", ServerRpcParams rpcParams = default)
    {

        GameObject player_obj;

        if(scene_name == "Platformer")
        {
            player_obj = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        }
        else if(scene_name == "Kart")
        {
            player_obj = Instantiate(Driver, Vector3.zero, Quaternion.identity);
        }
        else
        {
            player_obj = Instantiate(Player, Vector3.zero, Quaternion.identity);
        }
        player_obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true); // destroyWithScene = true 
        player_obj.GetComponent<NetworkObject>().ChangeOwnership(clientId);
        Debug.Log("UniversalPlayer: Spawned a player object for: " + clientId);

        if (scene_name == "StartLobby")
        {
            //
        }

        if (scene_name == "Platformer")
        {
            PlayerScript playerData = player_obj.GetComponent<PlayerScript>();
            playerData.playerScoring = playerScoring;
        }
        playerScoring.NewGame();
        PlayerInitiatedClientRpc();
    }

    ///*
    [ClientRpc]
    private void PlayerInitiatedClientRpc(ClientRpcParams clientRpcParams = default)
    {
        FindPlayer();
        SetPlayerColor();
        SetPlayerName();
        Debug.Log("client rpc");
        Debug.Log("color set to " + playerColor.Value.ToString());
    }
    //*/

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        playerColor.OnValueChanged += OnColorChange;
        username.OnValueChanged += OnUsernameChange;
    }

    public override void OnNetworkDespawn()
    {
        // Unsubscribe to value changes
        playerColor.OnValueChanged -= OnColorChange;
        username.OnValueChanged -= OnUsernameChange;
    }

    public void OnColorChange(Color previous, Color current)
    {
        SetPlayerColor();
    }

    public void OnUsernameChange(FixedString32Bytes previous, FixedString32Bytes current)
    {
        SetPlayerName();
    }

    private void SetPlayerColor()
    {
        if (currentPlayer != null)
        {
            currentPlayer.GetComponent<SpriteRenderer>().color = playerColor.Value;
        }
    }

    private void SetPlayerName()
    {
        if (currentPlayer != null)
        {
            currentPlayer.GetComponent<PlayerScript>().SetUsername(username.Value.ToString());
        }
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

    [ServerRpc(RequireOwnership = false)]
    public void SetUsernameServerRpc(string newName)
    {
        username.Value = newName;
    }

    private void FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == OwnerClientId)
            {
                currentPlayer = player;
                return;
            }
        }
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

        playerScoring = GetComponent<PlayerScoring>();

        if (IsLocalPlayer)
        {
            SetColorServerRpc();

            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        FindPlayer();
        SetPlayerColor();
        SetPlayerName();
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
