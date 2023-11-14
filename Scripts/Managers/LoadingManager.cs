using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public static LoadingManager instance;

    public bool canStartGame = false;

    public bool[] isPlayerReady = new bool[2];

    [SerializeField] private bool[] isPlayerJoin = new bool[2];
    [SerializeField] private bool isPoolingOver = false;
    [SerializeField] private bool isWeaponCreated = false;

    [Header("Loading Items")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private Image countDownBar;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private float countDownTimer;
    [SerializeField] private TMP_Text loadingText;

    private PhotonView view;

    private int howManyCheckComplete = 0;

    public bool[] IsPlayerJoin => isPlayerJoin;

    private void Awake()
    {
        instance = this;

        view = GetComponent<PhotonView>();
    }
    private void Start()
    {
        StartCoroutine(CheckGameStart());
    }


    private IEnumerator CheckGameStart()
    {
        float tempFillAmount = 0;

        bool tempStartGame = false;

        while (!tempStartGame)
        {
            float maxLimit = howManyCheckComplete * 20;

            while (tempFillAmount < maxLimit)
            {
                tempFillAmount += 50 * Time.deltaTime;

                loadingBar.value = tempFillAmount / 100;
                loadingText.text = "Loading %" +tempFillAmount.ToString();

                yield return null;
            }

            tempStartGame = CanGameStarted();
            yield return null;
        }

        RPCManager.instance.SendRPC(view, "Ready", (int)GameManager.instance.localPlayer);


        if (PhotonNetwork.IsMasterClient)
        {
            while (!isPlayerReady[0] || !isPlayerReady[1]) { yield return null; }

            RPCManager.instance.SendRPC(view, "CountDownFill");
        }
    }

    private IEnumerator Fill()
    {
        float tempFillAmount = 60;

        while (tempFillAmount < 100)
        {
            tempFillAmount += 60 * Time.deltaTime;

            loadingBar.value = tempFillAmount/100;
            loadingText.text = "Loading %" + tempFillAmount.ToString();

            yield return null;
        }

        tempFillAmount = 100;
        float tempDecreaseAmount = 100 / countDownTimer;
        float time = countDownTimer;
        int tempTime;

        loadingBar.gameObject.SetActive(false);

        countDownBar.gameObject.SetActive(true);

        while (tempFillAmount > 0)
        {
            tempFillAmount -= tempDecreaseAmount * Time.deltaTime;

            time -= Time.deltaTime;

            tempTime = (int)time + 1;

            countDownBar.fillAmount = tempFillAmount / 100;

            countDownText.SetText(tempTime.ToString());

            yield return null;
        }

        Cursor.lockState = CursorLockMode.Locked;

        loadingPanel.SetActive(false);

        canStartGame = true;
    }
    public bool CanGameStarted()
    {
        return isPlayerJoin[0] && isPlayerJoin[1] && isPoolingOver && isWeaponCreated;
    }

    [PunRPC]
    public void ChangePlayerJoin(int m_playerType, int m_value)
    {
        if (m_value == 0)
            isPlayerJoin[m_playerType] = false;
        else
            isPlayerJoin[m_playerType] = true;

        howManyCheckComplete++;
    }

    [PunRPC]
    public void ChangePoolingOver(int m_value)
    {
        if (m_value == 0)
            isPoolingOver = false;
        else
            isPoolingOver = true;

        howManyCheckComplete++;
    }

    [PunRPC]
    public void ChangeWeaponCreated(int m_value)
    {
        if (m_value == 0)
            isWeaponCreated = false;
        else
            isWeaponCreated = true;

        howManyCheckComplete++;
    }

    [PunRPC]
    public void CountDownFill()
    {
        StartCoroutine(Fill());
    }
    [PunRPC]
    public void Ready(int m_playerType)
    {
        isPlayerReady[m_playerType] = true;
    }
}
