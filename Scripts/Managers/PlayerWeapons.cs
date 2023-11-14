using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviourPunCallbacks
{
    public static PlayerWeapons instance;
    private LoadingManager loadingManager;

    #region Variables
    [Header("Knight Weapons")]
    public GameObject boomerang;
    public GameObject earthquake;
    public GameObject swordRotation;

    [Header("Archer Weapons")]
    public GameObject bombArrow;
    public GameObject arrowRain;
    public GameObject sovaUlt;

    #endregion
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        loadingManager = LoadingManager.instance;

        if (!PhotonNetwork.IsMasterClient) return;

        StartCoroutine(start());
    }

    private IEnumerator start()
    {
        while (!loadingManager.IsPlayerJoin[0] || !loadingManager.IsPlayerJoin[1])
        {
            yield return null;
        }

        RPCManager.instance.SendRPC(photonView, "SpawnWeapons");
    }

    [PunRPC]
    private void SpawnWeapons()
    {
        #region SpawnWeapons
        boomerang = Instantiate(Resources.Load("Weapons/Knight/Boomerang") as GameObject);
        earthquake = Instantiate(Resources.Load("Weapons/Knight/Earthquake") as GameObject);
        swordRotation = Instantiate(Resources.Load("Weapons/Knight/SwordRotation") as GameObject);

        bombArrow = Instantiate(Resources.Load("Weapons/Archer/bombArrow") as GameObject);
        arrowRain = Instantiate(Resources.Load("Weapons/Archer/arrowRain") as GameObject);
        sovaUlt = Instantiate(Resources.Load("Weapons/Archer/sovaUlt") as GameObject);
        #endregion

        #region SetParent
        boomerang.transform.parent = this.transform;
        earthquake.transform.parent = this.transform;
        swordRotation.transform.parent = this.transform;

        arrowRain.transform.parent = this.transform;
        bombArrow.transform.parent = this.transform;
        sovaUlt.transform.parent = this.transform;
        #endregion

        #region
        if (KnightBase.instance != null)
        {
            KnightBase.instance.SaveAbility(0, boomerang);
            KnightBase.instance.SaveAbility(1, swordRotation);
            KnightBase.instance.SaveAbility(3, earthquake);
        }

        if (ArcherBase.instance != null)
        {
            ArcherBase.instance.SaveAbility(1, bombArrow);
            ArcherBase.instance.SaveAbility(2, arrowRain);
            ArcherBase.instance.SaveAbility(3, sovaUlt);
        }
        #endregion

        #region CloseWeapons
        boomerang.SetActive(false);
        earthquake.SetActive(false);
        swordRotation.SetActive(false);

        bombArrow.SetActive(false);
        arrowRain.SetActive(false);
        sovaUlt.SetActive(false);
        #endregion

        loadingManager.ChangeWeaponCreated(1);
    }
}
