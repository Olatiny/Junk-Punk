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
	[Export] public int baseAttackDmg = 1;
	[Export] public int baseAttackRange = 1;
	[Export] public int currentScrap = 0;
	[Export] public int scrapIncome = 10;

	//public HeadMod headMod = new HeadMod;
	public ArmMod[] armMods = new ArmMod[2];
	public LegMod[] legMods = new LegMod[2];

	public Mod[] inventory = new Mod[5];

	private bool mouseOver = false;
	private bool primedToMove = false;
	private bool primedToAttack = false;
	private int activeAttackModIdx = -1;

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
				Mod bishop = modDatabase.GetMod("rook");
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
					if (GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition()))
						primedToAttack = false;
				}
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

	public void SetActiveAttackMod(Mod mod)
	{
		if (mod == null || mod is not ArmMod)
			return;

		if (mod.uid == armMods[0].uid)
			activeAttackModIdx = 0;
		else if (mod.uid == armMods[1].uid)
			activeAttackModIdx = 1;
	}

	public ArmMod GetActiveAttackMod()
	{
		if (activeAttackModIdx == -1 || armMods[activeAttackModIdx] == null)
			return null;

		return armMods[activeAttackModIdx];
	}

	// returns true if player is killed, they still take damage if false
	public bool TakeDamage(int damage)
	{
		health -= damage;

		return damage <= 0;
	}

	// TODO: Implement Scrap
	// public bool HarvestScrap(Scrap scrap)
	// {
	// 	currentScrap += scrap.value;
	// }

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
