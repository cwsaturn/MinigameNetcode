using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostButton : MonoBehaviour
{
    public void OnButtonPress()
    {
      PlayerPrefs.SetString("IsHost", "True");
      SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
    }
}
