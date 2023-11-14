using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Xml;
using Unity.VisualScripting;
using Photon.Pun.Demo.SlotRacer.Utils;

public class DropSpawnManager : MonoBehaviourPunCallbacks
{
    public static DropSpawnManager instance;
    private PoolingSystemManager poolingSystem;
    private LoadingManager loadingManager;

    public PhotonView view;

    public bool canSpawn = false;


    [SerializeField] private string dropSpawnObject;

    [SerializeField] private Transform dropSpawnPoints;

    private Dictionary<string, GameObject> dropList = new Dictionary<string, GameObject>();

    private Queue<Transform> dropQueue = new Queue<Transform>();

    private Dictionary<string, Transform> dropDic = new Dictionary<string, Transform>();



    private void Awake()
    {
        instance = this;

        for (int i = 0; i < dropSpawnPoints.childCount; i++)
        {
            dropQueue.Enqueue(dropSpawnPoints.GetChild(i).transform);

        }

        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        poolingSystem = PoolingSystemManager.instance;
        loadingManager = LoadingManager.instance;

        StartCoroutine(SpawnDropCoroutine());
        StartCoroutine(SpawnSandikCoroutine());
    }

    public void AddDrop(string key, GameObject obj)
    {
        dropList.Add(key, obj);
    }
    public void RemoveDrop(string key)
    {
        dropList.Remove(key);
    }

    public GameObject GetDropWithID(string m_ID)
    {
        return dropList[m_ID].gameObject;
    }

    public IEnumerator SpawnDropCoroutine()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        yield return new WaitForEndOfFrame();

        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        while (true)
        {
            RPCManager.instance.SendRPC(view, "SpawnObject", dropSpawnObject);
            float spawnTime = UnityEngine.Random.Range(25f, 40.0f);
            yield return new WaitForSeconds(spawnTime);
        }
    }
    public IEnumerator SpawnSandikCoroutine()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        yield return new WaitForEndOfFrame();

        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        while (true)
        {
            RPCManager.instance.SendRPC(view, "SpawnObject", "Sandik");
            float spawnTime = UnityEngine.Random.Range(90f, 120.0f);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    [PunRPC]
    public void SpawnObject(string m_objectName)
    {
        if (dropQueue.Count > 0)
        {
            Transform spawnPoint = dropQueue.Dequeue();

            GameObject dropObj = poolingSystem.OpenObject(m_objectName, spawnPoint.position);

            string ID = dropObj.GetComponent<PoolingID>().poolID;

            dropList.Add(ID, dropObj);

            dropDic.Add(ID, spawnPoint);

        }
    }

    [PunRPC]
    public void TakeDropWithID(string m_ID, int oran)
    {

        GameObject obj = ObjectSpawnManager.instance.GetObjectID(m_ID);
        if (oran < 40)
        {
            GameObject healthBar = PoolingSystemManager.instance.OpenObject("HealthItem", obj.transform.position);
            OpenObject(healthBar);
        }
        else if (oran < 0)
        {
            GameObject life = PoolingSystemManager.instance.OpenObject("EkstraLife", obj.transform.position);
            OpenObject(life);
        }
        else if (oran < 70)
        {
            GameObject freeze = PoolingSystemManager.instance.OpenObject("EnemyFreeze", obj.transform.position);
            OpenObject(freeze);
        }
        else
        {
            GameObject diamond = PoolingSystemManager.instance.OpenObject("Elmas", obj.transform.position);
            OpenObject(diamond);
        }
        PoolingSystemManager.instance.SetObject(obj, "Drop");
        dropQueue.Enqueue(dropDic[m_ID]);

        dropDic.Remove(m_ID);

        RemoveDrop(m_ID);
    }

    private void OpenObject(GameObject obje)
    {
        if (obje != null)
        {
            string ID = obje.GetComponent<PoolingID>().poolID;
            dropList.Add(ID, obje);
        }
    }

    [PunRPC]
    public void TakeHealth(string ID, int type)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "HealthItem");
        GameManager.instance.GetPlayerWithType((PlayerType)type).GetComponent<PlayerHealth>().GiveHealth(200);
    }
    [PunRPC]
    public void LevelUpgrade(string ID, int type)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "LevelUpgrade");
        GameManager.instance.GetPlayerWithType((PlayerType)type).GetComponent<PlayerLevel>().AddLevel();
    }
    [PunRPC]
    public void EkstraLife(string ID, int type)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "EkstraLife");
        print("Ekstra Can Verildi");
    }
    [PunRPC]
    public void EnemyFreeze(string ID)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "EnemyFreeze");
        foreach (GameObject i in EnemyManager.instance.enemyObjects.Values)
        {
            i.GetComponent<EnemyAI>().FreezeEnemy();
        }
    }

    [PunRPC]
    public void Diamond(string ID, int type)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "Elmas");
        GameManager.instance.GetPlayerWithType((PlayerType)type).GetComponent<PlayerMoney>().AddMoney(1);
    }
    [PunRPC]
    public void ParaCantasi(string ID, int type, int cash)
    {
        GameObject item = ObjectSpawnManager.instance.GetObjectID(ID);
        PoolingSystemManager.instance.SetObject(item, "ParaCantasi");
        GameManager.instance.GetPlayerWithType((PlayerType)type).GetComponent<PlayerMoney>().AddMoney(cash);
    }

    [PunRPC]
    public void TakeSandikWithID(string m_ID, int oran)
    {
        GameObject obj = ObjectSpawnManager.instance.GetObjectID(m_ID);
        if (oran < 50)
        {
            GameObject Level = PoolingSystemManager.instance.OpenObject("LevelUpgrade", obj.transform.position);
            OpenObject(Level);
        }
        else
        {
            GameObject paraCantasi = PoolingSystemManager.instance.OpenObject("ParaCantasi", obj.transform.position);
            OpenObject(paraCantasi);
        }
        PoolingSystemManager.instance.SetObject(obj, "Sandik");
        dropQueue.Enqueue(dropDic[m_ID]);
        dropDic.Remove(m_ID);
        RemoveDrop(m_ID);
    }

}
