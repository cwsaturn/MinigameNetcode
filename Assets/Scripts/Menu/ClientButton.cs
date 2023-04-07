using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    public GameObject ipInputField;
    public GameObject localButton;
    public GameObject lanButton; 
    public void OnButtonPress()
    {
      localButton.GetComponent<Button>().onClick.AddListener(() => SetIpAddress("127.0.0.1"));
      lanButton.GetComponent<Button>().onClick.AddListener(ShowInputField);
    }
   
    private void ShowInputField()
    {
      SetButtonsNotActive();
      ipInputField.SetActive(true);
      ipInputField.GetComponent<InputField>().onSubmit.AddListener((ipAddr) => SetIpAddress(ipAddr)); //Host enters ip address
    }

    private void SetIpAddress(string ipAddr)
    {
      PlayerPrefs.SetString("HostIpAddr", ipAddr);
      PlayerPrefs.SetString("IsHost", "False");
      SceneManager.LoadScene("StartLobby", LoadSceneMode.Single);
    }

    private void SetButtonsNotActive()
    {
      localButton.SetActive(false);
      lanButton.SetActive(false);
    }
}
