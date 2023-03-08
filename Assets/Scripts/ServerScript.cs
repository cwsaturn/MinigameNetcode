using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerScript : NetworkBehaviour
{
    public NetworkList<float> colorList;
    public NetworkVariable<float> networkFloat = new NetworkVariable<float>(0f);

    private void Awake()
    {
        colorList = new NetworkList<float>();
    }

    private void Start()
    {

    }


    [ServerRpc]
    public void AddColorServerRpc(float color, ServerRpcParams rpcParams = default)
    {
        colorList.Add(color);
        networkFloat.Value = Random.Range(0f, 1f);
        //colorList.Value.Add(color);
    }

    void Update()
    {
        if (IsClient)
        {
            if (Input.GetKeyDown("q"))
            {
                Debug.Log("color " + networkFloat.Value);
                Debug.Log("\n");
            }
        }
    }
}