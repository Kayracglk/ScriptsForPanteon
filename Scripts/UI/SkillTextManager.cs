using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTextManager : MonoBehaviour
{
    public LevelScriptableObject scriptableObj;
    [SerializeField] private int skillLevel = 1;
    [SerializeField] private TMP_Text[] skillType;
    [SerializeField] private TMP_Text[] skillCount;

    public void GetText()
    {
        skillLevel += 1;
        for(int i = 0; i < 3; i++)
        {
            skillType[i].transform.parent.gameObject.SetActive(true);
            string[] a = scriptableObj.levels[skillLevel].levelText[i].Split('&');
            if(a.Length > 1)
            {
                skillType[i].text = a[0];
                skillCount[i].text = a[1];
            }
            else
                skillType[i].transform.parent.gameObject.SetActive(false);
        }
        
    }
}
