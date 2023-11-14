using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionDetect : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;

    private float timer = 0;

    private GameObject collidedObject;
    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (timer < 1) return;

        if (other.CompareTag("Enemy") && other.gameObject != enemyAI.gameObject || other.CompareTag("Player"))
        {
            enemyAI.isBlocking = true;

            collidedObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && other.gameObject == collidedObject || other.CompareTag("Player"))
        {
            enemyAI.isBlocking = false;
        }
    }
}
