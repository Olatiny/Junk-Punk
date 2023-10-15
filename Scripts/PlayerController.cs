using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerController : Area2D
{
	public Vector2I boardPosition = new(0, 0);

	private bool mouseOver = false;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsPressed() && mouseOver)
				GetParent<ChessBoard>().GetValidMoves();
			else if (mouseEvent.IsPressed())
				GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition());
		}

		if (@event.IsActionPressed("Cancel"))
			GetParent<ChessBoard>().ClearValidTiles();
	}

    public override void _MouseEnter()
    {
		mouseOver = true;
    }

    public override void _MouseExit()
    {
        mouseOver = false;
    }
}
