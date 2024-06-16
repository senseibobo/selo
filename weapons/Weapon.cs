using Godot;
using System;

public partial class Weapon : Node3D
{
    public Action FinishedAttack;
    public virtual float Damage => 10f;
    [Export] AnimationPlayer _animationPlayer;
    [Export] Area3D _hitArea;
    [Export] public MeshInstance3D WeaponMesh;

    public override void _EnterTree()
    {
        base._EnterTree();
        _animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animation)
    {
        FinishedAttack?.Invoke();
    }

    public virtual void Attack()
    {
        _animationPlayer.Play("attack");
    }

    public virtual void Hit()
    {
        foreach (Node area in _hitArea.GetOverlappingAreas())
        {
            Node parent = area.GetParent();
            if (parent is Entity entity && parent != Owner)
            {
                entity.Hit(Damage, -GlobalBasis.Z.Normalized());
                (Owner as Entity).LeaderboardEntry.AddDamage(Damage);
            }
        }
    }

    public void SetDepthTest(bool depthTest)
    {
        MeshInstance3D weaponMesh = WeaponMesh;
        Mesh mesh = weaponMesh.Mesh;
        StandardMaterial3D originalMaterial = mesh.SurfaceGetMaterial(0) as StandardMaterial3D;
        StandardMaterial3D newMaterial = originalMaterial.Duplicate(true) as StandardMaterial3D;
        mesh.SurfaceSetMaterial(0, newMaterial);
        newMaterial.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;
        newMaterial.NoDepthTest = depthTest;
    }
}
