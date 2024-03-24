using Godot;
using Godot.Collections;
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
	[Export] Array<AudioStream> equipFX;
	[Export] Array<AudioStream> moneyFX;
	[Export] AudioStream deathFX;

    public void PlayGameTheme()
	{
		musicPlayer.Stream = gameTheme;
		musicPlayer.VolumeDb *= .8f;
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

	public void FXequip()
	{
		fxPlayer.Stream = equipFX.PickRandom();
		fxPlayer.Play(0);
	}

	public void FXfunnyScrap()
	{
		fxPlayer.Stream = scrapFallFX;
		fxPlayer.Play(0);
	}

	public void FXshop()
	{
		fxPlayer.Stream = moneyFX.PickRandom();
		fxPlayer.Play(0);
	}

	public void FXdeath()
	{
		fxPlayer.Stream = deathFX;
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
		musicPlayer.VolumeDb = fxPlayer.VolumeDb = Mathf.LinearToDb(vol);
		musicPlayer.VolumeDb *= .8f;
	}
}
