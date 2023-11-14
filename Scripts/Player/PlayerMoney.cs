using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    public int totalMoney = 0;
    
    private TMP_Text cashText;

    private void Start()
    {
        if(gameObject.GetComponent<KnightBase>())
            cashText = CashManager.instance.KnightCash;
        else
            cashText = CashManager.instance.ArcherCash;

        cashText.text = totalMoney.ToString();
    }

    public void AddMoney(int money)
    {
        print("Para eklendi");
        totalMoney += money;
        cashText.text = totalMoney.ToString();
        PlayerPrefs.SetInt("Money", totalMoney);
    }

    public bool SpendMoney(int money)
    {
        if (totalMoney < money) { Debug.LogError("Yetersiz Para");  return false; }

        totalMoney -= money;
        cashText.text = totalMoney.ToString();
        PlayerPrefs.SetInt("Money", totalMoney);
        return true;
    }

}
