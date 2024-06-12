using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{
    public enum EnemyState
    {
        Idle,
        FindingWeapon,
        Chasing,
        Fleeing
    }


    [Export] NavigationAgent3D _agent;
    protected EnemyState _enemyState = EnemyState.Idle;
    protected Node3D _target;

    public override void _Ready()
    {
        base._Ready();
        PickRandomTargetEntity();
        _enemyState = EnemyState.Chasing;
    }

    protected override void ProcessFree(double delta)
    {
        base.ProcessFree(delta);
        ProcessEnemyState(delta);
    }

    protected void ProcessEnemyState(double delta)
    {
        switch(_enemyState)
        {
            case EnemyState.Idle: ProcessEnemyIdle(delta); break;
            case EnemyState.FindingWeapon: ProcessEnemyFindingWeapon(delta); break;
            case EnemyState.Chasing: ProcessEnemyChasing(delta); break;
            case EnemyState.Fleeing: ProcessEnemyFleeing(delta); break;
        }
    }

    protected virtual void ProcessEnemyIdle(double delta) {}
    protected virtual void ProcessEnemyFindingWeapon(double delta) 
    {
        if(IsInstanceValid(_target))
            PathfindToward(_target.GlobalPosition - GlobalPosition);
    }
    protected virtual void ProcessEnemyChasing(double delta) 
    {
        if(IsInstanceValid(_target))
            PathfindToward(_target.GlobalPosition);
        else
            PickRandomTargetEntity();
    }
    protected virtual void ProcessEnemyFleeing(double delta) 
    {
        Vector3 dirToTarget = (_target.GlobalPosition - GlobalPosition).Normalized();
        PathfindToward(GlobalPosition - dirToTarget*50f);
    }
    protected virtual void PathfindToward(Vector3 position)
    {
        _agent.TargetPosition = position;
        Vector3 nextPosition = _agent.GetNextPathPosition();
        MoveDir(nextPosition - GlobalPosition);
    }

    private void PickRandomTargetEntity()
    {
        if(Entities.Count > 1)
            while(_target != this)
                _target = Entities[Random.Shared.Next()%Entities.Count];
        else _target = null;
    }

    private void PickRandomTargetWeapon()
    {
        _target = null;
        //_target = WeaponPickup.Pickups[Random.Shared.Next()%WeaponPickup.Pickups.Count];
    }
}
