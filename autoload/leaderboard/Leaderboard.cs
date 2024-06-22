using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Leaderboard : CanvasLayer
{
	public static Leaderboard Instance;
	public static List<LeaderboardEntry> Entries = new();

	[Export] VBoxContainer _leaderboardContainer;
	[Export] PackedScene _leaderboardEntryScene;
	[Export] AnimationPlayer _animationPlayer;
	[Export] Label _judgementLabel;

	bool _restartable = false;
	bool _gameEnded = false;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public LeaderboardEntry AddEntry(Entity entity)
	{
		LeaderboardEntry entry = _leaderboardEntryScene.Instantiate<LeaderboardEntry>();
		_leaderboardContainer.AddChild(entry);
		Entries.Add(entry);
		entry.Name = entity.Name;
		entry.SetEntryInfo(entity.Name, 0f);
		return entry;
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Keycode == Key.R && _restartable)
			{
				_restartable = false;
				Enemy.RegenerateNames();
				Entries.Clear();
				foreach (Node child in _leaderboardContainer.GetChildren())
				{
					if (child is LeaderboardEntry) child.QueueFree();
				}
				_animationPlayer.Play("RESET");
				GetTree().ReloadCurrentScene();
				GetTree().CreateTimer(0.2f).Timeout += OnGameStart;
			}
		}
	}

	private void OnGameStart()
	{
		_gameEnded = false;
	}

	public void RemoveEntry(LeaderboardEntry entry)
	{
		Entries.Remove(entry);
	}

	public void SortEntries()
	{
		Entries.Sort((LeaderboardEntry e1, LeaderboardEntry e2) => (int)(e2.Damage - e1.Damage));
		for (int i = 0; i < Entries.Count; i++)
		{
			_leaderboardContainer.MoveChild(Entries[i], i + 1);
		}
	}

	public void EndGame()
	{
		if (_gameEnded) return;
		_gameEnded = true;
		_animationPlayer.Play("end");
		_judgementLabel.Text = _leaderboardContainer.GetChild<LeaderboardEntry>(1).Name != "You" ? "Not good enough..." : "Well done.";
		_judgementLabel.Text += "\nPress R to restart.";
		_restartable = true;
	}
}