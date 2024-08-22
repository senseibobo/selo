using Godot;
using System;

public partial class PlayerCamera : Camera3D
{

	private float _strength = 0f;
	private float _timeLeft = 0f;
	private float _totalTime = 1f;
	private float _frequency = 0f;
	private FastNoiseLite _noise = new FastNoiseLite();
	private float _shakePos = 0f;


	public override void _Process(double delta)
	{
		base._Process(delta);
		_shakePos += (float)delta * _frequency;
		_timeLeft = (float)Mathf.MoveToward(_timeLeft, 0f, delta);
		HOffset = (_noise.GetNoise2D(_shakePos, 0f) - 0.5f) * _strength * _timeLeft / _totalTime;
		VOffset = (_noise.GetNoise2D(0f, _shakePos) - 0.5f) * _strength * _timeLeft / _totalTime;
	}

	public void ShakeScreen(float strength, float frequency, float time)
	{
		_totalTime = time;
		_timeLeft = time;
		_strength = strength;
		_frequency = frequency;
	}
}
