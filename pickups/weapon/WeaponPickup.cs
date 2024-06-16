using Godot;
using System;
using System.Collections.Generic;

public partial class WeaponPickup : Pickup
{
	public static List<WeaponPickup> WeaponPickups = new();

	[Export] public PackedScene WeaponScene;

	public override void _EnterTree()
	{
		base._EnterTree();
		WeaponPickups.Add(this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		WeaponPickups.Remove(this);
	}
}
