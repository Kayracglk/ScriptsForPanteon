using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySaveToList : MonoBehaviour
{
    private EnemyAI enemyAI;
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyManager = EnemyManager.instance;

    }
    private void OnEnable()
    {
        StartCoroutine(onEnable());
    }

    private IEnumerator onEnable()
    {
        yield return new WaitForEndOfFrame();
        enemyManager.AddEnemy(transform);
    }
}
