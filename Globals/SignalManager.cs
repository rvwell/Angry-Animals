using Godot;
using System;

public partial class SignalManager : Node
{
	public static SignalManager Instance { get; private set; }

	[Signal] public delegate void OnAnimalDiedEventHandler();
	[Signal] public delegate void OnCupDestroyedEventHandler();

	public override void _Ready()
	{
		Instance = this;
	}

	public static void EmitOnAnimalDied()
	{
		Instance.EmitSignal(SignalName.OnAnimalDied);
	}
	
	public static void EmitOnACupDestroyed()
	{
		Instance.EmitSignal(SignalName.OnCupDestroyed);
	}
}
