using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;
    public PhotonView view;

    [Header("Panels")]
    public GameObject lobbyPanel;
    public GameObject roomPanel;

    [Header("Text")]
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomID;
    public TMP_InputField lobbyRoomID;

    [Header("Buttons")]
    public GameObject playButton;

    [Header("Lists&Series")]
    public List<PlayerItem> playerItems = new List<PlayerItem>();

    public GameObject[] parents;
    private List<int> roomIDList = new List<int>();
    public PlayerItem playerItemPrefab;

    private void Awake()
    {
        instance = this;
        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    #region PunCallbacks

    public override void OnCreatedRoom()
    {
        Debug.Log($"You have created a Photon Room named {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.MasterClient.NickName + "'s Lobby";
        roomID.text = PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
    }
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }


    #endregion

    #region Public Methods
    public void OnClickCreate()
    {
        PhotonNetwork.CreateRoom(CreateUniuqueRoomID(), new RoomOptions { MaxPlayers = 2, BroadcastPropsChangeToAll = true });
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(lobbyRoomID.text);
    }

    public void OnClickRoomLeave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ChangeCharacter()
    {
        if (PhotonNetwork.IsMasterClient && parents[0].transform.childCount == 1 && parents[1].transform.childCount == 1)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }
    #endregion

    #region PunRPC

    /*[PunRPC]
    public void CreatePlayerItem(string userID)
    {
        GameObject newPlayerItem = PhotonNetwork.Instantiate("Network/PlayerItem", Vector3.zero, Quaternion.identity);

        newPlayerItem.transform.SetParent(playerItemParent.transform, false);

        playerItems.Add(userID,newPlayerItem);
    }

    [PunRPC]
    public void DeletePlayerItem(string otherPlayer)
    {
        foreach (var item in playerItems)
        {
            if (item.Key == otherPlayer)
            {
                GameObject temp = playerItems[otherPlayer];
                playerItems.Remove(otherPlayer);
                PhotonNetwork.Destroy(temp);
            }
        }
    }
    [PunRPC]
    public void SetParent(int _player, int ID)
    {
        if(ID == 0)
        {
            masterClient.transform.SetParent(parents[_player].transform, false);
        }
        else
        {
            client.transform.SetParent(parents[_player].transform, false); 
        }
    }*/
    #endregion

    #region Private Methods
    private string CreateUniuqueRoomID()
    {
        int temp = UnityEngine.Random.Range(10000, 100000);
        while (roomIDList.Contains(temp))
        {
            temp = UnityEngine.Random.Range(10000, 100000);
        }
        roomIDList.Add(temp);
        return temp.ToString();
    }

    private void UpdatePlayerList()
    {
        foreach (var item in playerItems)
        {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        if (PhotonNetwork.CurrentRoom == null) { return; }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, parents[2].transform);
            newPlayerItem.SetPlayerInfo(player.Value);

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.AddParents(parents);
                newPlayerItem.ApplyLocalChanges();
            }
            playerItems.Add(newPlayerItem);
        }
    }
    #endregion
}
