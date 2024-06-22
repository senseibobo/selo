using Godot;
using System;

public partial class Axe : Weapon
{
	[Export] Area3D _scanArea;
	public override float Damage => (0.5f + (float)_animationPlayer.CurrentAnimationPosition) * 40f;
	public override float Drag => 0f;
	public override bool AttackWhileSliding => true;


	public override void Attack()
	{
		if (!_animationPlayer.IsPlaying())
			_animationPlayer.Play("attack_buildup");
	}

	public override void ReleaseAttack()
	{
		_animationPlayer.Play("release_attack");
	}

	public override void ProcessAi(Entity owner)
	{
		if (_animationPlayer.CurrentAnimation == "attack_buildup")
		{
			bool insideHitArea = EntityInsideArea(_hitArea);
			if (insideHitArea)
				ReleaseAttack();
		}
		else
		{
			bool insideScanArea = EntityInsideArea(_scanArea);
			if (insideScanArea)
				Attack();
		}
	}

	private bool EntityInsideArea(Area3D area)
	{
		bool insideArea = false;
		foreach (Node child in area.GetOverlappingAreas())
		{
			Node parent = child.GetParent();
			if (parent is Entity entity)
			{
				insideArea = true;
				break;
			}
		}
		return insideArea;
	}
}
