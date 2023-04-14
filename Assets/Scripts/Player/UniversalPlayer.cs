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
    public Color PlayerColor
    { get { return playerColor.Value; } }
    public string Username
    { get { return username.Value.ToString(); } }

    //REMEMBER TO ADD TO NETWORK PREFABS
    //REMEMBER TO SET PREFAB TAG TO PLAYER
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject Platformer;
    [SerializeField]
    private GameObject Driver;
    [SerializeField]
    private GameObject ShellCursor;

    private GameObject currentPlayer;

    private PlayerScoring playerScoring;

    [ServerRpc]
    //[ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, string scene_name = "StartLobby", ServerRpcParams rpcParams = default)
    {

        GameObject player_obj;

        if (scene_name == "StartLobby")
        {
            username.Value = PlayerPrefs.GetString("username", "Player" + clientId);
        }

        if(scene_name == "Platformer" || scene_name == "Platformer2")
        {
            player_obj = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        }
        else if(scene_name == "Kart")
        {
            Vector3 offset = Vector3.zero;
            int playerNum = (int)clientId;
            offset.x = 2 * (playerNum % 4);
            offset.y = -2 * (playerNum / 4);
            player_obj = Instantiate(Driver, offset, Quaternion.identity);
        }
        else if (scene_name == "ShellGame")
        {
            Vector3 offset = Vector3.zero;
            int playerNum = (int)clientId;
            
            switch (playerNum)
            {
                case 0:
                    offset = new Vector3(-0.5f, 0.5f, 0);
                    break;
                case 1:
                    offset = new Vector3(0, 0.5f, 0);
                    break;
                case 2:
                    offset = new Vector3(-0.5f, -0.5f, 0);
                    break;
                case 3:
                    offset = new Vector3(0, -0.5f, 0);
                    break;
                default:
                    Debug.Log("too many players");
                    break;
            }

            player_obj = Instantiate(ShellCursor, offset, Quaternion.identity);
        }
        else
        {
            Vector3 offset = Vector3.zero;
            int playerNum = (int)clientId;
            offset.x = 4 * (playerNum % 2);
            offset.y = -4 * (playerNum / 2);
            player_obj = Instantiate(Player, offset, Quaternion.identity);
        }
        player_obj.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true); // destroyWithScene = true 
        player_obj.GetComponent<NetworkObject>().ChangeOwnership(clientId);
        Debug.Log("UniversalPlayer: Spawned a player object for: " + clientId);

        //Setup PlayerScript and PlayerScoring
        if (scene_name != "StartLobby" && scene_name != "MidgameLobby")
        {
            PlayerScript playerData = player_obj.GetComponent<PlayerScript>();
            playerData.playerScoring = playerScoring;
            //playerScoring.NewGame();
        }
        PlayerInitiatedClientRpc();
    }

    ///*
    [ClientRpc]
    private void PlayerInitiatedClientRpc(ClientRpcParams clientRpcParams = default)
    {
        FindPlayer();
        SetPlayerColor();
        SetPlayerName();
    }
    //*/

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        Debug.Log("Changed color");
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
            currentPlayer.GetComponent<PlayerScript>().SetColor(playerColor.Value);
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

    public int GetScore()
    {

        return playerScore.Value;
    }

    [ServerRpc]
    private void SetColorServerRpc()
    {
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
