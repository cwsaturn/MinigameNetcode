using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupScript : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Cursor.visible = false;
        yield return new WaitForSecondsRealtime(1);
        transform.Translate(0, 2, 0);
        yield return new WaitForSecondsRealtime(1);
        transform.Translate(0, -2, 0);
        yield return new WaitForSecondsRealtime(25);
        transform.Translate(0, 2, 0);


    }

    // Update is called once per frame
    void Update()
    {

        
    }
}
