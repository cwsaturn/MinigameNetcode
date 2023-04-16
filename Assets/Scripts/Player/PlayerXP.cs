using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerXP : MonoBehaviour
{
    public TextMeshProUGUI XPText;

    // Start is called before the first frame update
    void Start()
    {
        XPText.text = "XP: " + PlayerPrefs.GetFloat("xp", 0).ToString();
    }
}
