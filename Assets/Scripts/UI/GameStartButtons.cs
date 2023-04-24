using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButtons : MonoBehaviour
{
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

        GUILayout.BeginArea(new Rect(10, 130, 300, 300));
        StartButton("TargetGame", "Start Target Game");
        GUILayout.EndArea(); 

        GUILayout.BeginArea(new Rect(10, 160, 300, 300));
        StartButton("Shootemup", "Start Shoot 'Em Up");
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 190, 300, 300));
        StartButton("RandomGame", "Start a Random Game");
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
                if (sceneName == "RandomGame")  // If the random game button is pressed
                {
                    string[] games = {"Platformer2", "Kart", "ShellGame", "Card", "TargetGame", "Shootemup"};
                    sceneName = games[Random.Range(0, games.Length)];  // Get random game from list
                    Debug.Log("Random scene: " + sceneName);
                }

                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }
}
