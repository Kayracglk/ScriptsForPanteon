using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : MonoBehaviour, IWeaponLevel
{
    public static BombArrow instance;

    [Header("Variables")]
    public float[] variables = new float[6];

    [SerializeField] LevelScriptableObject levelValues;

    public int level = 0;

    private GameManager gameManager;
    private PhotonView view;
    private void Awake()// YAP : Max levelde 2 adet atacak
    {
        instance = this;

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
        yield return new WaitForEndOfFrame();
        
        PoolingSystemManager.instance.OpenObject("NukeConeExplosionFire", transform.position, 1);

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            transform.localScale += Vector3.one * (25 * Time.deltaTime);

            if (transform.localScale.x >= variables[4]) { gameObject.SetActive(false); yield break; }

            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Arhcer) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "DecreaseEnemySpeed", ID, variables[3], 1f);
            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, variables[0]);
        }
    }
    private void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    public void IncreaseLevel()
    {
        for (int i = 0; i < 6; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        ArcherBase.instance.coolDowns[2] = variables[5];

        level++;
    }

    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(6, this);
    }
}
