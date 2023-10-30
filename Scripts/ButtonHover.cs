using Godot;
using System;

public partial class ButtonHover : TextureButton
{
	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		mouseOver = true;
		Scale = new(1.2f, 1.2f);
	}

	private void OnMouseExited()
	{
		mouseOver = false;
		Scale = new(1, 1);
	}

	private void OnMouseDown()
	{
		if (mouseOver)
			Scale = new(0.8f, 0.8f);
	}

	private void OnMouseUp()
	{
		if (mouseOver)
			Scale = new(1, 1);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventMouseButton mb)
		{
			if (mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
				OnMouseDown();

			if (mb.IsReleased())
				OnMouseUp();
		}
	}
}
