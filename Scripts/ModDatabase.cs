using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class ModController : Node
{
	[ExportCategory("Mod Information")]
	[Export] public char[] modName = "";
	[Export] public int modAttack = 0;
	[Export] public int modHealth = 0;
	[Export] public char[] description = "";
	[Export] public Array<Array<String>> modMovement;
	
	public override mod get_mod(char[] modName) {
		if(mod.modName == modName) {
			return mod;	
		}
	}
}
