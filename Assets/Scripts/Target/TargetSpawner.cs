using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TargetSpawner : NetworkBehaviour
{

    // Start is called before the first frame update
    [SerializeField]
    private GameObject target;
    private float initTime = 1f; 
    private float timer = 0f;
    
    private int totalTargets = 20; 
    private int currentTargets = 1; 

    public int hitTargets = 0; 
    public NetworkVariable<bool> finishGame = new NetworkVariable<bool>(false);

    void Start()
    {
        if(!NetworkManager.Singleton.IsHost) return; 
        Vector3 spawnLocation = new Vector3(Random.Range(-5,5),Random.Range(-3,3),0);
        GameObject spawnedTarget = Instantiate(target, spawnLocation, Quaternion.identity);
        spawnedTarget.GetComponent<NetworkObject>().Spawn(); 
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > initTime && currentTargets != totalTargets)
        {
            CreateTargetServerRpc();
            timer -= initTime;
        }
        
    }

    [ServerRpc]
    void CreateTargetServerRpc()
    {
        Vector3 spawnLocation = new Vector3(Random.Range(-5,5),Random.Range(-3,3),0);
        GameObject newTarget = Instantiate(target, spawnLocation, Quaternion.identity);
        newTarget.GetComponent<NetworkObject>().Spawn(); 
        currentTargets++; 
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHitTargetsServerRpc()
    {
        hitTargets += 1; 
        if(hitTargets >= totalTargets)
        {
            finishGame.Value = true; 
        }
    }




}
