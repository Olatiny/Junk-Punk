using Godot;
using System;
using System.ComponentModel;

public partial class DurabilityIndicatorGroup : Control
{
	[Export] DurabilityIndicator head;
	[Export] DurabilityIndicator leftArm;
	[Export] DurabilityIndicator rightArm;
	[Export] DurabilityIndicator leftLeg;
	[Export] DurabilityIndicator rightLeg;

	public void InitModDurability(Mod mod, Mod.BodyPart bodyPart, bool isRight = false)
	{
		switch (bodyPart)
		{
			case Mod.BodyPart.Head:
				head.InitDurability(mod);

				break;
			case Mod.BodyPart.Arm:
				if (isRight)
					rightArm.InitDurability(mod);
				else
					leftArm.InitDurability(mod);

				break;
			case Mod.BodyPart.Leg:
				if (isRight)
					rightLeg.InitDurability(mod);
				else
					leftLeg.InitDurability(mod);

				break;
		}
	}

	public void UpdateDurability(Mod.BodyPart bodyPart, bool isRight = false)
	{
		switch (bodyPart)
		{
			case Mod.BodyPart.Head:
				head.UpdateDurability();

				break;
			case Mod.BodyPart.Arm:
				if (isRight)
					rightArm.UpdateDurability();
				else
					leftArm.UpdateDurability();

				break;
			case Mod.BodyPart.Leg:
				if (isRight)
					rightLeg.UpdateDurability();
				else
					leftLeg.UpdateDurability();

				break;
		}

	}

	public void Unequip(Mod mod, Mod.BodyPart bodyPart, bool isRight = false)
	{
		switch (bodyPart)
		{
			case Mod.BodyPart.Head:
				head.Unequip(mod);

				break;
			case Mod.BodyPart.Arm:
				if (isRight)
					rightArm.Unequip(mod);
				else
					leftArm.Unequip(mod);

				break;
			case Mod.BodyPart.Leg:
				if (isRight)
					rightLeg.Unequip(mod);
				else
					leftLeg.Unequip(mod);

				break;
		}
	}
}
