using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameInput;
    public TextMeshProUGUI buttonText;

    const string playerNamePrefKey = "PlayerName";
    void Start()
    {
        string defaultName = string.Empty;
        if (userNameInput != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                userNameInput.text = defaultName;
            }
        }
        PhotonNetwork.NickName = defaultName;
    }

    public void OnClickConnect()
    {
        if (userNameInput.text.Length > 0)
        {
            PhotonNetwork.NickName = userNameInput.text;
            PlayerPrefs.SetString(playerNamePrefKey, userNameInput.text);
        }
        else
        {
            string tempName = playerNamePrefKey + Random.Range(1000, 10000);
            PhotonNetwork.NickName = tempName;
            PlayerPrefs.SetString(playerNamePrefKey, tempName);
        }

        buttonText.text = "Connecting..";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
