using Godot;
using System;

public partial class Weapon : Node3D
{
    public virtual float Damage => 10f;
    [Export] AnimationPlayer _animationPlayer;
    [Export] Area3D _hitArea;
    public virtual void Attack()
    {
        _animationPlayer.Play("attack");
    }

    public virtual void Hit()
    {
        GD.Print("trying to hit");
        foreach(Node area in _hitArea.GetOverlappingAreas())
        {
            GD.Print("BOdy " + area.ToString());
            Node parent = area.GetParent();
            if(parent is Entity entity && parent != Owner)
            {
                entity.Hit(Damage);
                GD.Print("HIT!");
            }
        }
    }
}
