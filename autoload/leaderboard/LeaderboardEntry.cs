using Godot;
using System;

public partial class LeaderboardEntry : HBoxContainer
{
	[Export] Label _nameLabel;
	[Export] Label _damageLabel;

	float _damage;

	public void SetEntryInfo(string name, float damage)
	{
		_nameLabel.Text = name;
		_damageLabel.Text = damage.ToString();
		_damage = damage;
	}

	public void AddDamage(float damage)
	{
		_damage += damage;
		_damageLabel.Text = _damage.ToString();
	}
}
