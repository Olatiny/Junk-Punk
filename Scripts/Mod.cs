using Godot;
using Godot.Collections;
using System;

public partial class Mod : Sprite2D
{
	public enum Type { Head, Arm, Leg };

	[Export] public Type type { get; set; }
	[Export] public String uid { get; set; }

	public Mod()
	{
		uid = "";
	}

	public Mod(String modUID)
	{
		uid = modUID;
	}
}
