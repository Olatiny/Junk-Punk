using Godot;
using Godot.Collections;
using System;

public partial class Scrap : Sprite2D
{
	[Export] int maxScrapValue = 100;
	[Export] int maxDurability = 3;
	[Export] PackedScene sparks;

	public PlayerController owner = null;

	bool dropping = false;

	public int armor = 1;
	int currentDurability;
	int currentScrapValue;
	bool dropOnPlayer = false;
	PlayerController playerDroppingOn = null;
	Vector2 startingPosition;
	public Vector2I gridPosition { get; set; }
	ChessBoard board;

	public override void _Ready()
	{
		base._Ready();

		currentDurability = maxDurability;
		currentScrapValue = maxScrapValue;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (dropping)
		{
			Position = Position.MoveToward(startingPosition, (float)delta * 200.0f);

			if (Position.Y >= startingPosition.Y)
			{
				Position = startingPosition;
				dropping = false;

				if (dropOnPlayer)
				{
					playerDroppingOn.TakeDamage(1);
					QueueFree();
				}
			}
		}
	}

	public void Drop(ChessBoard board, Vector2I tileLocation)
	{
		startingPosition = tileLocation * board.tileSize + board.tileOffset;
		gridPosition = tileLocation;
		ZIndex = gridPosition.Y;
		this.board = board;
		if (board.IsTileOccupied(tileLocation))
		{
			Node2D playerNode = board.GetNodeAtTile(tileLocation);
			if (playerNode is PlayerController player)
			{
				dropOnPlayer = true;
				playerDroppingOn = player;
			}
		}
		board.PlaceOnBoard(this, tileLocation);
		Position = new(startingPosition.X, -1 * Texture.GetHeight());
		dropping = true;
	}

	public void Harvest(PlayerController player)
	{
		armor--;
		if (armor > 0)
			return;

		GpuParticles2D sparksEmitter = sparks.Instantiate() as GpuParticles2D;
		sparksEmitter.Position = Position;
		sparksEmitter.ZIndex = ZIndex;
		GetTree().Root.AddChild(sparksEmitter);

		player.currentScrap += currentScrapValue;

		Globals globals = GetNode<Globals>("/root/Globals");
		globals.EmitSignal(Globals.SignalName.CollectedScrap, player, this);

		board.RemoveFromBoard(this);
	}

	public void CheckDurability()
	{
		currentDurability--;
		currentScrapValue -= (maxScrapValue /= maxDurability) * currentDurability;

		if (currentDurability <= 0)
		{
			GpuParticles2D sparksEmitter = sparks.Instantiate() as GpuParticles2D;
			sparksEmitter.Position = Position;
			sparksEmitter.ZIndex = ZIndex;
			GetTree().Root.AddChild(sparksEmitter);

			board.RemoveFromBoard(this);
			return;
		}
	}

	public Scrap Clone()
	{
		Node node = Duplicate();
		node.SetScript(GetScript());

		Scrap scrapNode = node as Scrap;
		scrapNode.maxScrapValue = maxScrapValue;
		scrapNode.maxDurability = maxDurability;
		scrapNode.dropping = dropping;
		scrapNode.currentDurability = maxDurability;
		scrapNode.currentScrapValue = maxScrapValue;
		scrapNode.dropOnPlayer = dropOnPlayer;
		scrapNode.playerDroppingOn = playerDroppingOn;
		scrapNode.startingPosition = startingPosition;
		scrapNode.gridPosition = gridPosition;
		scrapNode.board = board;

		return scrapNode;
	}
}
