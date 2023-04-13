using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CursorMovement : NetworkBehaviour
{

    // public float moveSpeed = 200.0f;

    private bool playerActive = true;
    private int position = 0;

    private float x_offset;
    private float y_offset;

    [SerializeField]
    private GameObject canvasName;

    [SerializeField]
    private GameObject sprite;

    private ShellGameManager cupManager;

    private bool locked = false;


    // Start is called before the first frame update
    IEnumerator Start()
    {

        x_offset = transform.position.x;
        y_offset = transform.position.y;

        if (IsServer)
        {
            cupManager = GameObject.Find("Game Manager").GetComponent<ShellGameManager>();

            while (!cupManager.getShuffleStatus()) { yield return new WaitForSecondsRealtime(0.1f); }


            yield return new WaitForSecondsRealtime(6);

            Debug.Log("locked in");
            playerActive = false;
            locked = true;
        }
        


    }

    // Update is called once per frame
    void Update()
    {

        if (!playerActive) return;

        if (Input.GetKeyDown("left") | (Input.GetKeyDown("a")))
        {
            if(position > -1)
            {
                position -= 1;
            }
        }

        if (Input.GetKeyDown("right") || Input.GetKeyDown("d"))
        {
            if (position < 1)
            {
                position += 1;
            }
        }

        transform.position = new Vector3(x_offset + 5 * position, y_offset, transform.position.z);

        // var targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // targetPos.z = transform.position.z;
        // transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public bool getLock()
    {
        return locked;
    }

}
