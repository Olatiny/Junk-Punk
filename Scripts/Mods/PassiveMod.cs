using Godot;
using Godot.Collections;
using System;

public abstract partial class PassiveMod : Mod
{
	public PassiveMod() : base()
	{ }
	
	public virtual void OnPlayerTookDamage(PlayerController Player) {
		
	}
	
	public virtual void OnSetup(PlayerController Player) {
		
	}
	
	public virtual void OnUpkeep(PlayerController Player) {
		
	}
	
	public virtual void OnActionPhase(PlayerController Player) {
		
	}
	
	public virtual void OnSummonedScrap(PlayerController Player) {
		
	}
	
	public virtual void OnCollectedScrap(PlayerController Player) {
		
	}
	
	public virtual void OnModBreak(PlayerController Player) {
		
	}
	
	public virtual void OnPlayerAttack(PlayerController Player) {
		
	}
	
	public virtual void OnModEquip(PlayerController Player) {
		
	}
	
	public virtual void OnMovement(PlayerController Player) {
		
	}
	
	public virtual void OnTurnEnd(PlayerController Player) {
		
	}
	
	public virtual void OnRoundEnd(PlayerController Player) {
		
	}
	
	public virtual void OnTurnStart(PlayerController Player) {
		
	}
	
	public virtual void OnRoundStart(PlayerController Player) {
		
	}
}
