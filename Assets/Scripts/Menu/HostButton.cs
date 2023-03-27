using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostButton : MonoBehaviour
{

    public GameObject ipInputField;
    public void OnButtonPress()
    {
  
      ipInputField.SetActive(true);
      ipInputField.GetComponent<InputField>().onSubmit.AddListener((ipAddr) => SetIpAddress(ipAddr)); //Host enters ip address
    }

    private void SetIpAddress(string ipAddr)
    {
      PlayerPrefs.SetString("HostIpAddr", ipAddr);
      PlayerPrefs.SetString("IsHost", "True");
      SceneManager.LoadScene("StartLobby", LoadSceneMode.Single);
    }
}
