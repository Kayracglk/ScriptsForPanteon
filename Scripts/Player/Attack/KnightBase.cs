using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Threading;

public class KnightBase : PlayerBase
{
    public static KnightBase instance;

    private Boomerang boomerang;
    private Earthquake earthquake;

    public GameObject earthQuakeShader;

    [Space(10)]
    [Header("LightingVariables")]
    public float[] variables = new float[5];

    public GameObject swordRotationParticle;
    public override void Awake()
    {
        instance = this;

        base.Awake();

        gameManager.PlayerSpawn(PlayerType.Knight, this.gameObject);
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

            transform.rotation = Quaternion.Euler(transform.rotation.x, RotationY, transform.rotation.z);

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Knight, "First");

            ChangeCursor(false);

            DirectionIndicator = false;

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[0] && Skill1Available) { skillUse[0] = false;}

        StartCoroutine(OpenDirectionIndicator());

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

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Knight, "Second");

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[1] && Skill2Available) { skillUse[1] = false; }

    }
    public override void Skill3(InputAction.CallbackContext context)
    {
        if (!loadingManager.canStartGame) return;

        if (playerMovement.IsStunned) return;

        if (!photonView.IsMine) return;

        if (!Skill3Available) { skillUse[2] = true; return; }

        if (context.phase == InputActionPhase.Canceled)
        {
            if (skillUse[2]) { skillUse[2] = false; return; }

            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[2] && Skill3Available) { skillUse[2] = false;}

        rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Knight, "Third");
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

            rpcManager.SendRPC(gameManager.view, "SetTriggerCharacterAnimation", (int)PlayerType.Knight, "Forth");

            ChangeCursor(false);

            DirectionIndicator = false;
            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        if (skillUse[3] && Skill4Available) { skillUse[3] = false;}

        StartCoroutine(OpenDirectionIndicator());

        ChangeCursor(true);
    }
    #endregion
    public override void IncreaseLevel()
    {
        for (int i = 0; i < 5; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (gameManager.doubleDamage) variables[0] *= 2;

        coolDowns[2] = variables[4];

        level++;
    }
    public override void SaveInsideOfManager()
    {
        gameManager.SaveWeaponInterface(2, this);
    }

    //public IEnumerator OpenDirectionIndicator()
    //{
    //    DirectionIndicator = true;

    //    weaponUI[0].gameObject.SetActive(true);

    //    weaponUI[0].rotation = Quaternion.Euler(Vector3.zero);

    //    while (true)
    //    {
    //        if (!DirectionIndicator) yield break;

    //        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    //        {
    //            mouseInput = new Vector3(hit.point.x, 0, hit.point.z);
    //        }

    //        Quaternion tempRotation = Quaternion.LookRotation(mouseInput - weaponUI[0].position);

    //        weaponUI[0].rotation = tempRotation;

    //        rotationY = weaponUI[0].eulerAngles.y;

    //        yield return null;
    //    }
    //}

    #region PunRPC
    [PunRPC]
    private void OpenBoomerang(float x, float y, float z, float angle)
    {
        boomerang = KnightBase.instance.skillGameObjects[0].GetComponent<Boomerang>();

        boomerang.OpenAbility(new Vector3(x, y, z), angle);
    }

    [PunRPC]
    private void OpenEarthquake(float x, float y, float z, float angle)
    {
        earthQuakeShader.SetActive(true);

        earthQuakeShader.transform.position = new Vector3(x, y, z);

        earthquake = KnightBase.instance.skillGameObjects[3].GetComponent<Earthquake>();

        earthquake.OpenAbility(new Vector3(x, y, z), angle);
    }

    [PunRPC]

    private void OpenSwordRotation()
    {
        swordRotationParticle.SetActive(true);

        playerWeapons.swordRotation.SetActive(true);
    }

    [PunRPC]
    private void OpenLighting(string m_ID, float m_damage, float m_slowRate, float m_slowTime)
    {
        PoolingSystemManager.instance.OpenObject("Lighting", EnemyManager.instance.GetEnemyWithID(m_ID).transform.position, 1);

        EnemyManager.instance.DecreaseEnemySpeed(m_ID, m_slowRate, m_slowTime);
        EnemyManager.instance.GiveDamageWithID(m_ID, m_damage);
    }
    #endregion

    private IEnumerator start()
    {
        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        gameManager.playerPrefabs[0] = this.gameObject;

        gameManager.IncreaseWeaponLevel(0);

        BuyMenu.instance.SavePlayer(GetComponent<PlayerHealth>());

        rpcManager.SendRPC(gameManager.view, "ChangeCharacterAlive", (int)PlayerType.Knight, true);
    }
}
