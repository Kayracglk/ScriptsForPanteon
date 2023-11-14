using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovaUltCollider : MonoBehaviour
{
    private GameManager gameManager;
    private SovaUlt sovaUlt;
    private void Awake()
    {
        sovaUlt = transform.parent.GetComponent<SovaUlt>();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Arhcer) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();

            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, sovaUlt.variables[0]);
        }
    }
}
