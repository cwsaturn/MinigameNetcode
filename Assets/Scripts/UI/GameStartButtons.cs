using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButtons : MonoBehaviour
{
    private static string MenuState = "BaseMenu";

    void OnGUI()
    {
        if (MenuState == "BaseMenu")  // Layer 1 buttons
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButton("StartSequence", "Game Sequence Thing");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("StartFreePlay", "Free Play Mode");
            GUILayout.EndArea();
        }
        
        else if (MenuState == "SequenceMenu")  // Buttons for game sequence submenu
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButton("IDK Yey 1", "IDK Yet 1");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("IDK Yey 2", "IDK Yet 2");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 70, 300, 300));
            StartButton("LeaveSequence", "Back");
            GUILayout.EndArea();
        }

        else  // Buttons for free play sub menu
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
            StartButton("Card", "Start Card Game");
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

            GUILayout.BeginArea(new Rect(10, 220, 300, 300));
            StartButton("LeaveFreePlay", "Back");
            GUILayout.EndArea();
        }
    }

    static void StartButton(string sceneName, string buttonText)
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        if (GUILayout.Button(buttonText))
        {
            // Flags for menu navigation
            if (sceneName == "StartFreePlay") {MenuState = "FreePlay";}
            else if (sceneName == "LeaveFreePlay" || sceneName == "LeaveSequence") {MenuState = "BaseMenu";}
            else if (sceneName == "StartSequence") {MenuState = "SequenceMenu";}

            // Game selection and transition
            else
            {
                if (sceneName == "RandomGame")  // Get a random game to start if needed
                {
                    string[] games = {"Platformer2", "Kart", "ShellGame", "Card", "TargetGame", "Shootemup"};
                    sceneName = games[Random.Range(0, games.Length)];  // Get random game from list
                    Debug.Log("Random scene: " + sceneName);
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
                
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            
        }
    }
}
