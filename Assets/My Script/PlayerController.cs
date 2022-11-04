using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class PlayerController : MonoBehaviour
{   public GameObject Player;//自身的gameObject
    private NavMeshAgent agent;//寻路机器人
    Animator anim;//动画控制机
    private GameObject attackTarget;//攻击目标的gameObject
    private float lastAttackTime;//上次攻击事件
    private CharacterStats characterStats;//脚本
    bool isDead;//是否死亡判断
    private float StopDistance;//停止距离
    // Start is called before the first frame update
    private void Awake()
    {   
        characterStats = this.GetComponent<CharacterStats>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StopDistance = agent.stoppingDistance;
    }
    private void OnEnable()//物体被激活时执行一次
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//增添鼠标点击事件,鼠标点击事件在MouseManager的Update中持续执行
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    void Start()
    {
       SaveManager.Instance.LoadPlayerData();
       
    }
    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;//删去鼠标点击事件，MouseManager的Update不再执行
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }
    // Update is called once per frame
    void Update()
    {
        if (characterStats.CurrentHealth == 0)
        { isDead = true; }
        SwitchAnimation();//关联动画控制器
        lastAttackTime-=Time.deltaTime;
        if (isDead)
        { GameManager.Instance.NotifyObservers(); }//消息系统广播Player死亡消息
        
    }

    void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }
    public void MoveToTarget(Vector3 target)//Player移动到目标点
    {  
        
        StopAllCoroutines();//关掉所有协程
        if (isDead) return;
        agent.isStopped = false;//Player结束停止移动
        agent.destination = target;//Player向destination坐标移动
       agent.stoppingDistance = StopDistance;//Player到达目标点前的停止距离
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical =UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    IEnumerator MoveToAttackTarget()//朝敌人移动，到了一定攻击范围，停止移动，发起攻击
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.stoppingDistance = characterStats.attackData.attackRange;
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }
    void Hit()
    {
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats, targetStats);
    
    }
    
}
