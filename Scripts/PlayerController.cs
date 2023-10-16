using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerController : Area2D
{
	[ExportCategory("Player Information")]
	[Export] public Vector2I gridPosition = new(0, 0);
	[Export] public int playerId = -1;
	[Export] public int health = 10;
	[Export] public int baseAttack = 1;
	[Export] public Array<Array<String>> modMovement;
	// [Export(PropertyHint.Enum, "Forward,Left,Right")] public Array<String> testStr;

	private bool mouseOver = false;
	private bool primedToMove = false;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsPressed())
			{
				if (mouseOver && !primedToMove)
				{
					GetParent<ChessBoard>().GetValidMoves(this);
					primedToMove = true;
				}
				else if (mouseOver && primedToMove)
				{
					GetParent<ChessBoard>().ClearValidTiles();
					primedToMove = false;
				}
				else if (!mouseOver && primedToMove)
				{
					if (GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition()))
						primedToMove = false;
				}
			}
		}

		if (@event.IsActionPressed("Cancel"))
		{
			GetParent<ChessBoard>().ClearValidTiles();
			primedToMove = false;
		}
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
