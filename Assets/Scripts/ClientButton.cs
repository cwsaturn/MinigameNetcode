using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    public GameObject ipInputField;

    public void OnButtonPress()
    {
      ipInputField.SetActive(true);
      ipInputField.GetComponent<InputField>().onSubmit.AddListener((ipAddr) => SetIpAddress(ipAddr)); //Client inputs host ip address
    }

    private void SetIpAddress(string ipAddr)
    {
      PlayerPrefs.SetString("HostIpAddr", ipAddr);
      PlayerPrefs.SetString("IsHost", "False");
      SceneManager.LoadScene("Scrap", LoadSceneMode.Single);
    }
}
