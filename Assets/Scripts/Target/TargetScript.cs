using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TargetScript : NetworkBehaviour
{

    private Rigidbody2D rigidbody2d; 
    private bool triggered = false; 
    private Collider2D colliderObject; 
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.WakeUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(triggered)
        {
            if (Input.GetButtonDown("Jump"))
            {
                triggered = false; 
                ulong ownerClientId =  colliderObject.gameObject.GetComponent<NetworkObject>().OwnerClientId;
                DestroyTargetServerRpc();
                ScoredUpdateServerRpc(ownerClientId);
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void ScoredUpdateServerRpc(ulong ownerClientId)
    {
        GameObject[] listOfTargetCrosshairs = GameObject.FindGameObjectsWithTag("Player");
        GameObject targetCrosshair = findPlayer(listOfTargetCrosshairs, ownerClientId);
        targetCrosshair.GetComponent<TargetCrosshair>().UpdateTargetCrosshairScoreClientRpc(ownerClientId);
    }



    private GameObject findPlayer(GameObject[] listOfTargetCrosshairs, ulong ownerClientId)
    {
        if (!IsClient || IsServer)
        {
            foreach(GameObject targetCrosshair in listOfTargetCrosshairs)
            {
                if(targetCrosshair.GetComponent<NetworkObject>().OwnerClientId == ownerClientId)
                    return targetCrosshair;
            }
            Debug.Log("TargetScript::findPlayer - Didn't find a matching player!");
        }
        return null; 
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyTargetServerRpc()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        colliderObject = col; 
        triggered = true; 
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        colliderObject = null; 
        triggered = false; 
    }


}
