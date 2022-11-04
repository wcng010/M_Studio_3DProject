using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject//继承ScriptableObject的类可以将变量可以通过创建一个Data储存数据
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;
    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }

    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LeveUP();
    }

    private void LeveUP()
    {
        currentLevel = Mathf.Clamp(++currentLevel, 0, maxLevel);//计算并限制++currentLevel的值
        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        Debug.Log("Level" + currentLevel + "Health" + currentHealth);
    }
}
