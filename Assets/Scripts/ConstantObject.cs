using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstantObject : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject score;
    [SerializeField]
    private TextMeshProUGUI scoreString;

    [SerializeField]
    private GameObject networkManagerObj;

    private string sceneName;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu")
        {
            Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
            /*
            GameObject[] networkManagers = GameObject.FindGameObjectsWithTag("NetworkManager");
            foreach(GameObject networkManager in networkManagers)
            {
                Destroy(networkManager);
            }
            */
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneName == "MainMenu") return;

        if (Input.GetKeyDown("escape"))
        {
            if(!Cursor.visible)
            {
                Cursor.visible = true;
            }

            menu.SetActive(!menu.activeSelf);
        }

        if (Input.GetKeyDown("tab"))
        {
            string scoreList = "";
            GameObject[] players = GameObject.FindGameObjectsWithTag("UniversalPlayer");
            foreach (GameObject player in players) 
            {
                UniversalPlayer universalPlayer = player.GetComponent<UniversalPlayer>();
                string score = universalPlayer.Username + " : " + universalPlayer.playerScore.Value.ToString();
                scoreList = scoreList + score + "\n";
            }
            scoreString.text = scoreList;
            score.SetActive(true);
        }

        if (Input.GetKeyUp("tab"))
        {
            score.SetActive(false);
        }
    }

    public void Exit()
    {
        /*
        Destroy(networkManagerObj);
        networkManagerObj = null;
        */

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }

    public bool activated()
    {
        return menu.activeSelf;
    }
}
