using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordRotation : MonoBehaviour, IWeaponLevel
{
    [Header("Variables")]
    public float[] variables = new float[4];

    [SerializeField] LevelScriptableObject levelValues;

    private GameManager gameManager;

    private int level;
    private PhotonView view;

    private void Awake()
    {
        SaveInsideOfManager();
        transform.parent = PlayerWeapons.instance.transform;
        gameObject.SetActive(false);
        view = GetComponent<PhotonView>();

    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnEnable()
    {
        transform.parent = GameManager.instance.GetPlayerWithType(PlayerType.Knight).transform;
        transform.position = GameManager.instance.GetPlayerWithType(PlayerType.Knight).transform.position;

        transform.localScale = Vector3.one*variables[1];

        StartCoroutine(LifeTime());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Knight) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();

            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, variables[0]);
        }
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(variables[2]);

        gameObject.SetActive(false);

        KnightBase.instance.swordRotationParticle.SetActive(false);
    }

    public void IncreaseLevel()
    {
        for (int i = 0; i < 4; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        KnightBase.instance.coolDowns[1] = variables[3];

        level++;
    }
    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(1, this);
    }
}
