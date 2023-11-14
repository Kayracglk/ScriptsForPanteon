using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Security.Cryptography;


public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerName;
    [SerializeField] private GameObject leftArrowButton;
    [SerializeField] private GameObject rightArrowButton;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    int value = 1;
    bool a = true;
    Player player;

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        if (a)
        {
            StartCoroutine(AddParentCoroutine());
            a = false;
        }
        else
        {
            UpdatePlayerItem(player);
        }

    }

    public void AddParents(GameObject[] parents)
    {
        for (int i = 0; i < parents.Length; i++)
        {
            print(parents[i].name + " " + parents.Length);
        }
        //StartCoroutine(AddParentCoroutine(parents));
    }
    private IEnumerator AddParentCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdatePlayerItem(player);

    }
    public void ApplyLocalChanges()
    {
        if (PhotonNetwork.LocalPlayer == player)
        {
            leftArrowButton.SetActive(true);
            rightArrowButton.SetActive(true);

            MenuManager.instance.localPlayerItem = this;
        }
    }

    public void OnClickLeftArrow()
    {
        value--;
        if (value <= 0)
        {
            playerProperties["playerAvatar"] = 0;
            value = 0;
        }
        else
        {
            playerProperties["playerAvatar"] = 2;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        value++;
        if (value >= 2)
        {
            playerProperties["playerAvatar"] = 1;
            value = 2;
        }
        else
        {
            playerProperties["playerAvatar"] = 2;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    private void UpdatePlayerItem(Player _player)
    {
        if (_player.CustomProperties.ContainsKey("playerAvatar"))
        {
            print((int)_player.CustomProperties["playerAvatar"]);
            if (LobbyManager.instance.parents[(int)_player.CustomProperties["playerAvatar"]] == null)
            {
                return;
            }
            gameObject.transform.SetParent(LobbyManager.instance.parents[(int)_player.CustomProperties["playerAvatar"]].transform, false);
            LobbyManager.instance.ChangeCharacter();
        }
        else
        {
            gameObject.transform.SetParent(LobbyManager.instance.parents[2].transform, false);
        }
    }
}
