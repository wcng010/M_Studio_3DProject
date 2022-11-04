using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = " New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//攻击距离
    public float skillRange;//远程攻击距离
    public float coolDown;//cd冷却时间
    public int minDamage;//最小攻击数值
    public int maxDamage;//最大攻击数值
    public float criticalMultiplier;//爆伤
    public float criticalChance;//暴击

  
}
