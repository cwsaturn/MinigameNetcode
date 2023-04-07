using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UsernameScript : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void SetUsername(string username)
    {
        text.text = username;
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
