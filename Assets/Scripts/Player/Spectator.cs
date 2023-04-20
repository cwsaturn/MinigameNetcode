using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator
{
    public bool spectating = false;
    private int playerSpectating = 0;
    private PlayerScript playerSpectatingObj = null;
    private bool noPlayers = false;

    private void SwitchPlayerSpectating()
    {
        /*
         * CRASHES ???
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int previousSpectating = playerSpectating;
        playerSpectating = (playerSpectating + 1) % players.Length;

        while (previousSpectating != playerSpectating)
        {
            GameObject player = players[playerSpectating];
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            if (!playerScript.PlayerFinished)
            {
                Debug.Log("player found: " + playerSpectating);
                Transform playerTransform = playerScript.Rigidbody2D.transform;
                Camera.main.GetComponent<CameraScript>().setTarget(playerTransform);
                playerSpectatingObj = playerScript;
                return;
            }
            playerSpectating = (playerSpectating + 1) % players.Length;
        }
        playerSpectating = -1;
        */

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerScript tempPlayerScript;
        foreach (GameObject player in players)
        {
            tempPlayerScript = player.GetComponent<PlayerScript>();
            if (tempPlayerScript == null) continue;
            if (!tempPlayerScript.PlayerFinished)
            {
                Debug.Log("player found: " + playerSpectating);
                Camera.main.GetComponent<CameraScript>().setTarget(player.transform);
                playerSpectatingObj = tempPlayerScript;
                return;
            }
        }
        noPlayers = true;
        return;
    }

    public void Update()
    {
        if (!spectating) return;
        if (noPlayers) return;

        if (playerSpectatingObj == null)
        {
            SwitchPlayerSpectating();
            return;
        }

        if (playerSpectatingObj.PlayerFinished || Input.GetButtonDown("Jump"))
        {
            SwitchPlayerSpectating();
        }
    }
}
