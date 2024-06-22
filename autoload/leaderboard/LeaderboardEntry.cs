using Godot;
using System;

public partial class LeaderboardEntry : HBoxContainer
{
	[Export] Label _nameLabel;
	[Export] Label _damageLabel;

	public float Damage;

	public void SetEntryInfo(string name, float damage)
	{
		_nameLabel.Text = name;
		_damageLabel.Text = damage.ToString();
		Damage = damage;
	}

	public void AddDamage(float damage)
	{
		Damage += damage;
		_damageLabel.Text = Damage.ToString();
		Leaderboard.Instance.SortEntries();
	}
}
