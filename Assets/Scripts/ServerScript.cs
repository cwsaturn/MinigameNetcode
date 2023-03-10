using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    public NetworkList<float> floatList;
    public NetworkVariable<float> networkFloat = new NetworkVariable<float>(0f);

    private List<GameObject> clientList = new List<GameObject>();

    //private NetworkList<GameObject> players;

    [SerializeField]
    private TextMeshProUGUI text;

    private void Awake()
    {
        floatList = new NetworkList<float>();
    }

    private void Start()
    {

    }



    [ServerRpc(RequireOwnership = false)]
    public void ChangeNumberServerRpc(ServerRpcParams rpcParams = default)
    {
        networkFloat.Value = Random.Range(0f, 1f);
        //colorList.Value.Add(color);
    }

    void Update()
    {
        text.text = networkFloat.Value.ToString();

        if (Input.GetKeyDown("w"))
        {
            ChangeNumberServerRpc();
        }

        if (IsClient)
        {
            if (Input.GetKeyDown("q"))
            {
                Debug.Log("color " + networkFloat.Value);
                Debug.Log("\n");
            }
            

            if (Input.GetKeyDown("e"))
            {
                foreach (NetworkClient player in NetworkManager.Singleton.ConnectedClients.Values)
                {
                    Color color = player.PlayerObject.GetComponent<ClientPlatformer>().PlayerColor;
                    Debug.Log(player.ClientId + "\n" + color.ToHexString());
                }
            }
        }

    }
}