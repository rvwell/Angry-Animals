using Godot;
using System;

public partial class Cup : StaticBody2D
{
	
	[Export] private AnimationPlayer _animationPlayer;
	public override void _Ready()
	{
		_animationPlayer.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animname)
	{
		SignalManager.EmitOnACupDestroyed();
		QueueFree();
	}

	public void Die()
	{
		_animationPlayer.Play("vanish");
	}
}
