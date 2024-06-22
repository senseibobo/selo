using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : Entity
{
    protected static List<string> _maleNamePool = new();
    protected static List<string> _femaleNamePool = new();

    protected static List<string> _maleFirstNames = new List<string> { "Jorgovan", "Milorad", "Draško", "Srđan", "Stojan", "Ljubivoje", "Rastko", "Srbobran", "Srećko", "Miodrag" };
    protected static List<string> _femaleFirstNames = new List<string> { "Jorgovanka", "Jelisaveta", "Zorica", "Stanimirka", "Bogosava", "Živka", "Jelka", "Mirka", "Smiljka" };

    protected static List<string> _lastNames = new List<string> { "Janković", "Ristić", "Marković", "Stojković", "Branković", "Marinković", "Isaković", "Jovanović" };

    public enum EnemyState
    {
        Idle,
        FindingWeapon,
        Chasing,
        Fleeing
    }

    public enum Gender
    {
        Male,
        Female
    }


    [Export] NavigationAgent3D _agent;
    [Export] Label3D _nameLabel;
    protected EnemyState _enemyState = EnemyState.FindingWeapon;
    protected Node3D _target;

    private float _entityTargetTimer;
    private float _attackTimer;
    public virtual float EntityTargetTime => 5f;
    public virtual float AttackTime => 0.4f;

    static Enemy()
    {
        RegenerateNames();
    }

    public static void RegenerateNames()
    {
        _maleNamePool.Clear();
        _femaleNamePool.Clear();

        foreach (string lastName in _lastNames)
        {
            foreach (string maleFirstName in _maleFirstNames)
            {
                string name = $"{maleFirstName} {lastName}";
                _maleNamePool.Add(name);
                GD.Print(name);
            }
            foreach (string femaleFirstName in _femaleFirstNames)
            {
                _femaleNamePool.Add($"{femaleFirstName} {lastName}");
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();
        PickRandomTargetEntity();
        _enemyState = EnemyState.FindingWeapon;
        Gender gender = Random.Shared.NextInt64() % 2 == 0 ? Gender.Male : Gender.Female;
        PickRandomName(gender);
        LeaderboardEntry.SetEntryInfo(Name, 0f);
        _nameLabel.Text = Name;
    }

    private void PickRandomName(Gender gender)
    {
        if (gender == Gender.Male)
        {
            Name = _maleNamePool[(int)(Random.Shared.NextInt64() % _maleNamePool.Count)];
            _maleNamePool.Remove(Name);
        }
        else
        {
            Name = _femaleNamePool[(int)(Random.Shared.NextInt64() % _femaleNamePool.Count)];
            _femaleNamePool.Remove(Name);
        }
    }

    protected override void ProcessFree(double delta)
    {
        base.ProcessFree(delta);
        ProcessEnemyState(delta);
    }

    protected void ProcessEnemyState(double delta)
    {
        switch (_enemyState)
        {
            case EnemyState.Idle: ProcessEnemyIdle(delta); break;
            case EnemyState.FindingWeapon: ProcessEnemyFindingWeapon(delta); break;
            case EnemyState.Chasing: ProcessEnemyChasing(delta); break;
            case EnemyState.Fleeing: ProcessEnemyFleeing(delta); break;
        }
    }

    protected override void MoveDir(Vector3 direction)
    {
        base.MoveDir(direction);
        RotateTowardsDirection(direction, GetPhysicsProcessDeltaTime());
    }

    protected virtual void ProcessEnemyIdle(double delta)
    {
        MoveDir(Vector3.Zero);
    }
    protected virtual void ProcessEnemyFindingWeapon(double delta)
    {
        if (!(_target is WeaponPickup)) _target = null;
        if (IsInstanceValid(_target))
        {
            PathfindToward(_target.GlobalPosition);
        }
        else
        {
            MoveDir(Vector3.Zero);
            PickTargetWeapon();
        }
    }
    protected virtual void ProcessEnemyChasing(double delta)
    {
        if (IsInstanceValid(_target))
        {
            PathfindToward(_target.GlobalPosition);
            if (IsInstanceValid(_weapon))
            {
                _weapon.ProcessAi(this);
            }

            _entityTargetTimer -= (float)delta;
            if (_entityTargetTimer <= 0)
            {
                PickRandomTargetEntity();
                _entityTargetTimer = EntityTargetTime * (0.5f + Random.Shared.NextSingle());
            }
        }
        else
            PickRandomTargetEntity();
    }
    protected virtual void ProcessEnemyFleeing(double delta)
    {
        Vector3 dirToTarget = (_target.GlobalPosition - GlobalPosition).Normalized();
        PathfindToward(GlobalPosition - dirToTarget * 50f);
    }
    protected virtual void PathfindToward(Vector3 position)
    {
        if (IsInstanceValid(_agent))
        {
            _agent.TargetPosition = position;
            Vector3 nextPosition = _agent.GetNextPathPosition();
            MoveDir(nextPosition - GlobalPosition);
        }
    }

    private void PickRandomTargetEntity()
    {
        if (Entities.Count > 1)
            do
            {
                _target = Entities[Random.Shared.Next() % Entities.Count];
            }
            while (_target == this);
        else _target = null;
    }

    private void PickTargetWeapon()
    {
        ref List<WeaponPickup> weapons = ref WeaponPickup.WeaponPickups;
        if (weapons.Count == 0)
        {
            _target = null;
            return;
        }
        WeaponPickup closest = weapons[0];
        float closestDistance = GlobalPosition.DistanceTo(closest.GlobalPosition);
        for (int i = 1; i < weapons.Count; i++)
        {
            WeaponPickup weapon = weapons[i];
            float distance = GlobalPosition.DistanceTo(weapon.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = weapon;
            }
        }
        _target = closest;
    }

    protected override void SetWeapon(Weapon weapon)
    {
        base.SetWeapon(weapon);
        if (weapon != null)
        {
            GD.Print("PICKED UP");
            _enemyState = EnemyState.Chasing;
            // StandardMaterial3D mat = GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride as StandardMaterial3D;
            // mat.AlbedoColor = Colors.Aqua;
        }
    }
}
