using Godot;
using System;

public partial class VictoryScreen : Control
{
	[Export] TextureRect victoryLabel;
	[Export] TextureRect victoryBackground;
    // [Export] AnimationPlayer animator;
	int victoryIndex = -1;

    public override void _Ready()
    {
        base._Ready();
   
		victoryIndex = GetNode<Globals>("/root/Globals").victoryIndex;

        if (victoryIndex < 0)
            victoryIndex = 0;

        ((AnimatedTexture)victoryLabel.Texture).CurrentFrame = victoryIndex;
        ((AnimatedTexture)victoryBackground.Texture).CurrentFrame = victoryIndex;

        // animator.Play("Fade In");
    }

    public void StartOver()
    {
        GetTree().ChangeSceneToFile("Scenes/InventoryShop.tscn");
    }

    public void ReturnToMain()
    {
        GetTree().ChangeSceneToFile("Scenes/MainMenu.tscn");
    }
}
