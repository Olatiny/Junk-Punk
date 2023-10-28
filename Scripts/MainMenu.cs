using Godot;
using System;

public partial class MainMenu : Control
{
	[ExportSubgroup("Main Menus")]
	[Export] Control mainMenu;
	[Export] Control creditsMenu;
	[Export] Control settingsMenu;

	public void StartGame()
	{
		GetTree().ChangeSceneToFile("Scenes/InventoryShop.tscn");
	}

	public void OpenSettings()
	{
		settingsMenu.Visible = true;
		mainMenu.Visible = creditsMenu.Visible = false;
	}

	public void ChangeVolume(float vol)
	{
		GetNode<Globals>("/root/Globals").masterVolume = vol;
	}

	public void OpenCredits()
	{
		creditsMenu.Visible = true;
		mainMenu.Visible = settingsMenu.Visible = false;
	}

	public void BackToMain()
	{
		mainMenu.Visible = true;
		creditsMenu.Visible = settingsMenu.Visible = false;
	}

	public void Quit()
	{
		GetTree().Quit();
	}
}
