using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public static PlayerLevel instance;
    PlayerType playerType;
    public int currentLevel = 0;
    public int currentXp = 0;
    public int levelUpXp = 6;
    private int currentUpgradeCount = 0;
    private int ultiUpgrade = 5;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image fillBar;
    private PhotonView photonView;

    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        playerType = this.GetComponent<PlayerCurrentType>().currentType;
        levelText.text = currentLevel.ToString();
    }

    public void AddXp(int xp)
    {
        if (currentXp + xp < levelUpXp)
        {
            currentXp += xp;

            fillBar.fillAmount = (float)currentXp / levelUpXp;
        }
        else if(currentXp + xp >= levelUpXp)
        {
            currentXp += xp;
            fillBar.fillAmount = (float)currentXp / levelUpXp;

            while (currentXp - levelUpXp >= 0)
            {
                currentXp -= levelUpXp;
                currentUpgradeCount++;
                levelUpXp *= 2;
                currentLevel += 1;
                EnemySpawnManager.instance.playerPower = currentLevel * 2;
                levelText.text = currentLevel.ToString(); // YAP : buraya level ui ekleenek
            }
            SkillManager.instance.UpgradeSkillShow((int)playerType, currentLevel, ultiUpgrade, currentUpgradeCount);
            currentUpgradeCount = 0;

            fillBar.fillAmount = (float)currentXp / levelUpXp;
        }
    }
    public void AddLevel()
    {
        currentLevel += 1;
        levelText.text = currentLevel.ToString();
        SkillManager.instance.UpgradeSkillShow((int)playerType, currentLevel, ultiUpgrade, 1);
    }
}
