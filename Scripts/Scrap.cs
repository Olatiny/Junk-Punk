using Godot;
using Godot.Collections;
using System;

public partial class Scrap : Sprite2D
{
    [Export] int maxScrapValue = 100;
    [Export] int maxDurability = 3;

    bool dropping = false;

    int currentDurability = 3;
    int currentScrapValue = 100;
    bool dropOnPlayer = false;
    PlayerController playerDroppingOn = null;
    Vector2 startingPosition;
    Vector2I gridPosition;
    ChessBoard board;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (dropping)
        {
            Position.MoveToward(startingPosition, (float)delta * 20.0f);

            if (Position.Y <= startingPosition.Y)
            {
                Position = startingPosition;
                dropping = false;

                if (dropOnPlayer)
                {
                    playerDroppingOn.TakeDamage(1);
                    board.RemoveFromBoard(this, gridPosition);
                }
            }
        }
    }

    public void Drop(ChessBoard board, Vector2I tileLocation)
    {
        startingPosition = tileLocation * board.tileSize + board.tileOffset;
        gridPosition = tileLocation;
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
        Position = new(Position.X, Position.Y - 500);
        dropping = true;
    }

    public void Harvest(PlayerController player)
    {
        player.currentScrap += currentScrapValue;
        board.RemoveFromBoard(this, gridPosition);
    }

    public void CheckDurability()
    {
        currentDurability--;
        currentScrapValue -= (maxScrapValue /= maxDurability) * currentDurability;

        if (currentDurability <= 0)
        {
            board.RemoveFromBoard(this, gridPosition);
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
