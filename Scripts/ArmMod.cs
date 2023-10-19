using Godot;
using Godot.Collections;
using System;

public partial class ArmMod : Mod
{	
	[Export] public int range {get; set;}
	[Export] public Array<Vector2I> aoe {get; set;}
	[Export] public int bonusDmg {get; set;}
	
	public ArmMod() : base()
	{ }
	
	public ArmMod(String modUID, int modDurability) : base(modUID, int modDurability)
	{
		type = Type.Arm;
		range = new();
		aoe = new();
		bonusDmg = new();
		
		ArmMod m = new();
	}
}
