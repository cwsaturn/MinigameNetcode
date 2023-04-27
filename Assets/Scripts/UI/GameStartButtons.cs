using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameStartButtons : MonoBehaviour
{
    private static string[] FullGameSequence = {"Platformer2", "Kart", "ShellGame", "Card", "TargetGame", "Shootemup"};
    private static string MenuState = "BaseMenu";
    private static string LastGamePlayed = "NULL";
    private static int FullSequenceGamesLeftToPlay = 0;
    private static int RandomGamesLeftToPlay = 0;
    public static GameObject gameNumberInputField; 

    void OnGUI()
    {

        if(!gameNumberInputField)
        {
            gameNumberInputField = GameObject.FindWithTag("GameNumberInputField");
        }
        // Top layer buttons
        if (MenuState == "BaseMenu")  
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            StartButton("StartFullSequence", "Full Game Sequence");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("StartRandomSequence", "Ramdom Game Sequence");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 70, 300, 300));
            StartButton("StartFreePlay", "Free Play Mode");
            GUILayout.EndArea();
        }
        
        // Buttons for random game sequence starting submenu
        else if (MenuState == "StartRandomSequenceMenu")  
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
            StartButton("CustomGame", "Custom");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 130, 300, 300));
            StartButton("LeaveSequence", "Back");
            GUILayout.EndArea();
        }

        // Buttons for mid random game sequence submenu
        else if (MenuState == "MidFullSequenceMenu")  
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            // Grammar is important kids
            if (FullSequenceGamesLeftToPlay != 1) {StartButton("ContinueFullSequence", "Continue - " + FullSequenceGamesLeftToPlay + " Games Left");}
            else {StartButton("ContinueFullSequence", "Continue - " + FullSequenceGamesLeftToPlay + " Game Left");}

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("EndSequence", "End");
            GUILayout.EndArea();
        }

        // Buttons for mid random game sequence submenu
        else if (MenuState == "MidRandomSequenceMenu")
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            // Grammar is still important kids
            if (RandomGamesLeftToPlay != 1) {StartButton("ContinueRandomSequence", "Continue - " + RandomGamesLeftToPlay + " Games Left");}
            else {StartButton("ContinueRandomSequence", "Continue - " + RandomGamesLeftToPlay + " Game Left");}

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 40, 300, 300));
            StartButton("EndSequence", "End");
            GUILayout.EndArea();
        }

        // Buttons for free play sub menu
        else  
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
                else if (sceneName == "StartRandomSequence") {MenuState = "StartRandomSequenceMenu";}

                // Quiting a sequence of games early
                else if (sceneName == "EndSequence")
                {
                    MenuState = "BaseMenu";
                    RandomGamesLeftToPlay = 0;
                    FullSequenceGamesLeftToPlay = 0;
                }

                // Game selection and transition
                else
                {
                    // Set count for sequence play
                    if (sceneName == "Sequence3") {RandomGamesLeftToPlay = 3;}
                    else if (sceneName == "Sequence5") {RandomGamesLeftToPlay = 5;}
                    else if (sceneName == "Sequence7") {RandomGamesLeftToPlay = 7;}

                    else if (sceneName == "StartFullSequence") {FullSequenceGamesLeftToPlay = FullGameSequence.Length;}
                    
                    if (sceneName == "CustomGame") 
                    {
                        gameNumberInputField.SetActive(true);
                        Debug.Log("Input field........");
                        Debug.Log(gameNumberInputField);
                        GameObject gn_input = gameNumberInputField.GetComponentInChildren<TMP_InputField>(true).gameObject;
                        gn_input.SetActive(true);
                        gn_input.GetComponent<TMP_InputField>().onSubmit.AddListener((gameNum) => SetCustomGame(sceneName, int.Parse(gameNum))); //Host enters ip address
                    }
                
                    // Get a random game to start if needed
                    if (sceneName == "RandomGame" || sceneName == "ContinueRandomSequence" || RandomGamesLeftToPlay > 0)
                    {
                        sceneName = FullGameSequence[Random.Range(0, FullGameSequence.Length)];  // Get random game from list

                        // I acknowledge that this is a very stupid way of doing this but the better way I tried had issues
                        while (sceneName == LastGamePlayed) 
                        {
                            sceneName = FullGameSequence[Random.Range(0, FullGameSequence.Length)];  // Prevent playing the same game twice in a row
                        }
                    }

                    // If currently doing a random sequence of games
                    if (RandomGamesLeftToPlay > 0)  
                    {
                        MenuState = "MidRandomSequenceMenu";  // This needs to be set the first time so it has to here
                        RandomGamesLeftToPlay -= 1;
                        Debug.Log("Random games left to play: " + RandomGamesLeftToPlay);

                        if (RandomGamesLeftToPlay <= 0) {MenuState = "BaseMenu";}  // Return to base menu when done with the sequence of games
                    }

                    // If currently doing a set sequence of games
                    if (sceneName == "ContinueFullSequence" || FullSequenceGamesLeftToPlay > 0)
                    {
                        MenuState = "MidFullSequenceMenu";
                        sceneName = FullGameSequence[FullSequenceGamesLeftToPlay - 1];
                        FullSequenceGamesLeftToPlay -= 1;

                        if (FullSequenceGamesLeftToPlay <= 0) {MenuState = "BaseMenu";}  // Return to base menu when done with the sequence of games
                    }

                    // The actual scene transition
                    LastGamePlayed = sceneName;
                    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }
        }
    }

    // Separate function is the easiest way to deal with waiting on input field submit 
    static void SetCustomGame(string sceneName, int gameNum)
    {
        RandomGamesLeftToPlay = gameNum - 1;
        sceneName = FullGameSequence[Random.Range(0, FullGameSequence.Length)];
        MenuState = "MidRandomSequenceMenu";
        LastGamePlayed = sceneName;
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
