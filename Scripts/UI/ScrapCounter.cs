using Godot;
using System;

public partial class ScrapCounter : TextureRect
{
	[Export] TextureRect hundreds;
	[Export] TextureRect tens;
	[Export] TextureRect ones;

	public void UpdateScrapCount(PlayerController player)
	{
		if (hundreds == null || tens == null || ones == null)
			return;

		int playerScrap = player.currentScrap;

		int onesPlace = playerScrap % 10;
		playerScrap /= 10;

		int tensPlace = playerScrap % 10;
		int hundredsPlace = playerScrap / 10;

		((AtlasTexture)ones.Texture).Region = new Rect2(onesPlace * 4, 0, 4, 6);
		((AtlasTexture)tens.Texture).Region = new Rect2(tensPlace * 4, 0, 4, 6);
		((AtlasTexture)hundreds.Texture).Region = new Rect2(hundredsPlace * 4, 0, 4, 6);
	}
}
