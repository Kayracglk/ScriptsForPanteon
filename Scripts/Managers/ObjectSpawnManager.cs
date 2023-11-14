using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class ObjectSpawnManager : MonoBehaviourPunCallbacks, IRestart
{
    public static ObjectSpawnManager instance;

    public PhotonView view;

    public bool canSpawn = false;

    [SerializeField] private Transform[] spawnPoints;

    private PoolingSystemManager poolingSystem;

    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
    public int enemyXp = 1;



    private void Awake()
    {
        instance = this;

        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        poolingSystem = PoolingSystemManager.instance;

        //RestartGameManager.instance.AddRestart(this);
        //StartCoroutine(SpawnEnemyCoroutine());
    }
    public void SaveObjectDict(string ID, GameObject obj)
    {
        objects.Add(ID, obj);
    }
    public GameObject GetObjectID(string m_ID)
    {
        return objects[m_ID];
    }
    public IEnumerator SpawnEnemyCoroutine()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        while (canSpawn)
        {
            for (int i = 0; i < 5; i++)
            {
                int enemyIndex = Random.Range(0, poolingSystem.pool.Length);
                int spawnPointIndex = Random.Range(0, spawnPoints.Length);
                int playerType = Random.Range(0, GameManager.instance.players.Count);

                RPCManager.instance.SendRPC(view, "SpawnEnemy", enemyIndex, spawnPointIndex, 0);

                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    public void Restart()
    {
        int objCount = objects.Count;

        List<GameObject> tempList = objects.Values.ToList();

        for (int i = 0; i < objCount; i++)
        {
            
            string name;
            if (tempList[i].GetComponent<DropName>() == null) return;
                name = tempList[i].GetComponent<DropName>().currentName;

            print(tempList.Count);

            PoolingSystemManager.instance.SetObject(tempList[i], name);
        }
    }
    [PunRPC]
    public void SpawnObj(int m_enemyIndex, int m_spawnPointIndex, int m_playerType) // burasi degisken olarak enemyIndex ve spawnIndex almali
    {
        GameObject obj = poolingSystem.OpenObject(poolingSystem.pool[m_enemyIndex].prefab.name, spawnPoints[m_spawnPointIndex].position);
    }

    /// <summary>
    /// Buradaki x y z olen enemy positionu
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    [PunRPC]
    public void SpawnOrbObject(float x, float y, float z, int oran)
    {
        if (oran < 60)
        {
            GameObject obj = poolingSystem.OpenObject("LevelIncreaseOrb", new Vector3(x, y, z));
        }
        else if (oran < 80)
        {
            // bilerek bos birakilimistir.
        }
        else if (oran < 100)
        {
            GameObject obj = poolingSystem.OpenObject("Elmas", new Vector3(x, y, z));
        }
    }

    [PunRPC]
    public void CreatePoolingObjectFromServer(string key, string ID)
    {
        GameObject obj = Instantiate(Resources.Load("Pooling/" + key) as GameObject, poolingSystem.transform);

        obj.GetComponent<PoolingID>().poolID = ID;

        poolingSystem.poolingObjects[key].Enqueue(obj);

        SaveObjectDict(ID, obj);

        obj.SetActive(false);
    }
    [PunRPC]
    public void TakeOrbWithID(string m_ID, int type)
    {
        GameObject orb = GetObjectID(m_ID);

        PoolingSystemManager.instance.SetObject(orb, "LevelIncreaseOrb");

        GameManager.instance.GetPlayerWithType((PlayerType)type).GetComponent<PlayerLevel>().AddXp(enemyXp);
    }
}
