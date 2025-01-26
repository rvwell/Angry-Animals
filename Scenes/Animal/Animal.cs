using Godot;
using System;

public partial class Animal : RigidBody2D
{
	public enum AnimalState {READY, DRAG, RELEASE}
	
	private static readonly Vector2 DRAG_LIM_MAX = new Vector2(0, 60);
	private static readonly Vector2 DRAG_LIM_MIN = new Vector2(-60, 0);

	private const float IMPULSE_MULT = 20.0f;
	private const float IMPULSE_MAX = 1200.0f;
	
	
	[Export] private AudioStreamPlayer2D _strechSound;
	[Export] private AudioStreamPlayer2D _lauchSound;
	[Export] private AudioStreamPlayer2D _kickSound;
	[Export] private Sprite2D _arrow;
	[Export] private VisibleOnScreenNotifier2D _visibleOnScreen;
	[Export] private Label _debugLabel;
	
	private AnimalState _state= AnimalState.READY;
	private float _arrowScaleX;
	private Vector2 _start = Vector2.Zero;
	private Vector2 _dragStart = Vector2.Zero;
	private Vector2 _draggedVector = Vector2.Zero;
	private Vector2 _lastDraggedVector = Vector2.Zero;
	private int _lastCollisionCount = 0;
	
	private void ConnectSignals()
	{
		_visibleOnScreen.ScreenExited += OnExitedScreen;
		SleepingStateChanged += OnSleepingStateChanged;
		InputEvent += OnInputEvent;

	}
	
	private void InitializeVariables()
	{
		_start = Position;
		_arrow.Hide();
		_arrowScaleX = _arrow.Scale.X;
		
	}
	
	public override void _Ready()
	{
		InitializeVariables();
		ConnectSignals();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		UpdateState();
		UpdateDebugLabel();
	}
	
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeidx)
	{
		// GD.Print($"{@event}");
		if (_state == AnimalState.READY &&@event.IsActionPressed("drag"))
		{
			ChangeState(AnimalState.DRAG);
		}
	}

	private void StartDragging()
	{
		_dragStart = GetGlobalMousePosition();
		_arrow.Show();
	}

	private Vector2 CalculateImpulse()
	{
		return _draggedVector * -IMPULSE_MULT;
	}

	private void StartRelease()
	{
		_arrow.Hide();
		_lauchSound.Play();
		Freeze = false;
		ApplyCentralImpulse(CalculateImpulse());
	}

	private void ConstrainDragWithinLimits()
	{
		_lastDraggedVector = _draggedVector;
		_draggedVector = _draggedVector.Clamp(DRAG_LIM_MIN, DRAG_LIM_MAX);
		Position = _start + _draggedVector;
	}

	private void PlayStrechSound()
	{
		Vector2 diff = _draggedVector-_lastDraggedVector;
		if (diff.Length() > 0 && !_strechSound.Playing)
		{
			_strechSound.Play();
		}
	}

	private void UpdateDraggedVector()
	{
		_draggedVector = GetGlobalMousePosition() - _dragStart;
	}

	private bool DetectRelease()
	{
		if (_state == AnimalState.DRAG && Input.IsActionJustReleased("drag"))
		{
			ChangeState(AnimalState.RELEASE);
			return true;
		}
		return false;
	}

	private void UpdateArrowScale()
	{
		float impulseLength = CalculateImpulse().Length();
		float scalePercentage = impulseLength / IMPULSE_MAX;
		_arrow.Scale = new Vector2((_arrowScaleX * scalePercentage) + _arrowScaleX, _arrow.Scale.Y);
		_arrow.Rotation = (_start-Position).Angle();
	}

	private void HandleDragging()
	{
		if(DetectRelease()) return;
		UpdateDraggedVector();
		PlayStrechSound();
		ConstrainDragWithinLimits();
		UpdateArrowScale();
	}

	private void PlayKickSoundOnCollision()
	{
		
		if(_lastCollisionCount == 0 && GetContactCount() > 0 && !_kickSound.Playing)
		{
			_kickSound.Play();
		}

		_lastCollisionCount = GetContactCount();
	}

	private void HandleFlight()
	{
		PlayKickSoundOnCollision();
	}

	private void UpdateState()
	{
		switch (_state)
		{
			case AnimalState.DRAG:
				HandleDragging();
				break;
			case AnimalState.RELEASE:
				HandleFlight();
				break;
		}
	}

	private void ChangeState(AnimalState newState)
	{
		_state = newState;
		switch (_state)
		{
			case AnimalState.DRAG:
				StartDragging();
				break;
			case AnimalState.RELEASE:
				StartRelease();
				break;
		}
	}
	
	private void OnSleepingStateChanged()
	{
		if (Sleeping)
		{
			foreach (Node2D body in GetCollidingBodies())
			{
				if (body is Cup cup)
				{
					cup.Die();
				}
			}

			CallDeferred("Die");
		}
	}


	public void UpdateDebugLabel()
	{
		_debugLabel.Text = $"St:{_state} Sl:{Sleeping}\n";
		_debugLabel.Text += $"{_dragStart.X:F1} {_dragStart.Y:F2}\n";
		_debugLabel.Text += $"{_draggedVector.X:F1} {_draggedVector.Y:F2}";
	}
	private void Die()
	{
		SignalManager.EmitOnAnimalDied();
		QueueFree();
	}
	
	private void OnExitedScreen()
	{
		GD.Print("OnExitedScreen");
		Die();
	}
	
	
}
