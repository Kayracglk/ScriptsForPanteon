using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRain : MonoBehaviour, IWeaponLevel
{
    [Header("Variables")]
    public float[] variables = new float[5];

    [SerializeField] LevelScriptableObject levelValues;

    private GameManager gameManager;

    private int level;

    private PhotonView view;

    private void Awake() // YAP : distanceyi playerin icindeki yerdeki alana gore degistirme def : 10
    {
        SaveInsideOfManager();
        transform.parent = PlayerWeapons.instance.transform;
        gameObject.SetActive(false);

        view = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        StartCoroutine(IncreaseScale());
    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    private IEnumerator IncreaseScale()
    {
        yield return new WaitForSeconds(variables[3]);

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Arhcer) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, variables[0]);
        }
    }
    private void OnDisable()
    {
        transform.localScale = Vector3.one * variables[1];
    }
    public void IncreaseLevel()
    {
        for (int i = 0; i < 5; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        ArcherBase.instance.coolDowns[1] = variables[4];

        level++;
    }
    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(5, this);
    }
}