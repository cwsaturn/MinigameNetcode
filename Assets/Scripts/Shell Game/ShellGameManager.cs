using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGameManager : MonoBehaviour
{

    public int NumberOfCups = 3;
    public GameObject[] Cups;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(3);
        Vector3 temp;
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSecondsRealtime(1);
            int x = Random.Range(0, 3);
            switch (x)
            {
                case 0:
                    temp = Cups[0].transform.position;
                    Cups[0].transform.position = Cups[1].transform.position;
                    Cups[1].transform.position = temp;
                    break;
                case 1:
                    temp = Cups[0].transform.position;
                    Cups[0].transform.position = Cups[2].transform.position;
                    Cups[2].transform.position = temp;
                    break;
                case 3:
                    temp = Cups[2].transform.position;
                    Cups[2].transform.position = Cups[1].transform.position;
                    Cups[1].transform.position = temp;
                    break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
