using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour, IWeaponLevel
{
    [Header("Variables")]
    public float[] variables = new float[7];
    
    [SerializeField] LevelScriptableObject levelValues;

    private GameManager gameManager;

    private float increaseSpeed;

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
    private IEnumerator IncreaseScale()
    {
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            transform.localScale += Vector3.forward * (increaseSpeed * Time.deltaTime);

            if (timer >= variables[2]) { KnightBase.instance.earthQuakeShader.SetActive(false); gameObject.SetActive(false); yield break; }

            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Knight) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "DecreaseEnemySpeed", ID, variables[1], variables[2]);
            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, variables[0]);
        }
    }
    private void OnDisable()
    {
        transform.localScale = Vector3.one * 1; // YAP : buraya degisken gelecek
    }
    public void IncreaseLevel()
    {
        for (int i = 0; i < 7; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        KnightBase.instance.coolDowns[3] = variables[6];

        level++;
    }
    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(3, this);
    }
    public void OpenAbility(Vector3 m_spawnPoint, float m_angle)
    {
        gameObject.SetActive(true);

        increaseSpeed = variables[5] / variables[2];

        transform.position = m_spawnPoint;
        transform.eulerAngles = Vector3.up * m_angle;

        StartCoroutine(IncreaseScale());
    }
}
