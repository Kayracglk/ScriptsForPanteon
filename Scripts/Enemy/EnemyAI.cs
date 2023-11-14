using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    public Dictionary<PlayerType, GameObject> players = new Dictionary<PlayerType, GameObject>();
    public EnemyName enemyName;
    public PhaseOfAI currentPhase;
    public PhaseOfAI prevPhase;

    [SerializeField] private Transform target;
    [SerializeField] private EnemyTypesEnum currentType;
    [SerializeField] private float stopDistance = 1f;


    private Renderer renderer;
    private Animator anim;
    private PoolingID poolingID;

    private float health = 1;
    private float walkSpeed;
    private int attackDamage;

    public bool canMove = true;
    public bool isBlocking = false;
    private bool[] blockBools = new bool[2];

    private Ray ray;
    private RaycastHit hit;

    public float WalkSpeed => walkSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        poolingID = GetComponent<PoolingID>();

        if (currentType != EnemyTypesEnum.Orc)
            renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();

        walkSpeed = EnemyManager.instance.enemyTypes[(int)currentType].walkSpeed;
        health = EnemyManager.instance.enemyTypes[(int)currentType].health;
        attackDamage = EnemyManager.instance.enemyTypes[(int)currentType].attackDamage;
    }


    private void Start()
    {
        players = GameManager.instance.players;
    }


    private void OnEnable()
    {
        health = EnemyManager.instance.enemyTypes[(int)currentType].health;

        ChangeMaterial();

        ChangePhase(PhaseOfAI.Idle);

        StartCoroutine(Movement());
    }

    private void OnDisable()
    {
        anim.SetBool("Attack", false);

        canMove = true;
    }

    public void AttackPlayer()
    {
        if (target == null) { return; }
        if (GameManager.instance.localPlayer != PlayerType.Knight) return;
        EnemyManager.instance.AttackedPlayer(target.gameObject, attackDamage);
    }

    private IEnumerator Movement()
    {
        yield return new WaitForEndOfFrame();

        ChangePhase(PhaseOfAI.Walk);

        canMove = true;

        while (true)
        {
            if (target == null) { yield return null; continue; }

            if (currentPhase != PhaseOfAI.Idle)
                transform.LookAt(target.position);

            if (!canMove) { yield return null; continue; }

            if (isBlocking) { yield return null; continue; }

            transform.position += transform.forward.normalized * (walkSpeed * Time.deltaTime);

            yield return null;
        }
    }
    private Transform CalculateClosestPlayer()
    {
        if (GameManager.instance.players.Count <= 0) return null;

        Transform target = GameManager.instance.players[0].transform;

        for (int i = 1; i < GameManager.instance.players.Count(); i++)
        {
            if (Vector3.Distance(transform.position, target.position) > Vector3.Distance(transform.position, GameManager.instance.players[(PlayerType)i].transform.position))
            {
                target = GameManager.instance.players[(PlayerType)i].transform;
            }
        }

        return target;
    }
    public void TakeDamage(float damage)
    {
        print("Ok test 1");

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void SetEnemy()
    {
        PoolingSystemManager.instance.SetObject(this.gameObject, enemyName.ToString());
    }    

    private void Die()
    {
        print("Ok test 2");

        EnemySpawnManager.instance.activeEnemyList.Remove(this.gameObject);
        EnemySpawnManager.instance.enemyCount--;
        EnemySpawnManager.instance.enemyCountText.text = EnemySpawnManager.instance.enemyCount.ToString();

        ChangePhase(PhaseOfAI.Die);

        SetEnemy();

        if (GameManager.instance.localPlayer != PlayerType.Knight) return;

        int oran = Random.Range(0, 100);

        RPCManager.instance.SendRPC(ObjectSpawnManager.instance.photonView, "SpawnOrbObject", transform.position.x, transform.position.y, transform.position.z, oran);
    }
    public void ChangeMaterial()
    {
        if (currentType != EnemyTypesEnum.Orc)
        {
            renderer.material = EnemyManager.instance.enemyTypes[(int)currentType].enemyMaterial[Random.Range(0, EnemyManager.instance.enemyTypes[(int)currentType].enemyMaterial.Length)];
        }
    }

    public void ChangeAttackAnimation(string m_key, int m_boolen)
    {
        if (m_boolen == 0)
        {
            if (currentPhase != PhaseOfAI.Idle)
                canMove = true;

            ChangePhase(PhaseOfAI.Walk);
            anim.SetBool(m_key, false);
        }
        else
        {
            canMove = false;
            ChangePhase(PhaseOfAI.Attack);
            anim.SetBool(m_key, true);
        }
    }

    public void AnimationIdle(string m_key, int m_boolen)
    {
        if (m_boolen == 0)
        {
            ChangePhase(PhaseOfAI.Walk);
            anim.SetBool(m_key, false);
        }
        else
        {
            ChangePhase(PhaseOfAI.Idle);
            anim.SetBool(m_key, true);
        }
    }
    public void FreezeEnemy()
    {
        if (gameObject.activeSelf == false) { return; }
        canMove = false;
        StartCoroutine(EnemyFreezeCooldown(10));
        RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "EnemyTriggerAnimtion", poolingID.poolID, "Idle", 1);
    }

    public void ChangeSpeed(float m_newSpeedValue)
    {
        walkSpeed = m_newSpeedValue;
    }
    IEnumerator EnemyFreezeCooldown(int time)
    {
        yield return new WaitForSeconds(time);
        canMove = true;
        RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "EnemyTriggerAnimtion", poolingID.poolID, "Idle", 0);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (target == null) return;

            //if (target.gameObject != other.gameObject)
            //{
            //    target = gameObject.transform;
            //}

            if (PhotonNetwork.IsMasterClient && currentPhase != PhaseOfAI.Idle)
                RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "ChangeAnimationWithID", poolingID.poolID, "Attack", 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //if (target.gameObject == other.gameObject)
            //{

            if (PhotonNetwork.IsMasterClient && currentPhase != PhaseOfAI.Idle)
                RPCManager.instance.SendRPC(EnemyManager.instance.photonView, "ChangeAnimationWithID", poolingID.poolID, "Attack", 0);
            //}
        }
    }

    public void ChangeTarget(Transform m_target)
    {
        target = m_target;
    }

    public void ChangePhase(PhaseOfAI nextPhase)
    {
        if (currentPhase == nextPhase) return;

        prevPhase = currentPhase;

        currentPhase = nextPhase;
    }
}
public enum EnemyName
{
    Cobra,
    Golem,
    Orc1,
    Orc2,
    Orc3,
    Spider,
    Treant,
    Wolf,
}
public enum PhaseOfAI
{
    Idle,
    Walk,
    Attack,
    Die,
}
