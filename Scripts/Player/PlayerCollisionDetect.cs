using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCollisionDetect : MonoBehaviour
{
    public GameObject revivePanel;

    private KnightBase knightBase;
    private ArcherBase archerBase;
    private PlayerCurrentType playerCurrentType;
    private PlayerHealth playerHealth;
    
    private PlayerType playerType;

    private void Awake()
    {
        playerCurrentType = GetComponent<PlayerCurrentType>();
        playerHealth = GetComponent<PlayerHealth>();

        playerType = playerCurrentType.currentType;

        if (playerCurrentType.currentType == PlayerType.Knight)
            knightBase = GetComponent<KnightBase>();
        else
            archerBase = GetComponent<ArcherBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerRevive"))
        {
            if (playerHealth.isPlayerAlive && !other.transform.parent.GetComponent<PlayerHealth>().isPlayerAlive)
                revivePanel.SetActive(true); 

            playerHealth.CollidedWithFriend = true;
        }

        if(other.CompareTag("Boomerang"))
        {
            if (playerType == PlayerType.Arhcer) return;

            if (!other.GetComponent<Boomerang>().canCollideWithPlayer) return;

            other.gameObject.SetActive(false);
        }

        if(other.CompareTag("LevelOrb"))
        {
            if (!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(ObjectSpawnManager.instance.photonView, "TakeOrbWithID", orbID, (int)playerType);
        }

        if (other.CompareTag("Health"))
        {
            if (!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "TakeHealth", orbID, (int)playerType);
        }

        if(other.CompareTag("LevelUpgrade"))
        {
            if(!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "LevelUpgrade", orbID, (int)playerType);
        }

        if(other.CompareTag("EkstraLife"))
        {
            if(!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "EkstraLife", orbID, (int)playerType);
        }

        if(other.CompareTag("EnemyFreeze"))
        {
            if(!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "EnemyFreeze", orbID);
        }

        if(other.CompareTag("Diamond"))
        {
            if(!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "Diamond", orbID, (int)playerType);
        }

        if(other.CompareTag("ParaCantasi"))
        {
            if(!PhotonNetwork.IsMasterClient) return;

            string orbID = other.GetComponent<PoolingID>().poolID;
            int cash = Random.Range(50, 100);
            RPCManager.instance.SendRPC(DropSpawnManager.instance.photonView, "ParaCantasi", orbID, (int)playerType, cash);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerRevive"))
        {
            revivePanel.SetActive(false);

            playerHealth.CollidedWithFriend = false;
        }
    }
}
