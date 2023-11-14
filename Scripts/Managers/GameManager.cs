using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public GameObject[] playerPrefabs;
    public GameObject finishPanel;
    public Dictionary<PlayerType, GameObject> players = new Dictionary<PlayerType, GameObject>();

    public bool[] playersAlive = new bool[2];

    public IWeaponLevel[] weaponLevels = new IWeaponLevel[8];

    public PlayerType localPlayer;

    public PhotonView view;

    public float[] anglesOfArrow = new float[9];
    public int arrowCount = 0;

    public bool doubleDamage = false;

    [SerializeField]private TMP_Text fpsText;

    private void Awake()
    {
        instance = this;

        Application.targetFrameRate = -1;

        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        fpsText.SetText("Fps : " + 1f / Time.deltaTime);
    }
    public void PlayerSpawn(PlayerType m_type, GameObject m_player)
    {
        players.Add(m_type, m_player);
    }

    public GameObject GetPlayerWithType(PlayerType m_type)
    {
        return players[m_type];
    }

    public void SaveWeaponInterface(int m_index, IWeaponLevel m_weaponLevel)
    {
        weaponLevels[m_index] = m_weaponLevel;
    }

    public bool CanSpawnAgain()
    {
        return playersAlive[0] || playersAlive[1];
    }
    [PunRPC]
    public void IncreaseWeaponLevel(int targetLevel)
    {
        print("Test");
        weaponLevels[targetLevel].IncreaseLevel();
    }

    [PunRPC]
    public void ChangeCharacterAlive(int m_index, bool m_boolen)
    {
        playersAlive[m_index] = m_boolen;
    }

    [PunRPC]
    public void SetTriggerCharacterAnimation(int m_playerType, string key)
    {
        GetPlayerWithType((PlayerType)m_playerType).GetComponent<PlayerBase>().SetTrigger(key);
    }

    [PunRPC]
    public void SetBoolCharacterAnimation(int m_playerType, string key, int m_boolen)
    {
        if (m_boolen == 0)
            GetPlayerWithType((PlayerType)m_playerType).GetComponent<PlayerBase>().SetBool(key, false);
        else
            GetPlayerWithType((PlayerType)m_playerType).GetComponent<PlayerBase>().SetBool(key, true);
    }
    [PunRPC]
    public void ArcherFirstSkill()
    {
        if (localPlayer == PlayerType.Arhcer) return;

        GetPlayerWithType(PlayerType.Arhcer).GetComponent<PlayerBase>().SetTrigger("First");
    }

    [PunRPC]
    public void SaveArrowAngles(float defaulYValue, float firstYValue, float rotationAngle, float arrowCount, float delay)
    {
        for (int i = 0; i < arrowCount; i++)
        {
            anglesOfArrow[i] = firstYValue - (rotationAngle * i);
        }

        GetPlayerWithType(PlayerType.Arhcer).transform.eulerAngles = new Vector3(ArcherBase.instance.transform.eulerAngles.x, firstYValue, ArcherBase.instance.transform.eulerAngles.z); // YAP : archerin sadece uzeri donebilir

        StartCoroutine(ArrowBurstShot(defaulYValue, arrowCount, delay));
    }
    [PunRPC]
    public void OpenFinishPanel()
    {
        finishPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
    }

    private IEnumerator ArrowBurstShot(float defYValue, float m_arrowCount, float m_delay)
    {
        arrowCount = 0;

        WaitForSeconds delay = new WaitForSeconds(m_delay);

        for (int i = 0; i < m_arrowCount; i++)
        {
            ArcherBase.instance.SetTrigger("First");

            yield return new WaitForSeconds(.15f);

            GetPlayerWithType(PlayerType.Arhcer).transform.eulerAngles = new Vector3(ArcherBase.instance.transform.eulerAngles.x, anglesOfArrow[arrowCount], ArcherBase.instance.transform.eulerAngles.z); // YAP : archerin sadece uzeri donebilir

            GameObject arrow = PoolingSystemManager.instance.OpenObject("Arrow", ArcherBase.instance.skillSpawnPoints[0].position);

            arrow.transform.eulerAngles = new Vector3(0, anglesOfArrow[arrowCount], 0);

            arrow.GetComponent<Arrow>().arrowType = ArrowType.defaultArrow;

            arrowCount++;

            yield return delay;
        }

        yield return delay;

        GetPlayerWithType(PlayerType.Arhcer).transform.eulerAngles = new Vector3(ArcherBase.instance.transform.eulerAngles.x, defYValue, ArcherBase.instance.transform.eulerAngles.z); // YAP : archerin sadece uzeri donebilir
    }
}
