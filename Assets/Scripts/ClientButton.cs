using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientButton : MonoBehaviour
{
    public void OnButtonPress()
    {
      PlayerPrefs.SetString("IsHost", "False");
      SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
    }
}
