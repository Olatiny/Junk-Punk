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
	[Export] PackedScene sparks;

	public enum Direction
	{
		up, down, left, right
	}

	[Export] public Direction direction;

	[ExportCategory("UI References")]
	[Export] public InventoryCollection inventoryCollection;

	[ExportCategory("Sprite References")]
	[Export] Texture defBigHead;
	[Export] Texture defBigArm;
	[Export] Texture defBigLeg;
	[Export] AnimatedSprite2D background;
	[Export] AnimatedSprite2D head;
	[Export] AnimatedSprite2D body;
	[Export] AnimatedSprite2D[] arms;
	[Export] AnimatedSprite2D[] legs;

	public Mod headMod = null;
	public Mod[] armMods = new Mod[2];
	public Mod[] legMods = new Mod[2];

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
				// Equip(modDatabase.GetMod("KnightLegPath"), Mod.BodyPart.Leg, 0);
				// Equip(modDatabase.GetMod("BishopLeg"), Mod.BodyPart.Leg, 1);
				// Equip(modDatabase.GetMod("KingArm"), Mod.BodyPart.Arm, 1);
				// Equip(modDatabase.GetMod("BishopHead"), Mod.BodyPart.Head, 0);

				break;
			case 2:
				// Equip(modDatabase.GetMod("QueenLeg"), Mod.BodyPart.Leg, 0);
				// Equip(modDatabase.GetMod("BishopArmPassive"), Mod.BodyPart.Arm, 1);
				// Equip(modDatabase.GetMod("QueenArm"), Mod.BodyPart.Arm, 0);

				break;
			default:
				break;
		}

		UpdateDirection(direction);
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

					UpdateSprites();
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

	public void UpdateDirection(Vector2I source, Vector2I destination)
	{
		Vector2 dir = destination - source;
		dir = dir.Normalized();

		if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
		{
			if (dir.X > 0)
				direction = Direction.right;
			else
				direction = Direction.left;
		}
		else
		{
			if (dir.Y > 0)
				direction = Direction.down;
			else
				direction = Direction.up;
		}

		UpdateSprites();
	}

	public void UpdateDirection(Direction newDirection)
	{
		direction = newDirection;

		UpdateSprites();
	}

	public void UpdateSprites()
	{
		string directionString = direction.ToString();
		bool flipH = false;

		if (direction == Direction.left)
		{
			flipH = true;
			directionString = "side";
		}
		else if (direction == Direction.right)
			directionString = "side";

		if (playerId == 1)
			background.Play("blue_" + directionString);
		else
			background.Play("red_" + directionString);

		background.FlipH = flipH;
		head.FlipH = flipH;
		body.FlipH = flipH;
		arms[0].FlipH = flipH;
		arms[1].FlipH = flipH;
		legs[0].FlipH = flipH;
		legs[1].FlipH = flipH;

		head.Play(directionString);
		body.Play(directionString);
		arms[0].Play(directionString);
		arms[1].Play(directionString);
		legs[0].Play(directionString);
		legs[1].Play(directionString);

		ZIndex = gridPosition.Y + 2;
	}

	public void CheckModDurability()
	{
		if (headMod != null)
		{
			headMod.durability--;

			if (headMod.durability <= 0)
				Unequip(headMod);
		}

		foreach (Mod mod in armMods)
		{
			if (mod == null)
				continue;

			mod.durability--;
			if (mod.durability <= 0)
				Unequip(mod);
		}

		foreach (Mod mod in legMods)
		{
			if (mod == null)
				continue;

			mod.durability--;
			if (mod.durability <= 0)
				Unequip(mod);
		}
	}

	public void Equip(Mod mod, Mod.BodyPart bodyPart, int limbIdx = 0)
	{
		GD.Print("Equip Output");
		GD.Print(mod);
		GD.Print(bodyPart);
		GD.Print(limbIdx);
		if (mod.bodyPart != bodyPart)
			return;

		switch (bodyPart)
		{
			case Mod.BodyPart.Head:
				Unequip(bodyPart, limbIdx);
				EquipHelper(ref mod, ref headMod);
				head.Modulate = mod.bodyPartTint;

				break;
			case Mod.BodyPart.Arm:
				Unequip(bodyPart, limbIdx);
				EquipHelper(ref mod, ref armMods[limbIdx]);
				if (arms != null)
					arms[limbIdx].Modulate = mod.bodyPartTint;

				break;
			case Mod.BodyPart.Leg:
				Unequip(bodyPart, limbIdx);
				EquipHelper(ref mod, ref legMods[limbIdx]);
				if (legs != null)
					legs[limbIdx].Modulate = mod.bodyPartTint;

				break;

		}
	}

	public void EquipHelper(ref Mod mod, ref Mod bodyPart)
	{
		bodyPart = mod;
		AddChild(mod);
		mod.InitSignals();

		// TODO: Equip stuff
	}

	public void Unequip(Mod mod)
	{
		if (mod == headMod)
		{
			UnequipHelper(ref mod, ref headMod);
			if (head != null)
				head.Modulate = new(1, 1, 1, 1);
		}

		for (int i = 0; i < armMods.Length; i++)
		{
			if (mod == armMods?[i])
			{
				UnequipHelper(ref mod, ref armMods[i]);
				if (arms?[i] != null)
					arms[i].Modulate = new(1, 1, 1, 1);
			}
		}

		for (int i = 0; i < legMods.Length; i++)
		{
			if (mod == legMods?[i])
			{
				UnequipHelper(ref mod, ref legMods[i]);
				if (legs?[i] != null)
					legs[i].Modulate = new(1, 1, 1, 1);
			}
		}
	}

	public void Unequip(Mod.BodyPart bodyPart, int limbIdx = 0)
	{
		Mod mod = null;
		switch (bodyPart)
		{
			case Mod.BodyPart.Head:
				{
					mod = headMod;
					UnequipHelper(ref mod, ref headMod);
					head.Modulate = new(1, 1, 1, 1);
					break;
				}
			case Mod.BodyPart.Arm:
				{
					mod = armMods?[limbIdx];
					UnequipHelper(ref mod, ref armMods[limbIdx]);
					if (arms != null)
						arms[limbIdx].Modulate = new(1, 1, 1, 1);
					break;
				}
			case Mod.BodyPart.Leg:
				{
					mod = legMods?[limbIdx];
					UnequipHelper(ref mod, ref legMods[limbIdx]);
					if (legs != null)
						legs[limbIdx].Modulate = new(1, 1, 1, 1);
					break;
				}
		}
	}

	private void UnequipHelper(ref Mod mod, ref Mod bodyPart)
	{
		if (mod == null || bodyPart == null)
			return;

		bodyPart = null;

		GpuParticles2D sparksEmitter = sparks.Instantiate() as GpuParticles2D;
		sparksEmitter.Position = Position;
		sparksEmitter.ZIndex = head.ZIndex + 10;
		GetTree().Root.AddChild(sparksEmitter);

		mod.DisconnectSignals();
		mod.QueueFree();
		return;
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

		return armMods[activeAttackModIdx] is ArmMod mod ? mod : null;
	}

	// returns true if player is killed, they still take damage if false
	public bool TakeDamage(int damage)
	{
		health -= damage;

		GpuParticles2D sparksEmitter = sparks.Instantiate() as GpuParticles2D;
		sparksEmitter.Position = Position;
		sparksEmitter.ZIndex = head.ZIndex + 10;
		GetTree().Root.AddChild(sparksEmitter);

		gameManager.UpdateScoreBoard();
		Globals globals = GetNode<Globals>("/root/Globals");
		globals.EmitSignal(Globals.SignalName.PlayerTookDamage, this, damage);

		gameManager.audioManager.FXdamage();

		if (health <= 0)
		{
			if (headMod == null || headMod is not PawnHeadMod)
				gameManager.DeclareVictory();
			else if (headMod is PawnHeadMod pawnHead)
				pawnHead.OnPlayerTookDamage(this, damage);
		}

		return health <= 0;
	}

	public void SpendScrap(int amt)
	{
		currentScrap -= amt;
		if (currentScrap < 0)
			currentScrap = 0;
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
