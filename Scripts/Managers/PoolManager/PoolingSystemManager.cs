using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PoolingSystemManager : MonoBehaviour
{
    public static PoolingSystemManager instance;

    public Dictionary<string, Queue<GameObject>> poolingObjects = new Dictionary<string, Queue<GameObject>>();

    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    [Serializable]
    public struct Pool
    {
        public GameObject prefab;
        public int size;
    }
    public Pool[] pool;

    private void Awake()
    {
        instance = this; 
    }
    private void Start()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            string key = pool[i].prefab.name;

            prefabs.Add(key, pool[i].prefab);
        }
        for (int i = 0; i < pool.Length; i++)
        {
            string key = pool[i].prefab.name;
            int size = pool[i].size;
            for (int j = 0; j < size; j++)
            {
                InstantiateObjects(key);

                if (!poolingObjects.ContainsKey(key))
                {
                    Queue<GameObject> queue = new Queue<GameObject>();
                    poolingObjects.Add(key, queue);
                }
            }
        }

        RPCManager.instance.SendRPC(LoadingManager.instance.photonView, "ChangePoolingOver",1);
    }
    public void InstantiateObjects(string key)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (prefabs[key].GetComponent<EnemyAI>())
            RPCManager.instance.SendRPC(EnemySpawnManager.instance.view, "CreatePoolingEnemyFromServer", key, Guid.NewGuid().ToString("N"));
        else
            RPCManager.instance.SendRPC(ObjectSpawnManager.instance.view, "CreatePoolingObjectFromServer", key, Guid.NewGuid().ToString("N"));
    }
    public GameObject OpenObject(string key, Vector3 pos)
    {
        GameObject particleObj = GetObject(key);
        particleObj.transform.position = pos;
        particleObj.SetActive(true);
        return particleObj;
    }
    public GameObject OpenObject(string key, Vector3 pos, Vector3 rotation)
    {
        GameObject particleObj = GetObject(key);
        particleObj.transform.position = pos;
        particleObj.transform.eulerAngles = rotation;
        particleObj.SetActive(true);
        return particleObj;
    }
    public GameObject OpenObject(string key, Vector3 pos, Transform parent)
    {
        GameObject particleObj = GetObject(key);
        particleObj.transform.position = pos;
        particleObj.transform.parent = parent;
        particleObj.SetActive(true);
        return particleObj;
    }
    public GameObject OpenObject(string key, Vector3 pos, float time)
    {
        GameObject particleObj = GetObject(key);
        particleObj.transform.position = pos;
        particleObj.SetActive(true);
        StartCoroutine(SetObject(particleObj, key, time));
        return particleObj;
    }
    public GameObject OpenObject(string key, Vector3 pos, Vector3 rotation, float time)
    {
        GameObject particleObj = GetObject(key);
        particleObj.transform.position = pos;
        particleObj.transform.eulerAngles = rotation;
        particleObj.SetActive(true);
        StartCoroutine(SetObject(particleObj, key, time));
        return particleObj;
    }
    //public IEnumerator OpenParticle(string key, Vector3 pos, float time)
    //{
    //    GameObject particleObj = GetParticle(key);
    //    if (particleObj == null) yield break;
    //    particleObj.SetActive(true);

    //    while (time > 0)
    //    {
    //        particleObj.transform.position = pos;
    //        time -= Time.deltaTime;
    //        yield return null;
    //    }

    //    poolingObjects[key].Push(particleObj);
    //    particleObj.SetActive(false);
    //}
    private IEnumerator SetObject(GameObject obj, string key, float time)
    {
        if (!poolingObjects.ContainsKey(key)) yield break;
        yield return new WaitForSeconds(time);
        poolingObjects[key].Enqueue(obj);
        obj.transform.parent = this.transform;
        obj.SetActive(false);
    }
    public GameObject GetObject(string key)
    {
        if (!poolingObjects.ContainsKey(key) || poolingObjects[key].Count <= 0) { print(key+" Obje Yok"); return null; }

        if (poolingObjects[key].Count <= 5)
            for (int i = 0; i < 5; i++)
            {
               InstantiateObjects(key);
            }

        GameObject particleObj = poolingObjects[key].Dequeue();
        return particleObj;
    }
    public void SetObject(GameObject obj, string key)
    {
        if (!poolingObjects.ContainsKey(key)) return;

        poolingObjects[key].Enqueue(obj);
        obj.transform.parent = this.transform;
        obj.SetActive(false);
    }
}
