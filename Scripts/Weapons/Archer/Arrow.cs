using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public ArrowType arrowType;
    private GameManager gameManager;


    [SerializeField] private float speed; // YAP : buralar scriptableObject olmali . ayrica bombali okun kendi speedi olacak ve normal okun
    [SerializeField] private float gravity; // YAP : buralar scriptableObject olmali . ayrica bombali okun kendi speedi olacak ve normal okun
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        transform.position -= Vector3.up * (gravity * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Arhcer) return;

        if (other.CompareTag("Enemy"))
        {
            print("Ok enemy ile temas etti");

            EnemyAI enemy = other.GetComponent<EnemyAI>();
            string ID = enemy.GetComponent<PoolingID>().poolID;

            if (arrowType == ArrowType.defaultArrow)
                RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, ArcherBase.instance.variables[0]);
        }

        if (other.CompareTag("Ground"))
        {
            if (arrowType != ArrowType.defaultArrow)
            {
                PlayerWeapons.instance.bombArrow.transform.position = transform.position;
                PlayerWeapons.instance.bombArrow.SetActive(true);
            }

            PoolingSystemManager.instance.SetObject(gameObject, "Arrow");
        }
    }
}
public enum ArrowType
{
    defaultArrow,
    bombArrow,
}