using Godot;
using Godot.Collections;
using System;

public partial class DurabilityIndicator : Control
{
	public float maxDurability;
	public float currentDurability;
	public Mod trackedMod;

	[Export] Texture2D durabilityTop;
	[Export] Texture2D durabilityMid;
	[Export] Texture2D durabilityBottom;

	public Array<TextureRect> durabilityUIArray;

	float timePassed = 0;

	public override void _Ready()
	{
		base._Ready();

		durabilityUIArray = new();

		// Mod mod = new LegMod()
		// {
		// 	durability = 2
		// };

		// InitDurability(mod);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// if (timePassed > 1)
		// {
		// 	trackedMod.durability--;
		// 	UpdateDurability();
		// 	timePassed = 0;
		// }

		// timePassed += (float) delta;
	}

	public void InitDurability(Mod mod)
	{
		if (trackedMod != null)
			Unequip(trackedMod);
		
		trackedMod = mod;
		maxDurability = currentDurability = mod.durability;

		TextureRect top = new();
		top.Texture = durabilityTop.Duplicate() as AtlasTexture;
		durabilityUIArray.Add(top);
		AddChild(top);

		for (int i = 1; i < maxDurability - 1; i++)
		{
			TextureRect middle = new();
			middle.Texture = durabilityMid.Duplicate() as AtlasTexture;
			if (i == 1)
				middle.Position = new(top.Position.X, top.Texture.GetHeight());
			else
				middle.Position = new(top.Position.X, durabilityUIArray[i - 1].Position.Y + middle.Texture.GetHeight());

			durabilityUIArray.Add(middle);
			AddChild(middle);
		}

		TextureRect bot = new();
		bot.Texture = durabilityBottom.Duplicate() as AtlasTexture;
		bot.Position = durabilityUIArray.Count > 1 ? durabilityUIArray[^1].Position + new Vector2(0, durabilityMid.GetHeight()) : new(top.Position.X, top.Position.Y + top.Texture.GetHeight());
		durabilityUIArray.Add(bot);
		AddChild(bot);
	}

	public void UpdateDurability()
	{
		if (trackedMod == null)
			return;

		currentDurability = trackedMod.durability;

		int i = 0;
		foreach (TextureRect rect in durabilityUIArray)
		{
			if (i < currentDurability)
			{
				((AtlasTexture)rect.Texture).Region = new(new(0, 0), rect.Texture.GetSize());
			}
			else
			{
				((AtlasTexture)rect.Texture).Region = new(new(rect.Texture.GetWidth(), 0), rect.Texture.GetSize());
			}

			i++;
		}
	}

	public void Unequip(Mod mod)
	{
		if (mod == null || mod != trackedMod)
			return;

		maxDurability = currentDurability = 0;
		trackedMod = null;

		foreach (TextureRect rect in durabilityUIArray)
		{
			RemoveChild(rect);
		}

		durabilityUIArray.Clear();
	}
}
