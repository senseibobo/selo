using Godot;
using System;

public partial class Pickup : Node3D
{

	public override void _Process(double delta)
	{
		base._Process(delta);
		foreach (Entity entity in Entity.Entities)
		{
			if (entity.GlobalPosition.DistanceTo(GlobalPosition) < 2f)
				OnPickup(entity);
		}

	}

	protected virtual bool OnPickup(Entity entity)
	{
		if (entity.OnPickup(this))
		{
			QueueFree();
			return true;
		}
		return false;
	}
}
