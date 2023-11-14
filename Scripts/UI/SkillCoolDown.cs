using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCoolDown : MonoBehaviour
{
    public static SkillCoolDown instance;
    [SerializeField] private TMP_Text[] cooldownText;
    [SerializeField] private Image[] cooldownImage;

    private void Awake()
    {
        instance = this;
    }

    public void SkillUsed(int skill,PlayerType playerType)
    {
        int type = (int)playerType;
        float totalTime = GameManager.instance.GetPlayerWithType(playerType).GetComponent<PlayerBase>().coolDowns[skill];
        float currentTime = totalTime;

        cooldownImage[skill + 4 * type].gameObject.SetActive(true);
        StartCoroutine(Colldown(skill, type, currentTime, totalTime));
    }

    IEnumerator Colldown(int skill,int type ,float currentTime, float totalTime)
    {
        while(currentTime >= 0)
        {
            cooldownText[skill + 4 * type].text = ((int)currentTime + 1).ToString();
            cooldownImage[skill + 4 * type].fillAmount = currentTime / totalTime;
            currentTime -= Time.deltaTime;
            yield return null;
        }
        cooldownImage[skill + 4 * type].gameObject.SetActive(false);
    }
}

