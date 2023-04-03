using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SetUsername : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameEntry;

    public GameObject universalPlayer;

    public void ResetText()
    {
        usernameEntry.text = PlayerPrefs.GetString("username", "");
    }

    public void SetText()
    {
        string username = usernameEntry.text;

        PlayerPrefs.SetString("username", username);


        ulong myId = NetworkManager.Singleton.LocalClientId;
        GameObject[] players = GameObject.FindGameObjectsWithTag("UniversalPlayer");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == myId)
            {
                player.GetComponent<UniversalPlayer>().SetUsernameServerRpc(username);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
