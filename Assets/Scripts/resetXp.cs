using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetXp : MonoBehaviour
{
    public void resetXpButton()
    {
        PlayerPrefs.SetFloat("xp", 0);
    }
}
