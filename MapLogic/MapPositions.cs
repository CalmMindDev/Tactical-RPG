using Godot;
using System;

public partial class MapPositions : Node
{
	[Export] private TileMapLayer _groundLayer;

	public Vector2I PixelToTile(Vector2 worldPos)
	{
		return _groundLayer.LocalToMap(worldPos);
	}

	public Vector2 TileToPixel(Vector2I cell)
	{
		return _groundLayer.MapToLocal(cell);
	}
}
