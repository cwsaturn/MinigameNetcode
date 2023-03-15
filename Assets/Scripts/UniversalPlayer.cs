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

    [ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        GameObject player = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        myPlayer = player;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetScorePlatformerServerRpc(int score, ServerRpcParams rpcParams = default)
    {
        currentGameScore.Value = score;
        Debug.Log("Universal Player Score : " + score);
        bool allFinished = true;
        foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            bool finished = player.PlayerObject.GetComponent<PlatformerData>().playerFinished.Value;
            if (!finished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
        {
            //NetworkManager.Singleton.SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
            Debug.Log("!Switch Scene!");
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("Level: " + level);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
