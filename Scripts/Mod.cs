using Godot;
using Godot.Collections;
using System;

public partial class Mod : Sprite2D
{
	public enum Type { Head, Arm, Leg };

	[Export] public Type type { get; set; }
	[Export] public String uid { get; set; }
	[Export] public int durability { get; set; }

	public Mod()
	{
		uid = "";
		durability = 0;
	}

	public Mod(String modUID, int modDurability)
	{
		uid = modUID;
		durability = modDurability;		
	}
}
