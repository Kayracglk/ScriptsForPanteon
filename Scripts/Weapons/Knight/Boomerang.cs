using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviourPunCallbacks, IWeaponLevel
{
    [HideInInspector] public bool canCollideWithPlayer = false;

    [Header("Variables")]
    public float[] variables = new float[7];

    [SerializeField] LevelScriptableObject levelValues;

    private Transform boomerangModel;

    private GameManager gameManager;

    private int level;

    private float timer;

    private float killCount = 0;

    private PhotonView view;
    private void Awake()
    {
        SaveInsideOfManager();

        boomerangModel = transform.GetChild(0);

        transform.parent = PlayerWeapons.instance.transform;

        gameObject.SetActive(false);
        view = GetComponent<PhotonView>();

    }

    private void Update()
    {
        if (timer > variables[2]) { canCollideWithPlayer = true; transform.LookAt(KnightBase.instance.skillSpawnPoints[0].transform); }

        boomerangModel.eulerAngles += 1080 * Time.deltaTime * Vector3.up;

        transform.Translate(Vector3.forward * (variables[1] * Time.deltaTime));

        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.localPlayer != PlayerType.Knight) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            string ID = enemy.GetComponent<PoolingID>().poolID;

            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "DecreaseEnemySpeed", ID, variables[5], 1f);
            RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "GiveDamageWithID", ID, variables[0]);
        }
    }
    private void OnDisable()
    {
        canCollideWithPlayer = false;
        timer = 0;

        killCount = 0;
        transform.localScale = Vector3.one * variables[4];
    }
    public void IncreaseLevel()
    {
        for (int i = 0; i < 7; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        KnightBase.instance.coolDowns[0] = variables[6];

        level++;
    }
    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(0, this);
    }
    public void OpenAbility(Vector3 m_spawnPoint, float m_angle)
    {
        gameObject.SetActive(true);

        transform.position = m_spawnPoint;
        transform.eulerAngles = Vector3.up * m_angle;
    }
}

