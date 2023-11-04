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

		return mods[modId].Clone();
	}

	public Mod GetRandomMod()
	{
		Node child = GetChildren().PickRandom();

		if (child is Mod mod)
			return mod.Clone();

		return null;
	}

	public Mod GetRandomModByBodyPart(Mod.BodyPart bodyPart)
	{
		Mod returnMod;

		do
		{
			returnMod = GetRandomMod();
		}
		while (returnMod.bodyPart != bodyPart);

		return returnMod;
	}

	public Array<Mod> GetShopSpread()
	{
		Array<Mod> returnArray = new();
		returnArray.Add(GetRandomModByBodyPart(Mod.BodyPart.Head));
		returnArray.Add(GetRandomModByBodyPart(Mod.BodyPart.Arm));

		while (returnArray.Count < 3)
		{
			Mod mod = GetRandomModByBodyPart(Mod.BodyPart.Arm);
			if (mod.uid != returnArray[^1].uid)
				returnArray.Add(mod);
		}

		returnArray.Add(GetRandomModByBodyPart(Mod.BodyPart.Leg));

		while (returnArray.Count < 5)
		{
			Mod mod = GetRandomModByBodyPart(Mod.BodyPart.Leg);
			if (mod.uid != returnArray[^1].uid)
				returnArray.Add(mod);
		}

		return returnArray;
	}
}
