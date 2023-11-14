using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcherBase : PlayerBase
{
    public static ArcherBase instance;

    [HideInInspector] public float tempYAngle;

    private SovaUlt sovaUlt;
    public float delay;
    [Space(10)]
    [Header("BurstArrowVariables")]
    public float[] variables = new float[6];

    public GameObject sovaUltiParticle;
    public override void Awake()
    {
        instance = this;

        base.Awake();

        GameManager.instance.PlayerSpawn(PlayerType.Arhcer, this.gameObject);
    }

    public override void Start()
    {
        base.Start();

        StartCoroutine(start());
    }
    #region Skills
    public override void Skill1(InputAction.CallbackContext context)
    {
        if (!loadingManager.canStartGame) return;

        if (playerMovement.IsStunned) return;

        if (!photonView.IsMine) return;

        if (!Skill1Available) { skillUse[0] = true; return; }

        if (context.phase == InputActionPhase.Canceled)
        {
            if (skillUse[0]) { skillUse[0] = false; return; }

            playerMovement.CanMove = false;

            transform.rotation = Quaternion.Euler(transform.rotation.x, RotationY, transform.rotation.z);

            //rpcManager.SendRPC(photonView, "OpenBurstArrow");

            OpenBurstArrow();

            SkillCoolDown.instance.SkillUsed(0, PlayerType.Arhcer);

            StartCoroutine(SkillCooldown(1));

            ChangeCursor(false);

            DirectionIndicator = false;

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[0] && Skill1Available) { skillUse[0] = false; }

        StartCoroutine(OpenDirectionIndicator());

        DirectionIndicator = true;

        ChangeCursor(true);
    }
    public override void Skill2(InputAction.CallbackContext context)
    {
        if (!loadingManager.canStartGame) return;

        if (playerMovement.IsStunned) return;

        if (!photonView.IsMine) return;

        if (!Skill2Available) { skillUse[1] = true; return; }

        if (context.phase == InputActionPhase.Canceled)
        {
            if (skillUse[1]) { skillUse[1] = false; return; }

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "ReleaseArrowRain");

            rpcManager.SendRPC(photonView, "OpenArrowRain", areaIndicatorPosition.x, areaIndicatorPosition.y, areaIndicatorPosition.z);

            SkillCoolDown.instance.SkillUsed(1, PlayerType.Arhcer);
            StartCoroutine(SkillCooldown(2));

            ChangeCursor(false);

            AreaIndicator = false;

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[1] && Skill2Available) { skillUse[1] = false; }

        StartCoroutine(OpenAreaIndicator());

        rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "AimArrowRain");

        ChangeCursor(true);
    }
    public override void Skill3(InputAction.CallbackContext context)
    {
        if (!loadingManager.canStartGame) return;

        if (playerMovement.IsStunned) return;

        if (!photonView.IsMine) return;

        if (!Skill3Available) { skillUse[2] = true; return; }

        if (context.phase == InputActionPhase.Canceled)
        { // YAP :  eger skill cooldownda ise mouse bastigimiz algilanip buraya girmemeli
            if (skillUse[2]) { skillUse[2] = false; return; }

            transform.rotation = Quaternion.Euler(transform.rotation.x, RotationY, transform.rotation.z);

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "ReleaseNormalArrow");

            ChangeCursor(false);

            DirectionIndicator = false;

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[2] && Skill3Available) { skillUse[2] = false; }

        rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "AimNormalArrow");

        StartCoroutine(OpenDirectionIndicator());

        ChangeCursor(true);
    }
    public override void Skill4(InputAction.CallbackContext context)
    {
        if (!loadingManager.canStartGame) return;

        if (playerMovement.IsStunned) return;

        if (!photonView.IsMine) return;

        if (!Skill4Available) { skillUse[3] = true; return; }

        if (context.phase == InputActionPhase.Canceled)
        {
            if (skillUse[3]) { skillUse[3] = false; return; }

            transform.rotation = Quaternion.Euler(transform.rotation.x, RotationY, transform.rotation.z);

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "ReleaseUltiArrow");

            ChangeCursor(false);

            DirectionIndicator = false;

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[3] && Skill4Available) { skillUse[3] = false; }

        rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Arhcer, "AimNormalArrow");

        StartCoroutine(OpenDirectionIndicator());

        ChangeCursor(true);
    }
    #endregion
    public override void IncreaseLevel()
    {
        for (int i = 0; i < 6; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (gameManager.doubleDamage) variables[0] *= 2;

        coolDowns[0] = variables[5];

        level++;
    }

    public override void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(4, this);
    }
    private void OpenBurstArrow()
    {
        BurstArrowShot(variables[1], variables[2], delay);
    }


    #region PunRPC
    [PunRPC]
    private void OpenBombArrow(float x, float y, float z, float angle)
    {
        GameObject arrow = PoolingSystemManager.instance.OpenObject("Arrow", new Vector3(x, y, z), Vector3.up * angle);

        arrow.GetComponent<Arrow>().arrowType = ArrowType.bombArrow;
    }

    [PunRPC]
    private void OpenArrowRain(float x, float y, float z)
    {
        playerWeapons.arrowRain.SetActive(true);
        playerWeapons.arrowRain.transform.position = new Vector3(x, y, z);
        PoolingSystemManager.instance.OpenObject("NovaFirePink", new Vector3(x, y, z), 2f);
    }

    [PunRPC]
    private void OpenSovaUlt(float x, float y, float z, float angle)
    {
        sovaUlt = skillGameObjects[3].GetComponent<SovaUlt>();

        sovaUlt.OpenAbility(new Vector3(x, y, z), angle);
    }
    #endregion

    /// <summary>
    /// Karakterin donerek ok atmasina yarar
    /// </summary>
    /// <param name="m_fovAngle">kac derecelik aci ile atacak.</param>
    /// <param name="m_arrowCount">kac adet ok atacak.</param>
    /// <param name="m_delay">iki ok atis arasinda bekleme suresi.</param>
    /// <returns></returns>
    private void BurstArrowShot(float m_fovAngle, float m_arrowCount, float m_delay)
    {
        float rotationAngle;
        if (m_arrowCount != 1)
            rotationAngle = m_fovAngle / (m_arrowCount - 1);
        else
            rotationAngle = m_fovAngle / (m_arrowCount);

        tempYAngle = transform.eulerAngles.y + m_fovAngle / 2;

        print("Temp Y Angle 0 : " + tempYAngle + " Transform : " + transform.eulerAngles);

        rpcManager.SendRPC(gameManager.photonView, "SaveArrowAngles", transform.eulerAngles.y, tempYAngle, rotationAngle, m_arrowCount, m_delay);
    }

    private IEnumerator start()
    {
        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        gameManager.IncreaseWeaponLevel(4);

        GameManager.instance.playerPrefabs[1] = this.gameObject;

        BuyMenu.instance.SavePlayer(GetComponent<PlayerHealth>());

        rpcManager.SendRPC(GameManager.instance.photonView, "ChangeCharacterAlive", (int)PlayerType.Arhcer, true);
    }
}
