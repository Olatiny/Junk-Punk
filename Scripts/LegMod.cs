using Godot;
using Godot.Collections;
using System;

public partial class LegMod : Mod
{
	[Export] public bool isPathContinuous = false;
	[Export(PropertyHint.Flags, "Up:1,Down:2,Left:4,Right:8")] public uint pathDirections = 0;
	[Export] public Array<Array<String>> modPaths { get; set; }
	[Export] public Array<Vector2I> modJumps { get; set; }

	public LegMod() : base() 
	{}

	public LegMod(String modUID) : base(modUID)
	{
		type = Type.Leg;
		modPaths = new();
		modJumps = new();

		LegMod m = new();
	}

	public void AddPath(Array<String> pathStrings)
	{
		modPaths.Add(pathStrings);
	}

	public void AddJump(Vector2I jumpPos)
	{
		modJumps.Add(jumpPos);
	}

	public void AddJumps(params Vector2I[] jumps)
	{
		foreach (Vector2I jump in jumps)
		{
			modJumps.Add(jump);
		}
	}
}
