using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartButtons : MonoBehaviour
{
    private static string MenuState = "BaseMenu";
    private static int GamesLeftToPlay = 0;

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
            StartButton("Sequence3", "Play 3 Random Games");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("Sequence5", "Play 5 Random Games");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 70, 300, 300));
            StartButton("Sequence7", "Play 7 Random Games");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 100, 300, 300));
            StartButton("This Doesn't do Anything yet", "Custom");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 130, 300, 300));
            StartButton("LeaveSequence", "Back");
            GUILayout.EndArea();
        }

        else if (MenuState == "MidSequenceMenu")
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButton("ContinueSequence", "Continue");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("EndSequence", "End");
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

        if (mode == "Host")  // Only host makes game choices
        {
            if (GUILayout.Button(buttonText))
            {
                // Flags for menu navigation
                if (sceneName == "StartFreePlay") {MenuState = "FreePlay";}
                else if (sceneName == "LeaveFreePlay" || sceneName == "LeaveSequence") {MenuState = "BaseMenu";}
                else if (sceneName == "StartSequence") {MenuState = "SequenceMenu";}

                else if (sceneName == "EndSequence")  // Quiting a sequence of games early
                {
                    MenuState = "BaseMenu";
                    GamesLeftToPlay = 0;
                }

                // Game selection and transition
                else
                {
                    if (sceneName == "Sequence3") {GamesLeftToPlay = 3;}  // Set count for sequence play
                    else if (sceneName == "Sequence5") {GamesLeftToPlay = 5;}
                    else if (sceneName == "Sequence7") {GamesLeftToPlay = 7;}

                    if (sceneName == "RandomGame" || sceneName == "ContinueSequence" || GamesLeftToPlay > 0)  // Get a random game to start if needed
                    {
                        string[] games = {"Platformer2", "Kart", "ShellGame", "Card", "TargetGame", "Shootemup"};
                        sceneName = games[Random.Range(0, games.Length)];  // Get random game from list
                    }

                    if (GamesLeftToPlay > 0)  // If currently doing a sequence of games
                    {
                        MenuState = "MidSequenceMenu";
                        GamesLeftToPlay -= 1;
                        Debug.Log("Games left to play: " + GamesLeftToPlay);

                        if (GamesLeftToPlay <= 0) {MenuState = "BaseMenu";}  // Return to base menu when done with the sequence of games
                    }
            
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }
        }
    }
}
