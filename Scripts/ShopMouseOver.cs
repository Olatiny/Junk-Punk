using Godot;
using System;

public partial class ShopMouseOver : TextureButton
{
	[Export] TextureRect icon;
	[Export] Control price;

	bool mouseOver = false;

	public override void _Ready()
	{
		base._Ready();
		
		price.Modulate = new(1, 1, 1, 0);
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnMouseEntered()
	{
		mouseOver = true;
		icon.Modulate = new(1, 1, 1, 0);
		price.Modulate = new(1, 1, 1, 1);
	}

	private void OnMouseExited()
	{
		mouseOver = false;
		icon.Modulate = new(1, 1, 1, 1);
		price.Modulate = new(1, 1, 1, 0);
	}
}
