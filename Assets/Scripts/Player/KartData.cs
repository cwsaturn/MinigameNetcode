using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KartData : NetworkBehaviour
{
    private float timePassed = 0f;

    private TextMeshProUGUI timeText;
    [SerializeField]
    private PlayerScript playerScript;

    [SerializeField]
    GameObject canvas;

    private bool playerActive = true;

    private void Awake()
    {
        //playerScript = GetComponent<PlayerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraScript>().setTarget(transform);
            timeText = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
        }

        //SyncNetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerActive) return;

        timePassed += Time.deltaTime;

        if (IsOwner)
            timeText.text = timePassed.ToString();

        canvas.transform.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision");

        if (!IsOwner) return;

        if (!playerActive) return;

        if (collision.gameObject.tag == "Finish")
        {
            playerScript.FinishedServerRpc(-timePassed);
            GetComponent<KartMovement>().active = false;
            playerActive = false;
            timeText.text = timePassed.ToString();
        }
    }
}
