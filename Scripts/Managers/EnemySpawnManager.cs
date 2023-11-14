using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class EnemySpawnManager : MonoBehaviourPunCallbacks, IRestart
{
    public static EnemySpawnManager instance;
    public TMP_Text enemyCountText;
    public int enemyCount = 0;
    public PhotonView view;

    public bool canSpawn = false;

    [SerializeField] private Transform[] enemySpawnPoints;

    private PoolingSystemManager poolingSystem;
    private EnemyManager enemyManager;
    private LoadingManager loadingManager;

    [SerializeField] private GameObject[] enemyPrefabs;
    public List<GameObject> activeEnemyList = new List<GameObject>();

    private int[] enemiesCount = { 0, 0, 0, 0, 0, 0 };

    private float spider = 10, wolf = 5, snake = 3, golem = 0, orgwar = 0, tree = 0;

    private float x = 0.1f, y = 0.2f, z = 0.34f;

    private float duration = 0;
    private int waveCount = 0;
    private int totalEnemy;
    public int playerPower = 0;

    private void Awake()
    {
        instance = this;

        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        poolingSystem = PoolingSystemManager.instance;
        enemyManager = EnemyManager.instance;
        loadingManager = LoadingManager.instance;

        //RestartGameManager.instance.AddRestart(this);

        StartCoroutine(SpawnEnemyCoroutine());
    }

    public IEnumerator SpawnEnemyCoroutine()
    {
        if (!PhotonNetwork.IsMasterClient) yield break;

        yield return new WaitForEndOfFrame();

        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        while (canSpawn)
        {
            CalculateSpawnEnemyCount();
            for (int i = 0; i < enemiesCount.Length; i++)
            {
                for (int j = 0; j < enemiesCount[i]; j++)
                {
                    int spawnPointIndex = Random.Range(0, enemySpawnPoints.Length);
                    int playerType = Random.Range(0, GameManager.instance.players.Count);
                    RPCManager.instance.SendRPC(view, "SpawnEnemy", i, spawnPointIndex, playerType);
                    enemyCount++;
                    enemyCountText.text = enemyCount.ToString();
                    yield return new WaitForSeconds(.2f);
                }
            }
            yield return new WaitForSeconds(EnemyDuration());
        }
    }
    [PunRPC]
    public void SpawnEnemy(int m_enemyIndex, int m_spawnPointIndex, int m_playerType) // burasi degisken olarak enemyIndex ve spawnIndex almali
    {
        GameObject enemy = poolingSystem.OpenObject(enemyPrefabs[m_enemyIndex].name, enemySpawnPoints[m_spawnPointIndex].position);

        enemy.GetComponent<EnemyAI>().ChangeTarget(GameManager.instance.players[(PlayerType)m_playerType].transform);

        activeEnemyList.Add(enemy);
    }

    [PunRPC]
    public void CreatePoolingEnemyFromServer(string key, string ID)
    {
        GameObject obj = Instantiate(Resources.Load("Pooling/" + key) as GameObject, poolingSystem.transform);

        obj.GetComponent<PoolingID>().poolID = ID;

        poolingSystem.poolingObjects[key].Enqueue(obj);

        enemyManager.SaveEnemyID(ID, obj);

        obj.SetActive(false);
    }

    private void CalculateSpawnEnemyCount()
    {
        spider += spider * x;
        if (spider > 40)
        {
            spider = 40;
        }
        enemiesCount[0] = (int)spider;

        if (spider > 15)
        {
            wolf += wolf * y;
        }

        if (wolf > 5)
        {
            enemiesCount[1] = (int)wolf;
        }

        if (wolf > 10)
        {
            snake += snake * z;
        }

        if (snake > 3)
        {
            enemiesCount[2] = (int)snake;
        }

        if (snake > 20)
        {
            tree += 2;
        }

        if (tree > 0)
        {
            enemiesCount[3] = (int)tree;
        }
        if (tree > 10)
        {
            golem++;
        }

        if (golem > 0)
        {
            enemiesCount[4] = (int)golem;
        }

        if (golem > 5)
        {
            orgwar++;
        }

        if (orgwar > 0)
        {
            enemiesCount[5] = (int)orgwar;
        }
        waveCount += 1;
    }

    private float EnemyDuration()
    {
        int health = 3 * enemiesCount[2] + 2 * enemiesCount[1] + enemiesCount[0] + 15 * enemiesCount[4] + 20 * enemiesCount[3] + 10 * enemiesCount[5];
        totalEnemy = 0;
        for (int i = 0; i < enemiesCount.Length; i++)
        {
            totalEnemy += enemiesCount[i];
        }
        if (waveCount <= 10)
        {
            duration = totalEnemy / 2;
        }
        else
        {
            if (playerPower == 0) playerPower = 1;
            duration = health / playerPower;
        }
        return duration;
    }

    public void Restart()
    {
        for (int i = 0; activeEnemyList.Count > 0; i++)
        {
            activeEnemyList[0].GetComponent<EnemyAI>().SetEnemy();

            activeEnemyList.RemoveAt(0);
        }
    }

}
