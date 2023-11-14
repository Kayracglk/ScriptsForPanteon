using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    public PhotonView photonView;
    public bool isPlayerAlive;

    public Transform reviveNeedPanel;
    public Transform revivePanel;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthSlider;

    private PlayerType playerType;
    private bool isReviveFriend;
    private bool collidedWithFriend;
    private Image fillBar;
    #endregion

    #region Properities
    public int CurrentHealth => currentHealth;

    public bool CollidedWithFriend { get { return collidedWithFriend; } set { collidedWithFriend = value; } }
    #endregion

    private void Awake()
    {
        currentHealth = maxHealth;

        isPlayerAlive = true;

        photonView = GetComponent<PhotonView>();

        playerType = GetComponent<PlayerCurrentType>().currentType;

        fillBar = revivePanel.GetChild(0).GetComponent<Image>();
    }
    private void Start()
    {
        TextHealthValue();
    }

    private void TextHealthValue()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && photonView.IsMine)
            TakeDamage(222);
    }
    public void TakeDamage(int m_damageValue)
    {
        currentHealth -= m_damageValue;

        TextHealthValue();

        if (currentHealth <= 0)
        {
            RPCManager.instance.SendRPC(photonView, "Die", (int)playerType);
        }
    }

    public void GiveHealth(int m_healthValue)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += m_healthValue;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            TextHealthValue();
        }
    }

    public void ReviveFriend(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (!collidedWithFriend) return;

        if (context.phase == InputActionPhase.Canceled)
        {
            isReviveFriend = false;
            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        StartCoroutine(Revive());
    }

    private IEnumerator Revive()
    {
        isReviveFriend = true;

        float timer = 0;

        fillBar.fillAmount = 0;

        while (isReviveFriend)
        {
            if (!isReviveFriend) yield break;

            if (!collidedWithFriend) yield break;

            if (timer > 3)
            {
                if (playerType == PlayerType.Knight)
                    RPCManager.instance.SendRPC(photonView, "Revive", (int)PlayerType.Arhcer);
                else
                    RPCManager.instance.SendRPC(photonView, "Revive", (int)PlayerType.Knight);
                yield break;
            }

            fillBar.fillAmount = timer / 3;

            timer += Time.deltaTime;

            yield return null;
        }
    }

    #region PunRPC
    [PunRPC]
    private void Die(int m_playerType)
    {
        if (!GameManager.instance.playersAlive[(int)m_playerType]) { print("Zaten Oluyum"); return; }

        GameManager.instance.ChangeCharacterAlive((int)m_playerType, false);

        if (GameManager.instance.CanSpawnAgain())
        {
            GameObject player = GameManager.instance.GetPlayerWithType((PlayerType)m_playerType);

            player.GetComponent<PlayerMovement>().IsStunned = true;

            player.GetComponent<PlayerHealth>().isPlayerAlive = false;
            player.GetComponent<PlayerHealth>().reviveNeedPanel.gameObject.SetActive(true);

            RPCManager.instance.SendRPC(GameManager.instance.view, "SetTriggerCharacterAnimation", (int)GetComponent<PlayerCurrentType>().currentType, "Die");

            print("Takim arkadasi kaldirilmasi bekleniyor");
        }
        else
        {
            GameManager.instance.GetPlayerWithType(PlayerType.Arhcer).SetActive(false);
            GameManager.instance.GetPlayerWithType(PlayerType.Knight).SetActive(false);

            if (PhotonNetwork.IsMasterClient)
                RPCManager.instance.SendRPC(GameManager.instance.photonView, "OpenFinishPanel");
        }
    }

    [PunRPC]
    private void Revive(int m_playerType)
    {
        GameManager.instance.ChangeCharacterAlive((int)m_playerType, true);

        GameObject player = GameManager.instance.GetPlayerWithType((PlayerType)m_playerType);

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        player.GetComponent<PlayerMovement>().IsStunned = false;

        playerHealth.isPlayerAlive = true;

        playerHealth.reviveNeedPanel.gameObject.SetActive(false);

        playerHealth.currentHealth = playerHealth.maxHealth;

        playerHealth.TextHealthValue();

        if (GameManager.instance.localPlayer != (PlayerType)m_playerType) return;

        RPCManager.instance.SendRPC(GameManager.instance.view, "SetTriggerCharacterAnimation", (int)player.GetComponent<PlayerCurrentType>().currentType, "Revive");

    }
    [PunRPC]
    public void IncreaseMaxHealth(int maxValue)
    {
        maxHealth = maxValue;

        currentHealth = maxHealth;

        TextHealthValue();
    }
    #endregion
}
