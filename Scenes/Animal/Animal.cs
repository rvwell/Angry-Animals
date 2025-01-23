using Godot;
using System;

public partial class Animal : RigidBody2D
{
	[Export] private AudioStreamPlayer2D _strechSound;
	[Export] private AudioStreamPlayer2D _lauchSound;
	[Export] private AudioStreamPlayer2D _kickSound;
	[Export] private Sprite2D _arrow;
	[Export] private VisibleOnScreenNotifier2D _visibleOnScreen;
	[Export] private Label _debugLabel;
	public override void _Ready()
	{
		_visibleOnScreen.Connect(VisibleOnScreenNotifier2D.SignalName.ScreenExited, Callable.From(OnExitedScreen));
	}

	private void OnExitedScreen()
	{
		GD.Print("OnExitedScreen");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
