using Godot;
using System;
using System.Collections.Generic;

public partial class Leaderboard : CanvasLayer
{
	public static Leaderboard Instance;
	public static List<LeaderboardEntry> Entries = new();

	[Export] VBoxContainer _leaderboardContainer;
	[Export] PackedScene _leaderboardEntryScene;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public LeaderboardEntry AddEntry(Entity entity)
	{
		LeaderboardEntry entry = _leaderboardEntryScene.Instantiate<LeaderboardEntry>();
		_leaderboardContainer.AddChild(entry);
		Entries.Add(entry);
		entry.SetEntryInfo(entity.Name, 0f);
		return entry;
	}

	public void RemoveEntry(LeaderboardEntry entry)
	{
		Entries.Remove(entry);
	}
}