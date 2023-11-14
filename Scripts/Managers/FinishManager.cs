using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishManager : MonoBehaviour
{
    public bool[] isPlayersReady = new bool[2];

    public GameObject button;

    private PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    public void PlayerReady()
    {
        RPCManager.instance.SendRPC(view, "ChangePlayerReadyRPC", GameManager.instance.localPlayer, 1);
    }

    public void PlayerUnready()
    {
        RPCManager.instance.SendRPC(view, "ChangePlayerReadyRPC", GameManager.instance.localPlayer, 0);
    }

    public void ChangePlayerReady()
    {
        if (isPlayersReady[(int)GameManager.instance.localPlayer])
        {
            PlayerUnready();
        }
        else PlayerReady();


    }

    public void OpenPlayButton()
    {
        if (isPlayersReady[0] && isPlayersReady[1])
        {
            button.SetActive(true);
        }
        else
        {
            button.SetActive(false);
        }
    }

    public void FinishGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("ConnectToServer");

    }

    public void Exit()
    {
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }

    [PunRPC]
    public void ChangePlayerReadyRPC(int playerType, int m_boolen)
    {
        if (m_boolen == 0)
            isPlayersReady[playerType] = false;
        else
            isPlayersReady[playerType] = true;

        if (PhotonNetwork.IsMasterClient)
        {
            OpenPlayButton();
        }

    }
}
