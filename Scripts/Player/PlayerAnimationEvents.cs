using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private RPCManager rpcManager;
    private GameManager gameManager;
    private KnightBase knightBase;
    private ArcherBase archerBase;
    private PhotonView view;
    private Animator animator;
    private void Awake()
    {
        rpcManager = RPCManager.instance;
        knightBase = KnightBase.instance;
        archerBase = ArcherBase.instance;
        gameManager = GameManager.instance;

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(start());
    }

    #region Knight
    public void KnightSpawnBoomerang()
    {
        if (gameManager.localPlayer != PlayerType.Knight) return;

        knightBase.transform.rotation = Quaternion.Euler(transform.rotation.x, knightBase.RotationY, transform.rotation.z);

        rpcManager.SendRPC(view, "OpenBoomerang", knightBase.skillSpawnPoints[0].position.x, knightBase.skillSpawnPoints[0].position.y, knightBase.skillSpawnPoints[0].position.z, knightBase.skillSpawnPoints[0].eulerAngles.y); // YAP : herkese yollamasina gerek olmayabilir
        SkillCoolDown.instance.SkillUsed(0, PlayerType.Knight);

        StartCoroutine(knightBase.SkillCooldown(1));
    }

    public void KnightSwordRotation()
    {
        if (gameManager.localPlayer != PlayerType.Knight) return;

        rpcManager.SendRPC(view, "OpenSwordRotation");
        SkillCoolDown.instance.SkillUsed(1, PlayerType.Knight);

        StartCoroutine(knightBase.SkillCooldown(2));
    }

    public void KnightLighting()
    {
        if (gameManager.localPlayer != PlayerType.Knight) return;

        for (int i = 0; i < knightBase.variables[1]; i++)
        {
            GameObject closestEnemy = EnemyManager.instance.GetClosestEnemy(transform);

            if (closestEnemy)
            {
                rpcManager.SendRPC(view, "OpenLighting", closestEnemy.GetComponent<PoolingID>().poolID, knightBase.variables[0], knightBase.variables[2], knightBase.variables[3]);
            }
        }

        SkillCoolDown.instance.SkillUsed(2, PlayerType.Knight);

        StartCoroutine(knightBase.SkillCooldown(3));
    }

    public void KnightEarthquake()
    {
        if (gameManager.localPlayer != PlayerType.Knight) return;

        knightBase.transform.rotation = Quaternion.Euler(transform.rotation.x, knightBase.RotationY, transform.rotation.z);

        rpcManager.SendRPC(knightBase.photonView, "OpenEarthquake", knightBase.skillSpawnPoints[3].position.x, knightBase.skillSpawnPoints[3].position.y, knightBase.skillSpawnPoints[3].position.z, knightBase.skillSpawnPoints[3].eulerAngles.y);
        SkillCoolDown.instance.SkillUsed(3, PlayerType.Knight);

        StartCoroutine(knightBase.SkillCooldown(4));
    }
    #endregion

    #region Archer
    public void ArcherBombArrow()
    {
        if (gameManager.localPlayer != PlayerType.Arhcer) return;

        archerBase.transform.rotation = Quaternion.Euler(transform.rotation.x, archerBase.RotationY, transform.rotation.z);

        rpcManager.SendRPC(view, "OpenBombArrow", archerBase.skillSpawnPoints[1].position.x, archerBase.skillSpawnPoints[1].position.y, archerBase.skillSpawnPoints[1].position.z, archerBase.skillSpawnPoints[1].eulerAngles.y);

        SkillCoolDown.instance.SkillUsed(2, PlayerType.Arhcer);

        StartCoroutine(archerBase.SkillCooldown(3));
    }

    public void ArcherUlt()
    {
        if (gameManager.localPlayer != PlayerType.Arhcer) return;

        archerBase.transform.rotation = Quaternion.Euler(transform.rotation.x, archerBase.RotationY, transform.rotation.z);
        print("Soa test 0");
        rpcManager.SendRPC(view, "OpenSovaUlt", archerBase.skillSpawnPoints[3].position.x, archerBase.skillSpawnPoints[3].position.y, archerBase.skillSpawnPoints[3].position.z, archerBase.skillSpawnPoints[3].eulerAngles.y);
        print("Soa test 1");
        SkillCoolDown.instance.SkillUsed(3, PlayerType.Arhcer);
        print("Soa test 2");

        StartCoroutine(archerBase.SkillCooldown(4));
        print("Soa test 3");
    }

    public void SetArcherPosition()
    {
        archerBase.transform.eulerAngles = new Vector3(archerBase.transform.rotation.x, gameManager.anglesOfArrow[gameManager.arrowCount], archerBase.transform.rotation.z);
    }
    #endregion
    public void StopMovementPlayer()
    {
        PlayerMovement player = gameManager.GetPlayerWithType(gameManager.localPlayer).GetComponent<PlayerMovement>();

        player.CanMove = false;
    }

    public void StartMovementPlayer()
    {
        PlayerMovement player = gameManager.GetPlayerWithType(gameManager.localPlayer).GetComponent<PlayerMovement>();

        player.CanMove = true;

        transform.parent.localEulerAngles = Vector3.zero;
    }

    private IEnumerator start()
    {
        while (!LoadingManager.instance.canStartGame)
        {
            yield return null;
        }

        if (gameManager.localPlayer == PlayerType.Knight)
            view = knightBase.photonView;
        else
            view = archerBase.photonView;
    }
}
