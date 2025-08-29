using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MapParameters : Node2D
{
	public int Height {get; private set;}
	public int Width {get; private set;}

	public Vector2 TileSize;

	public  void Initialize(TileMapLayer ground) {

		var sizeVect = ground.GetUsedRect().Size;

		Width = sizeVect[0];
		Height = sizeVect[1];

		TileSize = (Vector2)ground.TileSet.TileSize;
	}
}
