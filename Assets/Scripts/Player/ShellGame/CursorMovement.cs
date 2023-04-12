using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{

    // public float moveSpeed = 200.0f;

    private bool playerActive = true;
    private int position = 0;

    private float x_offset;
    private float y_offset;

    [SerializeField]
    BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;
        x_offset = transform.position.x;
        y_offset = transform.position.y;

        boxCollider.isTrigger = true;


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

    void OnTriggerStay2D(Collider2D collision)
    {
        boxCollider.isTrigger = false;
    }
}
