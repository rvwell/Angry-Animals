using Godot;
using System;

public partial class Level : Node2D
{
	[Export] private Marker2D _animalStart;
	[Export] private PackedScene _animalScene;
	public override void _Ready()
	{
		SignalManager.Instance.OnAnimalDied += OnAnimalDied;
		OnAnimalDied();
	}

	private void OnAnimalDied()
	{
		Animal animal = _animalScene.Instantiate<Animal>();
		animal.Position = _animalStart.Position;
		AddChild(animal);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnAnimalDied -= OnAnimalDied;
	}
}
