using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class ModDatabase : Node
{
	// // [Export]
	// // Array<Mod> modExports;

	// [Export]
	// Dictionary<String, Mod> mods;

	// Initializes all mods available in the game
	// public override void _Ready()
	// {
	// 	Array<Node> children = GetChildren();

	// 	foreach(Node child in children)
	// 	{
	// 		if (child is Mod mod)
	// 			mods.Add(mod.uid, mod);
	// 	}
	// }

	// private void InitMods()
	// {
	// 	LegMod horseyMod = new("horsey");
	// 	horseyMod.AddPath(new Array<string> { "Forward", "Forward", "Left" });
	// 	horseyMod.AddPath(new Array<string> { "Forward", "Forward", "Right" });
	// 	mods.Add(horseyMod.uid, horseyMod);

	// 	LegMod bishopMod = new("bishop");
	// 	bishopMod.AddJumps(new(-1, -1), new(-2, -2), new(-3, -3), 
	// 					   new(-1, 1), new(-2, 2), new(-3, 3),
	// 					   new(1, -1), new(2, -2), new(3, -3),
	// 					   new(1, 1), new(2, 2), new(3, 3));
	// 	mods.Add(bishopMod.uid, bishopMod);
	// }

	public Mod GetMod(string modId)
	{
		Array<Node> children = GetChildren();

		foreach(Node child in children)
		{
			if (child is Mod mod && mod.uid == modId)
				return mod;
		}

		return null;
	}
}
