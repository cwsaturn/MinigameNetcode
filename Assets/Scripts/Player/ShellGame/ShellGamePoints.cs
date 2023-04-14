using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShellGamePoints : NetworkBehaviour

{
    public GameObject winningCup;

    public Vector3 winningCupPosition;

    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    private CursorMovement mv;

    private float time = 0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        playerScript.SetDisappearingServerRpc(false);
        while (!mv.Locked) { yield return new WaitForSecondsRealtime(0.1f); }

        winningCup = GameObject.Find("Cup 1");
        winningCupPosition = winningCup.transform.position;
        winningCup.GetComponent<CupScript>().raise();

        yield return new WaitForSecondsRealtime(5);

        if (winningCupPosition.x - 1 < transform.position.x && winningCupPosition.x + 1 > transform.position.x)
        {
            Debug.Log("A cup won!");
            if(IsOwner)
            {
                playerScript.FinishedServerRpc(time);
            }
            
        }
        else
        {
            Debug.Log("A cup lost!");
            if(IsOwner)
            {
                playerScript.FinishedServerRpc(int.MaxValue);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!mv.Locked)
            time += Time.deltaTime;
    }

}
