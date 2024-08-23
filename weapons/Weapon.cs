using Godot;
using Godot.Collections;
using System;

public partial class Weapon : Node3D
{
    public Action FinishedAttack;
    public virtual float Damage => 10f;
    public virtual float Drag => 85f;
    public virtual bool AttackWhileSliding => true;
    [Export] protected AnimationPlayer _animationPlayer;
    [Export] protected Area3D _hitArea;
    [Export] public MeshInstance3D WeaponMesh;
    [Export] public PackedScene _pickupScene;
    [Export] protected PackedScene _powScene;
    [Export] protected Godot.Collections.Array<AudioStream> _hitSounds;

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

    public virtual void ReleaseAttack()
    {

    }

    public virtual void Hit()
    {
        foreach (Node area in _hitArea.GetOverlappingAreas())
        {
            Node parent = area.GetParent();
            if (parent is Entity entity && parent != Owner)
            {
                entity.Hit(Damage, -GlobalBasis.Z.Normalized(), Owner as Entity);
                Node3D powInstance = _powScene.Instantiate<Node3D>();
                GetTree().CurrentScene.AddChild(powInstance);
                float r1 = Random.Shared.NextSingle() - 0.5f;
                float r2 = Random.Shared.NextSingle() - 0.5f;
                float r3 = Random.Shared.NextSingle() - 0.5f;
                powInstance.GlobalPosition = entity.GlobalPosition + Vector3.Up * 1.2f + new Vector3(r1, r2, r3) * 0.6f;
                (Owner as Entity).LeaderboardEntry.AddDamage(Damage);
                PlayHitSound();
            }
        }
    }

    protected void PlayHitSound()
    {
        AudioStream sound = _hitSounds.PickRandom();
        AudioStreamPlayer3D player = new AudioStreamPlayer3D();
        player.Stream = sound;
        AddChild(player);
        player.GlobalPosition = GlobalPosition;
        player.Finished += player.QueueFree;
        player.Play();
    }

    public virtual void ProcessAi(Entity owner)
    {
        foreach (Node area in _hitArea.GetOverlappingAreas())
        {
            Node parent = area.GetParent();
            if (parent is Entity entity)
            {
                if (entity != owner)
                {
                    GetTree().CreateTimer(0.1).Timeout += Attack;
                    break;
                }
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
