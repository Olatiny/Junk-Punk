using Godot;
using Godot.Collections;
using System;

public abstract partial class PassiveMod : Mod
{
	public PassiveMod() : base()
	{ }

	public override void _Ready()
	{
		base._Ready();

		Globals globals = GetNode<Globals>("/root/Globals");
		globals.PlayerTookDamage += OnPlayerTookDamage;
		globals.Setup += OnSetup;
		globals.Upkeep += OnUpkeep;
		globals.ActionPhase += OnActionPhase;
		globals.SummonedScrap += OnSummonedScrap;
		globals.CollectedScrap += OnCollectedScrap;
		globals.ModDamaged += OnModDamaged;
		globals.ModBreak += OnModBreak;
		globals.PlayerAttack += OnPlayerAttack;
		globals.ModEquip += OnModEquip;
		globals.Movement += OnMovement;
		globals.TurnEnd += OnTurnEnd;
		globals.RoundEnd += OnRoundEnd;
		globals.TurnStart += OnTurnStart;
		globals.RoundStart += OnRoundStart;
	}

	public virtual void OnPlayerTookDamage(PlayerController Player, int damageAmount)
	{ }

	public virtual void OnSetup(PlayerController Player)
	{ }

	public virtual void OnUpkeep(PlayerController Player)
	{ }

	public virtual void OnActionPhase(PlayerController Player)
	{ }

	public virtual void OnSummonedScrap(PlayerController Player, Scrap scrap)
	{ }

	public virtual void OnCollectedScrap(PlayerController Player, Scrap scrap)
	{ }

	public virtual void OnModDamaged(PlayerController player, Mod mod)
	{ }

	public virtual void OnModBreak(PlayerController Player, Mod mod)
	{ }

	public virtual void OnPlayerAttack(PlayerController Player, Node2D enemy)
	{ }

	public virtual void OnModEquip(PlayerController Player, Mod mod)
	{ }

	public virtual void OnMovement(PlayerController Player)
	{ }

	public virtual void OnTurnEnd(PlayerController Player)
	{ }

	public virtual void OnRoundEnd()
	{ }

	public virtual void OnTurnStart(PlayerController Player)
	{ }

	public virtual void OnRoundStart()
	{ }
}
