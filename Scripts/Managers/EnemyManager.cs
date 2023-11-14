using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    public static EnemyManager instance;

    private List<Transform> enemyTransforms = new List<Transform>();

    public Dictionary<string, GameObject> enemyObjects = new Dictionary<string, GameObject>();

    public EnemyType[] enemyTypes;
    private void Awake()
    {
        instance = this;
    }
    public void AddEnemy(Transform m_enemyTransform)
    {
        enemyTransforms.Add(m_enemyTransform);
    }

    public void RemoveEnemy(Transform m_enemyTransform)
    {
        if (enemyTransforms.Contains(m_enemyTransform))
            enemyTransforms.Remove(m_enemyTransform);
    }

    public void SaveEnemyID(string m_ID, GameObject m_enemyObject)
    {
        enemyObjects.Add(m_ID, m_enemyObject);
    }
    public GameObject GetClosestEnemy(Transform m_player)
    {
        if (enemyTransforms.Count <= 0) return null;

        float distance = Vector3.Magnitude(m_player.position - enemyTransforms[0].position);
        float tempDistance;

        GameObject returnValue = enemyTransforms[0].gameObject;

        for (int i = 1; i < enemyTransforms.Count; i++)
        {
            tempDistance = Vector3.Magnitude(m_player.position - enemyTransforms[i].position);

            if (tempDistance < distance)
            {
                distance = tempDistance;
                returnValue = enemyTransforms[i].gameObject;
            }
        }
        RemoveEnemy(returnValue.transform);
        return returnValue;
    }
    public GameObject GetEnemyWithID(string m_ID)
    {

        return enemyObjects[m_ID].gameObject;
    }
    [PunRPC]
    public void GiveDamageWithID(string m_ID, float m_damage)
    {
        GameObject enemy = GetEnemyWithID(m_ID);

        PoolingSystemManager.instance.OpenObject("EnemyDamagePartical", enemy.transform.position, 1);

        enemy.GetComponent<EnemyAI>().TakeDamage(m_damage);
    }
    [PunRPC]
    public void ChangeAnimationWithID(string m_ID, string m_animation, int m_boolen)
    {
        GameObject enemy = GetEnemyWithID(m_ID);

        enemy.GetComponent<EnemyAI>().ChangeAttackAnimation(m_animation, m_boolen);
    }
    [PunRPC]
    public void EnemyTriggerAnimtion(string m_ID, string m_animation, int m_boolen)
    {
        GameObject enemy = GetEnemyWithID(m_ID);

        enemy.GetComponent<EnemyAI>().AnimationIdle(m_animation, m_boolen);

    }
    [PunRPC]
    public void DecreaseEnemySpeed(string m_ID, float m_slowRate, float m_slowTime)
    {
        StartCoroutine(ChangeEnemySpeed(m_ID, m_slowRate, m_slowTime));
    }

    public IEnumerator ChangeEnemySpeed(string m_ID, float m_slowRate, float m_slowTime)
    {
        GameObject enemy = GetEnemyWithID(m_ID);
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();

        float firstSpeedOfEnemy = enemyAI.WalkSpeed;

        float tempSlowRate = (firstSpeedOfEnemy * (100 - m_slowRate)) / 100;

        enemyAI.ChangeSpeed(m_slowRate);

        yield return new WaitForSeconds(m_slowTime);

        enemyAI.ChangeSpeed(firstSpeedOfEnemy);

    }
    public void AttackedPlayer(GameObject target, int damage)
    {
        RPCManager.instance.SendRPC(photonView, "AttackPlayer", (int)target.gameObject.GetComponent<PlayerCurrentType>().currentType, damage);
        PoolingSystemManager.instance.OpenObject("EnemyDamagePartical",target.transform.position,1);
    }
    [PunRPC]
    public void AttackPlayer(int type, int attackDamage)
    {
        GameManager.instance.players[(PlayerType)type].GetComponent<PlayerHealth>().TakeDamage(attackDamage);
    }

    IEnumerator ParticleReset(GameObject partical)
    {
        yield return new WaitForSeconds(1);
        PoolingSystemManager.instance.SetObject(partical, "EnemyDamagePartical");
    }
}
