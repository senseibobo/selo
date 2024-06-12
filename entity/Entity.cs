using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody3D
{

    public static List<Entity> Entities = new();

    public enum EntityState {
        Free,
        Stunned,
        Sliding,
        Attacking

    }

    [Export] Node3D _weaponsNode;

    public virtual float MaxHealth => 100f;
    public virtual float MovementSpeed => 10.0f;
    public virtual float Acceleration => 80.0f;
    public virtual float Deceleration => 120.0f;
    public virtual float Gravity => 9.81f;
    public virtual float SlideDrag => 5.0f;
    public virtual float AttackDrag => 5.0f;
    public float Health;

    private Weapon _weapon;

    private EntityState _entityState;
    private float _yVelocity = 0.0f;

    public override void _EnterTree()
    {
        base._EnterTree();
        Entities.Add(this);
        Health = MaxHealth;
        GD.Print(Health);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Entities.Remove(this);
    }

    public override void _Ready()
    {
        base._Ready();
        Health = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        _yVelocity -= (float)delta*Gravity;
        switch(_entityState) {
            case EntityState.Free: ProcessFree(delta); break;
            case EntityState.Sliding: ProcessSliding(delta); break;
            case EntityState.Stunned: ProcessStunned(delta); break;
            case EntityState.Attacking: ProcessAttacking(delta); break;
        }
        MoveAndSlide();
        _yVelocity = Velocity.Y;
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
    }
    protected virtual void ProcessAttacking(double delta) 
    {
        ApplyDrag((float)delta * AttackDrag);
    }

    protected void MoveDir(Vector3 direction) 
    {
        Vector3 newVelocity = (direction * new Vector3(1f, 0f, 1f)).Normalized() * MovementSpeed + new Vector3(0f, _yVelocity, 0f);
        float change = Acceleration;
        if(newVelocity.Length() < 0.1f || Velocity.Dot(newVelocity) < 0f)
            change = Deceleration;
        Velocity = Velocity.MoveToward(newVelocity, change*(float)GetPhysicsProcessDeltaTime());
    }

    protected void ApplyDrag(float amount)
    {
        Vector2 xzVelocity = new Vector2(Velocity.X, Velocity.Z);
        xzVelocity = xzVelocity.MoveToward(Vector2.Zero, amount);
        Velocity = new Vector3(xzVelocity.X, _yVelocity, xzVelocity.Y);
    }

    public void Hit(float damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            QueueFree();
        }
    }

    protected void Attack()
    {
        _weapon.Attack();
    }

    protected void SetWeapon(Weapon weapon)
    {
        if(IsInstanceValid(_weapon))
            _weapon.QueueFree();// DROP OLD WEAPON
        _weapon = weapon;
        _weaponsNode.AddChild(weapon);
        _weapon.Owner = this;
    }
}
