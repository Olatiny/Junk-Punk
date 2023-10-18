using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

public partial class PlayerController : Area2D
{
	[ExportCategory("Player Information")]
	[Export] public Vector2I gridPosition = new(0, 0);
	[Export] public int playerId = -1;
	[Export] public int health = 10;
	[Export] public int baseAttack = 1;

	//public HeadMod headMod = new HeadMod;
	//public ArmMod[] armMods = new ArmMod[2];
	public LegMod[] legMods = new LegMod[2];

	public Mod[] inventory = new Mod[5];

	private bool mouseOver = false;
	private bool primedToMove = false;
	private bool primedToAttack = false;

	private ModDatabase modDatabase;

	public override void _Ready()
	{
		base._Ready();

		modDatabase = GetNode<ModDatabase>("/root/ModDatabase");

		switch (playerId)
		{
			case 1:
				Mod horsey = modDatabase.GetMod("horsey");
				legMods[0] = horsey.type == Mod.Type.Leg ? (LegMod)horsey : null;

				break;
			case 2:
				Mod bishop = modDatabase.GetMod("bishop");
				legMods[0] = bishop.type == Mod.Type.Leg ? (LegMod)bishop : null;

				break;
			default:
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsPressed() && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (primedToMove)
				{
					if (GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition()))
						primedToMove = false;
				}
				else if (primedToAttack)
				{
					// TODO: Attack Stuff
				}

				// if (mouseOver && !primedToMove)
				// {
				// 	GetParent<ChessBoard>().GetValidMoves(this);
				// 	primedToMove = true;
				// }
				// else if (mouseOver && primedToMove)
				// {
				// 	GetParent<ChessBoard>().ClearValidTiles();
				// 	primedToMove = false;
				// }
				// else if (!mouseOver && primedToMove)
				// {
				// 	if (GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition()))
				// 		primedToMove = false;
				// }
			}
		}

		if (@event.IsActionPressed("Cancel"))
		{
			GetParent<ChessBoard>().ClearValidTiles();
			primedToMove = false;
		}
	}

	public void CheckModDurability()
	{
		// TODO: Implement This
	}

	public void PrimeToMove()
	{
		primedToMove = true;
		primedToAttack = false;
	}

	public void PrimeToAttack()
	{
		primedToMove = false;
		primedToAttack = true;
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
