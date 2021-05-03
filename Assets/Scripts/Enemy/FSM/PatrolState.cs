using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PatrolState : State
{
    private Vector3 patrolPos;    

    public PatrolState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.InitializeAll();          //변수 초기화        
        enemy.SetHasDestination();
        enemy.MoveToWayPoint();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();        
        enemy.FindTargets();
        //일정 거리에 들어오면 다시 순찰 포인트 설정
        if (enemy.DistanceXZ() <= enemy.minErrorWayPoint)
        {
            enemy.ChangeToPatrol();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

    
}
