using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using Photon.Pun.Demo.PunBasics;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;

    public GameObject[] skillCanvas;
    public GameObject[] loadingCanvas;
    private PlayerType currentType;
    public GameObject[] UpgradePanel;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameObject playerToSpawn;

        playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

        if (playerToSpawn.name == "Archer")
            currentType = PlayerType.Arhcer;
        else if (playerToSpawn.name == "Knight")
            currentType = PlayerType.Knight;

        GameObject player = PhotonNetwork.Instantiate("Player/" + playerToSpawn.name, spawnPoints[(int)currentType].position, Quaternion.identity);
        EnemySpawnManager.instance.canSpawn = true;

        skillCanvas[(int)currentType].gameObject.SetActive(true);
        loadingCanvas[(int)currentType].gameObject.SetActive(true);

        GameManager.instance.localPlayer = currentType;

        RPCManager.instance.SendRPC(LoadingManager.instance.photonView, "ChangePlayerJoin", (int)currentType, 1);
    }
}
