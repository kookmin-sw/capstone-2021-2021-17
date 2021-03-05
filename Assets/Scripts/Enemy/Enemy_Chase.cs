using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : MonoBehaviour
{
    //���� �þ�
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //���� ���µ�
    enum State {
        Idle,
        FindTarget,
        Move,
        Attack
    }

    private State state;

    //�þ߿� ���� ������ List
    public List<Transform> visibleTargets = new List<Transform>();
    
    // Initialize Players Position
    GameObject[] player;
    //has Path
    public bool hasP = false;
    public bool SetTarget = false;
    //�÷��̾ ��Ҵ��� üũ
    public bool Check_Catched = false;
    //�þ߿� ���� ���Դ��� üũ
    public bool FindTarget_Vision = false;

    //AI
    NavMeshAgent Enemy;        
    //For Sort by distance
    Dictionary<string, float> distance_target;
    // target
    GameObject target;

    void Start()
    {        
        //�⺻ ����
        state = State.Idle;
        //���� �ý����� �̿��ϱ� ���� �ʱ�ȭ
        Enemy = GetComponent<NavMeshAgent>();
        
        //�ڷ�ƾ�� �����ؼ� FSM�� ����.
        StartCoroutine("Run");
    }

    IEnumerator Run()
    {
        //�׽� �þ߰� �����ȴ�.
        StartCoroutine("FindTargetsWithDelay",0f);

        //ù �ڷ�ƾ�� �����ϸ� ���������� while���� ����.
        while (true)
        {
            switch (state)
            {
                case State.Idle:
                    yield return StartCoroutine("Idle");
                    break;
                case State.FindTarget:
                    yield return StartCoroutine("FindTargetState");
                    break;
                case State.Move:
                    yield return StartCoroutine("MoveState");
                    break;
                case State.Attack:
                    yield return StartCoroutine("AttackState");
                    break;

            }
        }
    }
    //Idle State
    IEnumerator Idle()
    {
        yield return new WaitForSeconds(3f);
        state = State.FindTarget;
    }
  
    IEnumerator FindTargetState()
    {
        while (state == State.FindTarget)
        {
            //�ӽ� �±׸� �̿��� �÷��̾���� ã�´�.
            player = GameObject.FindGameObjectsWithTag("Respawn");
            //��Ҵ��� Ȯ���ϴ� ���� �ʱ�ȭ
            Check_Catched = false;
            //��ųʸ��� �̿��� �÷��̾��� �̸��� �Ÿ��� ����
            distance_target = new Dictionary<string, float>();

            //�Ѹ��� ���� �߰����� ���Ѵٸ� Attack���·� ����
            // ** ���� Attack ���´� �ƹ��͵� �ƴ� ���� ** 
            if(player.Length == 0)
            {
                state = State.Attack;
                break;
            }

            for (int i = 0; i < player.Length; i++)
            {   
                // ���� ������ ��ųʸ��� �÷��̾��� �̸��� �����Ÿ��� ����.
                distance_target.Add(player[i].name, Vector3.Distance(transform.position, player[i].transform.position));
            }
            // �Ÿ� ������ ����
            var ordered = distance_target.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            // �� �� ���� ����� �Ÿ��� �ִ� ���� Ÿ������ ����
            target = GameObject.Find(ordered.First().Key);
            //Ÿ�� ���� ���� ����
            SetTarget = true;
            //Path�� ���� �ִٰ� �˸�
            hasP = true;
            // ���¸� Move ���·� ����
            state = State.Move;
            yield return null;
        }        
    }

    IEnumerator MoveState()
    {                      
        while(state == State.Move)
        {
            //���� ������ �ٽ� Idle ���·� ����.
            if(Check_Catched)
            {
                //��ΰ� ���ٰ� �˸�
                hasP = false;
                state = State.Idle;            
            }
            else
            {
                //����ؼ� ��θ� �����ؼ� �÷��̾ �������� �� ��θ� �ٽ� �����Ѵ�.
                Enemy.SetDestination(target.transform.position);
                //Ÿ���� ���������Ƿ� Ÿ�� ���� ���� �ʱ�ȭ
                SetTarget = false;
                yield return null;
            }            
        }
        
    }
    
    IEnumerator AttackState()
    {
        yield return null;
    }

    //�þ߿� ���� Ÿ���� ã�´�.
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            //�þ߿� ���� ���� ������
            if (FindTarget_Vision)
            {
                //�� ���� Ÿ������ ��´�.
                SetTarget_Vision();
            }
        }
    }

    //�þ߿� ���ο� ���� ������ ���� ���� �� ���� ����� Ÿ������ Ÿ�� ����
    void SetTarget_Vision()
    {
        //�þ߿� ���� �������Ƿ� ���� Ž���ϴ� ���� �ʱ�ȭ
        FindTarget_Vision = false;

        //���� FindTargetState�� ����
        distance_target = new Dictionary<string, float>();

        for (int i = 0; i < visibleTargets.Count; i++)
        {
            distance_target.Add(visibleTargets[i].gameObject.name, Vector3.Distance(transform.position, visibleTargets[i].position));
        }                        
        var ordered = distance_target.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        target = GameObject.Find(ordered.First().Key);
        SetTarget = true;
        hasP = true;
        state = State.Move;
    }
    

    //�þ߿� ���� �ִ��� ������ ã�´�.
    void FindVisibleTargets()
    {        
        //�þ߿� ���� Ÿ�ٵ��� �ʱ�ȭ
        visibleTargets.Clear();

        //�ֺ� �þ� ������ ���� Ÿ�ٵ��� ã�´�.
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //Ÿ�ٵ��� ũ�⸸ŭ for���� ���鼭 Ÿ���� �����ϰ� �þ߿� ���� ������ List�� �ִ´�.
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    //���� Ž���ϴ� ���� ����
                    FindTarget_Vision = true;
                }
            }
        }
    }

    //Scene���� �þ߰��� ���� ��ġ�� �մ� ���� �ߴ´�.
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
