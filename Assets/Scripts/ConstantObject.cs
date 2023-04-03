using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstantObject : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject canvas;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            canvas.SetActive(!canvas.activeSelf);
        }
    }

    public void Exit()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}
