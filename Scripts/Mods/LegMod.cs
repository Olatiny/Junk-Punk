using Godot;
using Godot.Collections;
using System;

public partial class LegMod : Mod
{
	// [Export] public bool isPathContinuous = false;
	
	public enum PathProperty
	{
		None = 0, UP = 1, DOWN = 2, LEFT = 4, RIGHT = 8, CONTINUOUS = 16, KEEPLAST = 32
	}

	[Export(PropertyHint.Flags, "Up:1,Down:2,Left:4,Right:8,Continuous:16,Keep Last:32")] public uint pathFlags = 0;
	[Export] public Array<Array<String>> modPaths { get; set; }
	[Export] public Array<Vector2I> modJumps { get; set; }

	public LegMod() : base()
	{ }

	public LegMod(String modUID, int modDurability) : base(modUID, modDurability)
	{
		bodyPart = BodyPart.Leg;
		modPaths = new();
		modJumps = new();
		
		LegMod m = new();
	}

	public bool CheckFlags(PathProperty propertyFlag)
	{
		return (PathProperty) (pathFlags & (uint) propertyFlag) == propertyFlag;
	}

	public override Mod Clone()
	{
		Mod modClone = base.Clone();
		modClone.SetScript(GetScript());

		LegMod legModClone = (LegMod) modClone;
		legModClone.pathFlags = pathFlags;
		legModClone.modPaths = modPaths;
		legModClone.modJumps = modJumps;

		return legModClone;
	}
}
