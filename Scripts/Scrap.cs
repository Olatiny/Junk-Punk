using Godot;
using Godot.Collections;
using System;

public partial class Scrap : Sprite2D
{
	[Export] public int maxScrapValue = 100;
	[Export] int maxDurability = 3;
	[Export] PackedScene sparks;

	public PlayerController owner = null;

	bool dropping = false;
	bool funnyScrap = false;

	public int armor = 1;
	int currentDurability;
	public int currentScrapValue;
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

				if (funnyScrap)
				{
					GetNode<AudioManager>("/root/AudioManager").FXfunnyScrap();

					GpuParticles2D sparksEmitter = sparks.Instantiate() as GpuParticles2D;
					sparksEmitter.Position = Position;
					sparksEmitter.ZIndex = ZIndex;
					sparksEmitter.Modulate = new(1, 1, 0);
					GetTree().Root.AddChild(sparksEmitter);

					QueueFree();
				}
			}
		}
	}

	public void Drop(ChessBoard board, Vector2I tileLocation)
	{
		RandomNumberGenerator rand = new();
		int randi = rand.RandiRange(1, 100);

		if (randi == 100)
		{
			Modulate = new(1, 1, 0.8f);
			funnyScrap = true;
		}

		startingPosition = tileLocation * board.tileSize + board.tileOffset;
		gridPosition = tileLocation;
		ZIndex = gridPosition.Y + 1;
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
		if (!funnyScrap)
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
		currentScrapValue -= 10;
		if (currentScrapValue <= 0)
			currentScrapValue = 0;
		
		Modulate -= new Color(.1f, .1f, .1f, 0);

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
