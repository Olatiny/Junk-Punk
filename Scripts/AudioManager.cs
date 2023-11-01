using Godot;
using System;

public partial class AudioManager : Node2D
{
	[ExportSubgroup("Players")]
	[Export] AudioStreamPlayer musicPlayer;
	[Export] AudioStreamPlayer fxPlayer;

	[ExportSubgroup("Music")]
	[Export] AudioStream gameTheme;
	[Export] AudioStream mainMenuTheme;
	[Export] AudioStream victoryTheme;

	[ExportSubgroup("Sound Effects")]
	[Export] AudioStream doorFX;
	[Export] AudioStream damageFX;
	[Export] AudioStream scrapFallFX;
	[Export] AudioStream equipFX;
	[Export] AudioStream moneyFX;
	[Export] AudioStream deathFX;

    public void PlayGameTheme()
	{
		musicPlayer.Stream = gameTheme;
		musicPlayer.Play();
	}

	public void PlayMenuTheme()
	{
		musicPlayer.Stream = mainMenuTheme;
		musicPlayer.Play();
	}

	public void PlayVictoryTheme()
	{
		musicPlayer.Stream = victoryTheme;
		musicPlayer.Play();
	}

	public void FXdoor()
	{
		fxPlayer.Stream = doorFX;
		fxPlayer.Play(0);
	}

	public void FXdamage()
	{
		fxPlayer.Stream = damageFX;
		fxPlayer.Play(0);
	}

	public void Play()
	{
		musicPlayer.Play(0);
		fxPlayer.Play(0);
	}

	public void Pause()
	{
		musicPlayer.Playing = fxPlayer.Playing = false;
	}

	public void Unpause()
	{
		musicPlayer.Playing = fxPlayer.Playing = true;
	}

	public void SetVolume(float vol)
	{
		musicPlayer.VolumeDb = fxPlayer.VolumeDb = vol;
	}
}
