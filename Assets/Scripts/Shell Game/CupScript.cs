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
        Debug.Log("select your cup");


    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void OnPointerClick()
    {
        Debug.Log("on pointer click");
    }

    void OnSelect()
    {
        Debug.Log("on select");
    }

}


