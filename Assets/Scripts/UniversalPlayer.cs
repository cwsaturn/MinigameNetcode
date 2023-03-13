using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UniversalPlayer : NetworkBehaviour
{
    [SerializeField]
    private GameObject Platformer;

    [ServerRpc(RequireOwnership = false)]
    void CreatePlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        GameObject player = Instantiate(Platformer, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
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

        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown("p"))
            {
                //CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
        
    }
}
