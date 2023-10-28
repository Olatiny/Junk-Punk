using Godot;
using System;

public partial class Globals : Node
{
	public float masterVolume { get; set; }

	[Signal]
	public delegate void PlayerTookDamageEventHandler(PlayerController player, int damageAmount);

	[Signal]
	public delegate void SetupEventHandler(PlayerController player);

	[Signal]
	public delegate void UpkeepEventHandler(PlayerController player);

	[Signal]
	public delegate void ActionPhaseEventHandler(PlayerController player);

	[Signal]
	public delegate void SummonedScrapEventHandler(PlayerController player, Scrap scrap);

	[Signal]
	public delegate void CollectedScrapEventHandler(PlayerController Player, Scrap scrap);

	[Signal]
	public delegate void ModDamagedEventHandler(PlayerController Player, Mod mod);

	[Signal]
	public delegate void ModBreakEventHandler(PlayerController Player, Mod mod);

	[Signal]
	public delegate void PlayerAttackEventHandler(PlayerController Player, Node2D enemy);

	[Signal]
	public delegate void ModEquipEventHandler(PlayerController Player, Mod mod);

	[Signal]
	public delegate void MovementEventHandler(PlayerController Player);

	[Signal]
	public delegate void TurnEndEventHandler(PlayerController Player);

	[Signal]
	public delegate void RoundEndEventHandler();

	[Signal]
	public delegate void TurnStartEventHandler(PlayerController Player);

	[Signal]
	public delegate void RoundStartEventHandler();
}