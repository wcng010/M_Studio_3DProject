using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;//血条更新事件
    public CharacterData_SO templateData;//
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]
    public bool isCritical;
    #region Read from Data_SO

     void Awake()
    {if (templateData != null)
            characterData = Instantiate(templateData);
    
    }
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }

        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }

        set { characterData.currentHealth = value; }
    }
    public int BaseDefnce
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }

        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }

        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)//Player和Monster共同调用，插入动画帧中
    {
        int demage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - demage, 0);
        if (attacker.isCritical)
        { defener.GetComponent<Animator>().SetTrigger("Hit"); }
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
        if (CurrentHealth <= 0)
            attacker.characterData.UpdateExp(characterData.killPoint);

     }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        { coreDamage *= attackData.criticalMultiplier; }
        return (int)coreDamage;
    }
    #endregion

}


