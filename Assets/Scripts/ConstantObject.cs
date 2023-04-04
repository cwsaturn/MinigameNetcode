using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstantObject : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject canvas;

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
        if (Input.GetKeyDown("escape") && sceneName != "MainMenu")
        {
            canvas.SetActive(!canvas.activeSelf);
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
}
