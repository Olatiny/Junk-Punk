using Godot;
using System;

public partial class ButtonHover : TextureButton
{
    public override void _Ready()
    {
        base._Ready();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
    }

	private void OnMouseEntered()
	{
		Scale = new(1.2f, 1.2f);
	}

	private void OnMouseExited()
	{
		Scale = new(1, 1);
	}
}
