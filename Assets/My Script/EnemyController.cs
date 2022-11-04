using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}//��ö���������ֹ����״̬
[RequireComponent(typeof(NavMeshAgent))]//������ض���û��NavMeshAgent���������һ��
[RequireComponent(typeof(CharacterStats))]//ͬ��
public class EnemyController : MonoBehaviour,IEndGameObserver//�̳�IEndGameObserver�ӿ�
{
    
    private Collider coll;
    private EnemyStates enemyStates;//ö�����ͱ�����¼״̬
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    [Header("Basic Setting")]

    public float sightRadius;//�����������
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;//����Ŀ����վ׮ʱ��
    private float lastAttackTime;
    private Quaternion guardRotation;//��ά����Ź����ʼ�Ƕ�
    [Header("Patrol State")]

    public float patrolRange;//����Ѳ������
    private Vector3 wayPoint;
    private Vector3 guardPos;
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool  isDead  ;
    bool playerDead;
    void Awake()
    {  characterStats=this.GetComponent<CharacterStats>();
        agent = GetComponent<NavMeshAgent>();//�õ��Զ�Ѱ·������Ϣ����player��
        anim = GetComponent<Animator>();//��ý�ɫ������Ϣ
        speed = agent.speed;
        guardPos=transform.position;//guardPos��¼��ɫ��ʼλ��
        remainLookAtTime = lookAtTime;
        guardRotation= transform.rotation;//guardRotation��¼��ɫ��ʼ�Ƕ�
        coll = GetComponent<Collider>(); 
    }
    void Start()
    {if (isGuard)//判断是否勾选为警卫模式
        {
            enemyStates = EnemyStates.GUARD;
        }
        else 
        { enemyStates = EnemyStates.PATROL;//否则改模式为巡逻模式
            GetNewWayPoint();
        }
        GameManager.Instance.AddObserver(this);
        lastAttackTime = characterStats.attackData.coolDown;
    }
    void Update()
    {  if (characterStats.CurrentHealth == 0 )
         isDead = true;
        if (!playerDead)
        {
            SwitchState();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
        if(lastAttackTime<-characterStats.attackData.coolDown)
            lastAttackTime = characterStats.attackData.coolDown;
        
    }
    void SwitchState()
    { if (isDead)
        {
            
            enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())//首先判断视线范围是否有Player
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        { case EnemyStates.GUARD://警卫模式
                isChase = false;
                
                if (transform.position != guardPos)//如果当前位置不在起始位置，巡逻切换回警卫时操作
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        isWalk = false;
                        transform.rotation=Quaternion.Lerp(transform.rotation,guardRotation,0.01f);
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)//如果到达巡逻点，则停止一段时间并前往下一个巡逻点               
               {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    { remainLookAtTime -= Time.deltaTime; }
                    else
                        GetNewWayPoint();
                }
                else//否则前往以设定好的巡逻点
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE://追击模式
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())//如果没找到Player,则停止一段时间后按情况进入巡逻或警卫模式。
                {
                    isFollow = false;
                    if (remainLookAtTime > 0) 
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    { enemyStates = EnemyStates.GUARD; }
                    else enemyStates = EnemyStates.PATROL;
                }
                else//若找到Player，去追击Player
                {   isFollow = true;
                    agent.isStopped=false;
                    agent.destination = attackTarget.transform.position; 
                }

                if (TargetInAttackRange() || TargetInSkillRange())//如果目标在攻击范围或技能范围
                {
                    isFollow = false;
                    agent.isStopped = true;
                    characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                    Attack();
                }
                
                
                break;
            case EnemyStates.DEAD:
               coll.enabled = false;
                agent.radius= 0;
                Destroy(gameObject,2f);//需要留时间播放死亡动画
                break;
        }
     }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        { anim.SetTrigger("Attack");}
        else
        { anim.SetTrigger("Skill"); }
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//射线检测以transform.position为球心，sightRadius为半径的球体范围内的碰撞体，返回碰撞体数组。            
        foreach (var target in colliders) //遍历碰撞体数组，若其中有碰撞体的tag是Player，得到Player的gameObject，返回true。
        {         
            if (target.tag=="Player")
            {
                attackTarget = target.gameObject;
                
                return true; }
        }
        attackTarget = null;
        return false;
          
    }

    bool TargetInAttackRange()
    { if (attackTarget != null)
        { return (Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange)&&lastAttackTime>0; }
        else return false; 
    }

    bool TargetInSkillRange()
    {    if (attackTarget != null && lastAttackTime <=0)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }

        else return false;  
    }

    void SwitchAnimation()//����ת����
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }


    void GetNewWayPoint()//ʹ������һ����Χ������ƶ�
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);//��������ĵ�λ
        float randomZ = Random.Range(-patrolRange, patrolRange); 
        
        Vector3 randomPoint = new Vector3(guardPos.x+randomX,transform.position.y ,guardPos.z+randomZ);//
        NavMeshHit hit;
      wayPoint=  NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position:transform.position;//�ж�randomPoint�ĵ��Ƿ�ΪWalkable�������򷵻�randomPoint��λ�ã����򷵻ص�ǰλ��
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//画出巡逻范围
    }
    void Hit()
    { if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    
    }

    public void EndNotify()
    {
        isChase = false;    
        isWalk = false; 
        attackTarget = null;
        anim.SetBool("Win",true);
        playerDead = true;
    }
    void OnDisable()
    {   if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this); }
}
    
    




