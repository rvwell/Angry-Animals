using Godot;
using System;

public partial class Water : Area2D
{
	[Export] private AudioStreamPlayer2D _splashSound;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		_splashSound.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
