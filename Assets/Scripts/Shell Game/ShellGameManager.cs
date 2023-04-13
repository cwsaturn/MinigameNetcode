using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGameManager : MonoBehaviour
{
    [SerializeField]
    public CupScript[] Cups;

    private int numSwaps = 20;

    private bool doneShuffling = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Random.InitState(42);
        yield return new WaitForSecondsRealtime(5);
        Vector3 temp;
        for (int i = 0; i < numSwaps; i++)
        {
            int x = Random.Range(0, 3);
            switch (x)
            {
                case 0:
                    while (Cups[0].getShuffleFlag() || Cups[1].getShuffleFlag() || Cups[2].getShuffleFlag()) { yield return new WaitForSecondsRealtime(0.1f); } // wait until not shuffling
                    temp = Cups[0].transform.position;
                    Cups[0].moveTo(Cups[1].transform.position);
                    Cups[1].moveTo(temp);
                    Debug.Log(Cups[0].getShuffleFlag());
                    //yield return new WaitForSecondsRealtime(1.2f);
                    break;
                case 1:
                    while (Cups[0].getShuffleFlag() || Cups[1].getShuffleFlag() || Cups[2].getShuffleFlag()) { yield return new WaitForSecondsRealtime(0.1f); } // wait until not shuffling
                    temp = Cups[0].transform.position;
                    Cups[0].moveTo(Cups[2].transform.position);
                    Cups[2].moveTo(temp);
                    //yield return new WaitForSecondsRealtime(2.4f);
                    break;
                case 2:
                    while (Cups[0].getShuffleFlag() || Cups[1].getShuffleFlag() || Cups[2].getShuffleFlag()) { yield return new WaitForSecondsRealtime(0.1f); } // wait until not shuffling
                    temp = Cups[1].transform.position;
                    Cups[1].moveTo(Cups[2].transform.position);
                    Cups[2].moveTo(temp);
                    //yield return new WaitForSecondsRealtime(1.2f);
                    break;
                default:
                    Debug.Log("how???");
                    break;
            }
        }
        //Debug.Log("Done shuffling");
        while (Cups[0].getShuffleFlag() || Cups[1].getShuffleFlag() || Cups[2].getShuffleFlag()) { yield return new WaitForSecondsRealtime(0.1f); } // wait until not shuffling
        doneShuffling = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getShuffleStatus()
    {
        return doneShuffling;
    }
}
