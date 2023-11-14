using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SovaUlt : MonoBehaviour, IWeaponLevel
{
    [Header("Variables")]
    public float[] variables = new float[6];

    [SerializeField] LevelScriptableObject levelValues;

    private GameObject child;

    private GameManager gameManager;

    private int level;

    private PhotonView view;
    private void Awake()
    {
        SaveInsideOfManager();
        child = transform.GetChild(0).gameObject;
        transform.parent = PlayerWeapons.instance.transform;
        gameManager = GameManager.instance;
        gameObject.SetActive(false);
        view = GetComponent<PhotonView>();

    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(1 * variables[2], 1, 1 * variables[1]);
        StopAllCoroutines();
    }

    private IEnumerator CanKillEnemy()
    {
        for (int i = 1; i < variables[3]; i++)
        {
            child.SetActive(true);
            ArcherBase.instance.sovaUltiParticle.SetActive(true);
            yield return new WaitForSeconds(variables[4]);
            child.SetActive(false);
            ArcherBase.instance.sovaUltiParticle.SetActive(false);
            yield return new WaitForSeconds(variables[4]);
        }

        child.SetActive(true);
        ArcherBase.instance.sovaUltiParticle.SetActive(true);
        yield return new WaitForSeconds(variables[4]);
        ArcherBase.instance.sovaUltiParticle.SetActive(false);
        child.SetActive(false);

        child.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void IncreaseLevel()
    {
        for (int i = 0; i < 6; i++)
        {
            variables[i] = levelValues.levels[level].variables[i].value;
        }

        if (GameManager.instance.doubleDamage) variables[0] *= 2;

        ArcherBase.instance.coolDowns[3] = variables[5];

        level++;
    }
    public void SaveInsideOfManager()
    {
        GameManager.instance.SaveWeaponInterface(7, this);
    }
    public void OpenAbility(Vector3 m_spawnPoint, float m_angle)
    {
        gameObject.SetActive(true);

        transform.parent = ArcherBase.instance.skillSpawnPoints[3];

        transform.position = m_spawnPoint;
        transform.eulerAngles = Vector3.up * m_angle;

        StartCoroutine(CanKillEnemy());
    }
}
