using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGamePoints : MonoBehaviour

{
    [SerializeField]
    Collider2D boxCollider;

    public Vector3 winningCup;

    [SerializeField]
    private PlayerScript playerScript;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        boxCollider.isTrigger = true;

        yield return new WaitForSecondsRealtime(30);

        winningCup = GameObject.Find("Cup 1").transform.position;

        yield return new WaitForSecondsRealtime(5);

        if (winningCup.x - 1 < transform.position.x && winningCup.x + 1 > transform.position.x)
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
