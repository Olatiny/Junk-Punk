using Godot;
using Godot.Collections;
using System;

public abstract partial class Mod : Sprite2D
{
	public enum BodyPart { Head, Arm, Leg };
	public enum BuffType { Passive, Attack, Movement, Defense }

	[Export] public BodyPart bodyPart { get; set; }
	[Export] public BuffType buffType { get; set; }
	[Export] public String uid { get; set; }
	[Export] public int durability { get; set; }
	[Export] public int cost { get; set; }
	[Export(PropertyHint.MultilineText)] public String description { get; set; }
	[Export] public Texture2D icon;
	[Export] public Texture2D bigSpriteBlue;
	[Export] public Texture2D bigSpriteRed;
	[Export] public Color bodyPartTint = new(1, 1, 1, 1);

	public PlayerController myPlayer = null;

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

	public virtual void InitSignals()
	{ }

	public virtual void DisconnectSignals()
	{ }

	public virtual Mod Clone()
	{
		Node duplicate = Duplicate();
		duplicate.SetScript(GetScript());
		// GetTree().Root.AddChild(duplicate);

		Mod modDuplicate = (Mod) duplicate;
		modDuplicate.bodyPart = bodyPart;
		modDuplicate.buffType = buffType;
		modDuplicate.uid = uid;
		modDuplicate.durability = durability;
		modDuplicate.description = description;
		modDuplicate.icon = icon;
		modDuplicate.bigSpriteBlue = bigSpriteBlue;

		return modDuplicate;
	}
}
