using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypesEnum
{
    Spider,
    Wolf,
    Cobra,
    Treant,
    Golem,
    Orc,
}

[CreateAssetMenu(fileName = "New Enemy Type", menuName = "EnemyType")]
public class EnemyType : ScriptableObject
{
    public float walkSpeed = 5f;
    public int health = 1;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public Material[] enemyMaterial;
    public EnemyTypesEnum enemyType;
}
