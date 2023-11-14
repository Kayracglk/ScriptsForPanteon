using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Skills", menuName = "Player Skill")]
public class LevelScriptableObject : ScriptableObject
{
    public SkillType skillType;

    public LevelStruct[] levels;

    [Serializable]
    public struct LevelStruct
    {
        public Variables[] variables;
        public string[] levelText;
    }

    [Serializable]
    public struct Variables
    {
        public string textName;
        public float value;
    }

}
public enum SkillType
{
    Skill1,
    Skill2,
    Skill3,
    Skill4,
}

