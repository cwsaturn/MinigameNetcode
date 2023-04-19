using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        StartButton("Platformer2", "Start Platformer");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 40, 300, 300));
        StartButton("Kart", "Start Kart Game");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 70, 300, 300));
        StartButton("ShellGame", "Start Cup Game");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 100, 300, 300));
        StartButton("Card", "Start Random Card Game");
        GUILayout.EndArea();
    }

    static void StartButton(string sceneName, string buttonText)
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        if (mode == "Host")
        {
            if (GUILayout.Button(buttonText))
            {
                //SceneManager.LoadScene("Platformer", LoadSceneMode.Single);
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
