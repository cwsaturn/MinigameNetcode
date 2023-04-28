using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    public GameObject ipInputField;
    public GameObject portInputField; 

    public GameObject localButton;
    public GameObject lanButton; 
    public GameObject portFwdButton; 
    
    private bool usePortForwarding = false; 

    public void OnButtonPress()
    {
      localButton.GetComponent<Button>().onClick.AddListener(() => SetIpAddress("127.0.0.1"));
      lanButton.GetComponent<Button>().onClick.AddListener(() => ShowIPInputField(false));
      portFwdButton.GetComponent<Button>().onClick.AddListener(() => ShowIPInputField(true));
    }

    private void ShowIPInputField(bool portForwarding)
    {
      if(portForwarding) { usePortForwarding = true;}
      SetButtonsNotActive();
      ipInputField.SetActive(true);
      ipInputField.GetComponent<InputField>().onSubmit.AddListener((ipAddr) => SetIpAddress(ipAddr)); 
    }

    private void ShowPortInputField()
    {
      ipInputField.SetActive(false);
      portInputField.SetActive(true);
      portInputField.GetComponent<InputField>().onSubmit.AddListener((port) => SetPort(port)); //Host enters port
    }

    private void SetIpAddress(string ipAddr)
    {
      SetButtonsNotActive();
      PlayerPrefs.SetString("HostIpAddr", ipAddr);
      PlayerPrefs.SetString("IsHost", "False");
      if(!usePortForwarding)
      {
        PlayerPrefs.SetString("HostPort", "7777");
        SceneManager.LoadScene("StartLobby", LoadSceneMode.Single);
      }
      else { ShowPortInputField(); }
    }

    private void SetPort(string port)
    {
      PlayerPrefs.SetString("HostPort", port);
      SceneManager.LoadScene("StartLobby", LoadSceneMode.Single);
    }
    
    private void SetButtonsNotActive()
    {
      localButton.SetActive(false);
      lanButton.SetActive(false);
      portFwdButton.SetActive(false);
    }
}
