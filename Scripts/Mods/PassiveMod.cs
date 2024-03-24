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

		// InitSignals();
	}

	// Call this on cloned mods.
	public override void InitSignals()
	{
		Globals globals = GetNode<Globals>("/root/Globals");
		globals.PlayerTookDamage += OnPlayerTookDamage;
		globals.Upkeep += OnUpkeep;
		globals.SummonedScrap += OnSummonedScrap;
		globals.CollectedScrap += OnCollectedScrap;
		globals.ModUnequip += OnModUnequip;

		// These ones aren't emitted in board/game manager yet. If we decide we need them we can add those calls where we'd need them
		globals.Setup += OnSetup;
		globals.ActionPhase += OnActionPhase;
		globals.ModDamaged += OnModDamaged;
		globals.PlayerAttack += OnPlayerAttack;
		globals.ModEquip += OnModEquip;
		globals.Movement += OnMovement;
		globals.TurnEnd += OnTurnEnd;
		globals.RoundEnd += OnRoundEnd;
		globals.TurnStart += OnTurnStart;
		globals.RoundStart += OnRoundStart;
	}

	public override void DisconnectSignals()
	{
		Globals globals = GetNode<Globals>("/root/Globals");
		globals.PlayerTookDamage -= OnPlayerTookDamage;
		globals.Upkeep -= OnUpkeep;
		globals.SummonedScrap -= OnSummonedScrap;
		globals.CollectedScrap -= OnCollectedScrap;

		// These ones aren't emitted in board/game manager yet. If we decide we need them we can add those calls where we'd need them
		globals.Setup -= OnSetup;
		globals.ActionPhase -= OnActionPhase;
		globals.ModDamaged -= OnModDamaged;
		globals.ModBreak -= OnModUnequip;
		globals.PlayerAttack -= OnPlayerAttack;
		globals.ModEquip -= OnModEquip;
		globals.Movement -= OnMovement;
		globals.TurnEnd -= OnTurnEnd;
		globals.RoundEnd -= OnRoundEnd;
		globals.TurnStart -= OnTurnStart;
		globals.RoundStart -= OnRoundStart;
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

	public virtual void OnModUnequip(PlayerController Player, Mod mod)
	{ }

	public virtual void OnPlayerAttack(PlayerController Player, Node2D enemy)
	{ }

	public virtual void OnModEquip(PlayerController Player, Mod mod, bool isLeft)
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

	public override Mod Clone()
	{
		Mod duplicate = base.Clone();
		duplicate.SetScript(GetScript());

		PassiveMod passiveDupe = (PassiveMod) duplicate;
		// passiveDupe.InitSignals();

		return passiveDupe;
	}
}
