using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CashManager : MonoBehaviourPunCallbacks
{
    public static CashManager instance;
    public TMP_Text ArcherCash;
    public TMP_Text KnightCash;

    private GameManager gameManager;
    private PhotonView view;

    private void Awake()
    {
        instance = this;
        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
    }

}
