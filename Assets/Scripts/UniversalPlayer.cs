using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UniversalPlayer : NetworkBehaviour
{
    GameObject myPlayer;

    [SerializeField]
    private GameObject Platformer;

    private NetworkVariable<int> playerScore = new NetworkVariable<int>(0);

    [ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        GameObject player = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        myPlayer = player;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddToScoreServerRpc(int score, ServerRpcParams rpcParams = default)
    {
        playerScore.Value += score;
        Debug.Log("made it here. Score : " + playerScore.Value);
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
        if (myPlayer != null)
        {
            if (Input.GetKeyDown("r"))
            {
                Debug.Log("player done: " + myPlayer.GetComponent<ClientPlatformer>().PlayerFinished);
            }
        }


        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown("p"))
            {
                //CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
        
    }
}
