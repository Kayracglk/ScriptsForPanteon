using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    [SerializeField] private GameObject[] skillsBg;
    [SerializeField] private GameObject[] skillUpgradesButtons;
    private int count = 0;
    private bool[] skill = new bool[4];

    private PhotonView photonView;
    private void Awake()
    {
        instance = this;
        skill[0] = true;

        photonView = GetComponent<PhotonView>();
    }

    public void SkillUpgradeButton(int test)
    {
        int m_skill = test % 10; // 0 1 2 3 yetenekleri 

        int playerType = test / 10; //skill 0 ise knight 1 ise archer

        int targetLevel = (playerType * 4) + m_skill;

        if(skill[m_skill])
        {
            RPCManager.instance.SendRPC(GameManager.instance.view, "IncreaseWeaponLevel", targetLevel);
        }
        else
        {
            RPCManager.instance.SendRPC(GameManager.instance.view, "IncreaseWeaponLevel", targetLevel);
            if (m_skill == 1)
                GameManager.instance.GetPlayerWithType((PlayerType)playerType).GetComponent<PlayerBase>().skill2Available = true;
            else if(m_skill == 2)
                GameManager.instance.GetPlayerWithType((PlayerType)playerType).GetComponent<PlayerBase>().skill3Available = true;
            else if(m_skill == 3)
                GameManager.instance.GetPlayerWithType((PlayerType)playerType).GetComponent<PlayerBase>().skill4Available = true;
            else 
                return;
            skill[m_skill] = true;
            skillsBg[m_skill + 4 * playerType].SetActive(false);
        }
        count--;
        print("Count : " + count);
        if (count <= 0)
        {
            for (int i = 0; i < skillUpgradesButtons.Length; i++)
            {
                skillUpgradesButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpgradeSkillShow(int player, int currentLevel, int ultiUpgrade, int count)
    {
        if (GameManager.instance.localPlayer != (PlayerType)player) return;

        this.count += count;
        for (int i = 4 * player; i < 4 * (player + 1); i++)
        {
            skillUpgradesButtons[i].gameObject.SetActive(true);
        }
        if(currentLevel < ultiUpgrade)
        {
            skillUpgradesButtons[3].gameObject.SetActive(false);
            skillUpgradesButtons[7].gameObject.SetActive(false);
        }

    }
}
