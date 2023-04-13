using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGamePoints : MonoBehaviour

{
    public GameObject winningCup;

    public Vector3 winningCupPosition;

    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    private CursorMovement mv;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (!mv.getLock()) { yield return new WaitForSecondsRealtime(0.1f); }

        winningCup = GameObject.Find("Cup 1");
        winningCupPosition = winningCup.transform.position;
        winningCup.GetComponent<CupScript>().raise();

        yield return new WaitForSecondsRealtime(5);

        if (winningCupPosition.x - 1 < transform.position.x && winningCupPosition.x + 1 > transform.position.x)
        {
            Debug.Log("A cup won!");
            playerScript.FinishedServerRpc(10);
        }
        else
        {
            Debug.Log("A cup lost!");
            playerScript.FinishedServerRpc(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
