using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupScript : MonoBehaviour
{

    [SerializeField]
    public int points;

    [SerializeField]
    BoxCollider2D boxCollider;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        boxCollider.isTrigger = true;

        // Cursor.visible = false;
        yield return new WaitForSecondsRealtime(1);
        transform.Translate(0, 2, 0);
        yield return new WaitForSecondsRealtime(1);
        transform.Translate(0, -2, 0);
        yield return new WaitForSecondsRealtime(25);
        Debug.Log("select your cup");
        yield return new WaitForSecondsRealtime(5);
        transform.Translate(0, 2, 0);


    }

    void OnTriggerStay2D(Collider2D collision)
    {
        boxCollider.isTrigger = false;
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


