using Godot;
using System;

public partial class Player : Entity
{
    public static Player Instance;
    public virtual float Sensitivity => 1f;

    public override void _EnterTree()
    {
        base._EnterTree();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Instance = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Instance == this)
            Instance = null;
        //Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    protected override void ProcessFree(double delta)
    {
        float hInput = Input.GetAxis("move_left", "move_right");
        float vInput = Input.GetAxis("move_forward", "move_back");
        MoveDir(Basis.Z * vInput + Basis.X * hInput);
        if (Input.IsActionJustPressed("attack"))
            Attack();
        else if (Input.IsActionJustReleased("attack"))
            ReleaseAttack();
    }

    protected override void ProcessSliding(double delta)
    {
        base.ProcessSliding(delta);
        if (Input.IsActionJustReleased("attack"))
            ReleaseAttack();
    }

    protected override void SetWeapon(Weapon weapon)
    {
        base.SetWeapon(weapon);
        weapon.SetDepthTest(false);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            _camera.RotateX(-motion.Relative.Y / 200f * Sensitivity);
            RotateY(-motion.Relative.X / 200f * Sensitivity);
            Vector3 rot = _camera.RotationDegrees;
            rot.X = Mathf.Clamp(rot.X, -20f, 20f);
            _camera.RotationDegrees = rot;
        }
    }

    public override void Hit(float damage, Vector3 direction, Entity dealer)
    {
        base.Hit(damage, direction, dealer);
        _camera.ShakeScreen(damage / 20f, 600f, 0.5f);
    }

    protected override void Death(Entity killer)
    {
        base.Death(killer);
        GetTree().Root.GetNode("DeathOverlay").Call("enable");
    }
}
