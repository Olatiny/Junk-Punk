using Godot;
using Godot.Collections;
using System;

public partial class ScoreBoard : TextureRect
{
	[Export] TextureRect turnIndicator;
	[Export] TextureRect leftOnes;
	[Export] TextureRect leftTens;
	[Export] TextureRect rightOnes;
	[Export] TextureRect rightTens;
	[Export] ScrapCounter leftScrapCounter;
	[Export] ScrapCounter rightScrapCounter;

	public void UpdateScoreBoard(int currentPlayerIdx, Array<PlayerController> players)
	{
		if (leftOnes == null || rightOnes == null || players == null || turnIndicator == null)
			return;

		turnIndicator.FlipH = currentPlayerIdx == 0;

		int player1hp = players[0].health;
		int player2hp = players[1].health;

		int leftOnesPlace = player1hp % 10;
		int leftTensPlace = player1hp / 10;

		int rightOnesPlace = player2hp % 10;
		int rightTensPlace = player2hp / 10;

		((AtlasTexture)leftOnes.Texture).Region = new Rect2(leftOnesPlace * 15, 0, 15, 16);
		((AtlasTexture)leftTens.Texture).Region = new Rect2(leftTensPlace * 15, 0, 15, 16);

		((AtlasTexture)rightOnes.Texture).Region = new Rect2(rightOnesPlace * 15, 0, 15, 16);
		((AtlasTexture)rightTens.Texture).Region = new Rect2(rightTensPlace * 15, 0, 15, 16);

		leftScrapCounter.UpdateScrapCount(players[0]);
		rightScrapCounter.UpdateScrapCount(players[1]);
	}
}
