using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody3D
{

    public static List<Entity> Entities = new();

    public enum EntityState
    {
        Free,
        Stunned,
        Sliding

    }

    [Export] protected Node3D _weaponsNode;
    [Export] protected PlayerCamera _camera;

    public virtual float MaxHealth => 700f;
    public virtual float MovementSpeed => 10.0f;
    public virtual float Acceleration => 80.0f;
    public virtual float Deceleration => 120.0f;
    public virtual float Gravity => 9.81f;
    public virtual float SlideDrag => 50.0f;
    public virtual float RotationSpeed => 10f;
    public float Health;

    public LeaderboardEntry LeaderboardEntry;

    protected Weapon _weapon;

    private EntityState _entityState;
    private float _yVelocity = 0.0f;
    private bool _attacking;

    public override void _EnterTree()
    {
        LeaderboardEntry = Leaderboard.Instance.AddEntry(this);
        Entities.Add(this);
        Health = MaxHealth;
    }

    public override void _ExitTree()
    {
        //Leaderboard.Instance.RemoveEntry(LeaderboardEntry);
        LeaderboardEntry.Modulate = Colors.Red;
        Entities.Remove(this);
        if (Entities.Count == 1)
        {
            Leaderboard.Instance.EndGame();
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Health = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        _yVelocity -= (float)delta * Gravity;
        switch (_entityState)
        {
            case EntityState.Free: ProcessFree(delta); break;
            case EntityState.Sliding: ProcessSliding(delta); break;
            case EntityState.Stunned: ProcessStunned(delta); break;
        }
        MoveAndSlide();
        _yVelocity = Velocity.Y;
        if (GlobalPosition.Y < -100f)
        {
            Death(null);
        }
    }

    protected virtual void ProcessFree(double delta)
    {

    }

    protected virtual void ProcessStunned(double delta)
    {
        Velocity = new Vector3(0f, _yVelocity, 0f);
    }

    protected virtual void ProcessSliding(double delta)
    {
        ApplyDrag((float)delta * SlideDrag);
        if (Velocity.Length() < 2f)
            _entityState = EntityState.Free;
    }

    protected virtual void MoveDir(Vector3 direction)
    {
        Vector3 newVelocity = (direction * new Vector3(1f, 0f, 1f)).Normalized() * MovementSpeed + new Vector3(0f, _yVelocity, 0f);
        float change = Acceleration;
        if (newVelocity.Length() < 0.1f || Velocity.Dot(newVelocity) < 0f)
            change = Deceleration;
        Velocity = Velocity.MoveToward(newVelocity, change * (float)GetPhysicsProcessDeltaTime());
        if (_attacking)
            ApplyDrag(_weapon.Drag * (float)GetPhysicsProcessDeltaTime());
    }

    protected void RotateTowardsDirection(Vector3 direction, double delta)
    {
        float angle = -new Vector2(direction.X, direction.Z).Angle() - Mathf.Pi / 2f;
        Vector3 rot = Rotation;
        rot.Y = Mathf.LerpAngle(rot.Y, angle, RotationSpeed * (float)delta);
        Rotation = rot;
    }

    protected void ApplyDrag(float amount)
    {
        Vector2 xzVelocity = new Vector2(Velocity.X, Velocity.Z);
        xzVelocity = xzVelocity.MoveToward(Vector2.Zero, amount);
        Velocity = new Vector3(xzVelocity.X, _yVelocity, xzVelocity.Y);
    }

    public virtual void Hit(float damage, Vector3 direction, Entity dealer)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Death(dealer);
            return;
        }
        _entityState = EntityState.Sliding;
        Velocity = Mathf.Max(damage - 5f, 0f) * direction * 4f;
    }

    protected void Attack()
    {
        if (_weapon != null)
        {
            _weapon.Attack();
            _attacking = true;
        }
    }

    protected void ReleaseAttack()
    {
        if (_weapon != null)
        {
            _weapon.ReleaseAttack();
        }
    }

    protected virtual void SetWeapon(Weapon weapon)
    {
        if (IsInstanceValid(_weapon))
        {
            _weapon.QueueFree();// DROP OLD WEAPON
            _weapon.FinishedAttack -= OnWeaponFinishedAttack;
        }
        _weapon = weapon;
        _weaponsNode.AddChild(weapon);
        _weapon.FinishedAttack += OnWeaponFinishedAttack;
        _weapon.Owner = this;
    }

    protected virtual void Death(Entity killer)
    {
        if (_camera != null)
        {
            if (killer == null) killer = Entities[(int)Random.Shared.NextInt64() % Entities.Count];
            ReparentCamera(killer);
        }
        QueueFree();
    }

    protected void ReparentCamera(Entity newOwner)
    {
        Vector3 oldPos = _camera.Position;
        _camera.Reparent(newOwner);
        _camera.Position = oldPos;
        newOwner._camera = _camera;
        _camera.Rotation = new Vector3();
    }

    private void OnWeaponFinishedAttack()
    {
        _attacking = false;
    }

    // returns true if picked up else false
    public virtual bool OnPickup(Pickup pickup)
    {
        if (pickup is WeaponPickup weaponPickup)
        {
            if (_weapon == null)
            {
                SetWeapon(weaponPickup.WeaponScene.Instantiate<Weapon>());
                return true;
            }
        }
        return false;
    }
}
