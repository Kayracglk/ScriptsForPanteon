using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TakeDrop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Knight) return;

        if(other.gameObject.GetComponent<ParentType>() && gameObject.CompareTag("Sandik"))
        {
            string dropID = this.GetComponent<PoolingID>().poolID;
            int oran = Random.Range(0, 100);
            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "TakeSandikWithID", dropID, oran);
        }
        else if (other.gameObject.GetComponent<ParentType>())
        {
            string dropID = this.GetComponent<PoolingID>().poolID;
            int oran = Random.Range(0, 100);

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "TakeDropWithID", dropID, oran);
        }
        
    }
}
