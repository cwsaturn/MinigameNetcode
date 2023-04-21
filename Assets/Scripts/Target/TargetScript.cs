using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TargetScript : NetworkBehaviour
{

    private Rigidbody2D rigidbody2d; 
    private Collider2D colliderObject;
    private float maxSize = 6.0f;
    private float growthRate = 1.0f;
    private float scale = 1.0f; 
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.WakeUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(scale < maxSize)
        {
            transform.localScale = new Vector3(1.0f,1.0f,1.0f) * scale;
            scale += growthRate * Time.deltaTime * 5;
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
    public void DestroyTargetServerRpc()
    {
        Destroy(gameObject);
        ulong ownerClientId = colliderObject.gameObject.GetComponent<NetworkObject>().OwnerClientId;
        ScoredUpdateServerRpc(ownerClientId); 
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        colliderObject = col; 
    }
}
