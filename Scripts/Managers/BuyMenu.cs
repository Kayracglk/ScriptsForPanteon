using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuyMenu : MonoBehaviour
{
    public static BuyMenu instance;

    private GameManager gameManager;
    private PlayerHealth playerHealth;
    private PlayerMoney playerMoney;

    [SerializeField] private int buyHealthMoney;
    [SerializeField] private int buyWeaponMoney;
    [SerializeField] private int buyReviveMoney;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gameManager = GameManager.instance;
        playerMoney = gameManager.GetPlayerWithType(gameManager.localPlayer).GetComponent<PlayerMoney>();
    }
    public void SavePlayer(PlayerHealth m_health)
    {
        playerHealth = m_health;
    }
    public void BuyHealth()
    {
        if (playerMoney.SpendMoney(buyHealthMoney))
            RPCManager.instance.SendRPC(gameManager.GetPlayerWithType(gameManager.localPlayer).GetComponent<PlayerHealth>().photonView, "IncreaseMaxHealth",200);
    }
    public void BuyWeapon()
    {
        if (playerMoney.SpendMoney(buyWeaponMoney))
            gameManager.doubleDamage = true;
    }
}
