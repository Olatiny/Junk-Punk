using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class MovementTest : Node2D
{
	enum InputTypes { Right, Left, Up, Down };

	public override void _Input(InputEvent @event)
	{
		var values = Enum.GetValues(typeof(InputTypes));

		foreach (InputTypes value in values)
		{
			if (@event.IsActionPressed(value.ToString()))
				GetParent<ChessBoard>().RequestMove(value.ToString());
		}
	}
}
