using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupScript : MonoBehaviour
{

    [SerializeField]
    public bool hasBall;

    [SerializeField]
    public GameObject ball;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    public bool raiseFlag = false;
    public bool lowerFlag = false;
    public bool shuffleFlag = false;

    public Vector3 targetpos;

    private float moveSpeed = 5f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Cursor.visible = false;
        yield return new WaitForSecondsRealtime(1);
        raiseFlag = true;
        yield return new WaitForSecondsRealtime(2);
        lowerFlag = true;
        yield return new WaitForSecondsRealtime(1);
        //Debug.Log("select your cup");
        yield return new WaitForSecondsRealtime(5);
        //transform.Translate(0, 2, 0);


    }


    // Update is called once per frame
    void Update()
    {
        if (raiseFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 2, transform.position.z), moveSpeed * Time.deltaTime);
            if (transform.position.y == 2)
            {
                raiseFlag = false;
            }
        }

        if (lowerFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0, transform.position.z), moveSpeed * Time.deltaTime);
            if (transform.position.y == 0)
            {
                lowerFlag = false;
            }
        }

        if(shuffleFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetpos, moveSpeed * Time.deltaTime);
            if (transform.position == targetpos)
            {
                shuffleFlag = false;
                if (hasBall)
                {
                    //Debug.Log("Ended : " + Time.time);
                    ball.SetActive(true);
                    ball.transform.position = new Vector3(targetpos.x, -1.2f, 20f);
                }
            }
        }

    }

    public void moveTo(Vector3 pos)
    {
        targetpos = pos;
        if (hasBall)
        {
            //Debug.Log("Started : " + Time.time);
            ball.SetActive(false);
        }
        shuffleFlag = true;
    }

    public bool getShuffleFlag()
    {
        return shuffleFlag;
    }

    public void raise()
    {
        raiseFlag = true;
    }
}


