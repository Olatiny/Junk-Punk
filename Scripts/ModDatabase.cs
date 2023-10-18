using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class ModDatabase : Node
{
	Dictionary<string, Mod> mods;

	public ModDatabase()
	{
		mods = new();
	}

	private void InitMods()
	{
		Array<Node> children = GetChildren();

		foreach (Node child in children)
		{
			if (child is Mod mod)
				mods.Add(mod.uid, mod);
		}
	}

	public Mod GetMod(string modId)
	{
		if (mods.Count <= 0)
			InitMods();

		return mods[modId];
	}

	public Mod GetRandomMod()
	{
		Node child = GetChildren().PickRandom();

		if (child is Mod mod)
			return mod;

		return null;
	}

	public /*HeadMod*/void GetRandomModOfType(Mod.Type aModType)
	{
		Array<Node> children = GetChildren();
		Mod child = (Mod) children.PickRandom();

		while (child.type != aModType)
		{
			child = (Mod) children.PickRandom();
		}

		//return child;
	}
}
