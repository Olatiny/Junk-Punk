using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Linq;
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
	public bool primedToMove { get; private set; } = false;
	public bool primedToAttack { get; private set; } = false;
	private int activeAttackModIdx = -1;

	private ModDatabase modDatabase;
	private GameManager gameManager;

	public override void _Ready()
	{
		base._Ready();

		modDatabase = GetNode<ModDatabase>("/root/ModDatabase");
		gameManager = GetParent().GetParent<GameManager>();

		switch (playerId)
		{
			case 1:
				Mod horsey = modDatabase.GetMod("KnightLeg");
				legMods[0] = horsey.bodyPart == Mod.BodyPart.Leg ? (LegMod)horsey : null;

				Mod hands = modDatabase.GetMod("BurningHands");
				armMods[0] = hands.bodyPart == Mod.BodyPart.Arm ? (ArmMod)hands : null;
				activeAttackModIdx = 0;

				break;
			case 2:
				Mod bishop = modDatabase.GetMod("RookLeg");
				legMods[0] = bishop.bodyPart == Mod.BodyPart.Leg ? (LegMod)bishop : null;

				Mod cone = modDatabase.GetMod("BishopArm");
				armMods[0] = cone.bodyPart == Mod.BodyPart.Arm ? (ArmMod)cone : null;
				activeAttackModIdx = 0;

				break;
			default:
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsReleased() && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (primedToMove)
				{
					if (GetParent<ChessBoard>().RequestMove(this, GetGlobalMousePosition()))
						primedToMove = false;
				}
				else if (primedToAttack)
				{
					if (GetParent<ChessBoard>().RequestAttack(this, GetGlobalMousePosition()))
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

	public void SetActiveAttackMod(int attackModIdx)
	{
		if (attackModIdx >= 0 && attackModIdx < armMods.Length)
		{
			if (armMods[attackModIdx] == null)
			{
				activeAttackModIdx = -1;

				if (primedToAttack)
					gameManager.GetValidAttackTiles();

				return;
			}
		}
		
		activeAttackModIdx = attackModIdx;

		if (primedToAttack)
			gameManager.GetValidAttackTiles();
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

		gameManager.UpdateScoreBoard();

		if (health <= 0)
			gameManager.DeclareVictory();

		return health <= 0;
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

	public void UnPrime()
	{
		primedToAttack = primedToMove = false;
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
