using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Grid : Node
{
	public int Height {get; private set;}
	public int Width {get; private set;}

	public MapParameters Map;
	[Export] public TileMapLayer GroundLayer;
	[Export] public TileMapLayer PassabilityLayer;
	[Export] public TileMapLayer RangeLayer;

	public AStar2D AStarInfantry = new AStar2D();
	public AStar2D AStarCavalry = new AStar2D();
	public AStar2D AStarRogue = new AStar2D();
	private AStar2D AStarFree = new AStar2D();
	public 	int[,] IDArray;

	public override void _EnterTree()
	{   

		Map = GetNode<MapParameters>("/root/TestScene/Map"); 
		Map.Initialize(GroundLayer);

		//Astar Punkte
		IDArray = new int[Map.Width, Map.Height];

		var id = 0;
		for (int y = 0; y < Map.Height; y++)      
		for (int x = 0; x < Map.Width; x++){
			var vect = new Vector2I(x, y);
			AStarFree.AddPoint(id, vect, 1f);
			IDArray[x, y] = id;
			if (PassabilityLayer.GetCellTileData(vect) != null && PassabilityLayer.GetCellTileData(vect).GetCustomData("Unpassable").As<bool>()){
				id++;
				continue;
			 }
		
			var td = GroundLayer.GetCellTileData(vect);

			AStarCavalry.AddPoint(id, vect, td.GetCustomData("Cavalry").As<float>());
			
			var infantryWeight = td.GetCustomData("Infantry").As<float>();
			AStarInfantry.AddPoint(id, vect, infantryWeight);
			AStarFree.AddPoint(id, vect, 1f);
			
			if (infantryWeight < 1f)
				AStarRogue.AddPoint(id, vect, infantryWeight);
			else 
				AStarRogue.AddPoint(id, vect, 1f);
			
			id++;
		}

		//Verbindungen
		ConnectPoints(AStarCavalry);
		ConnectPoints(AStarInfantry);
		ConnectPoints(AStarRogue);

	}

	private void ConnectPoints(AStar2D astar) {
		
		HashSet<long> points = new HashSet<long>(astar.GetPointIds());  
		
		for (int y = 0; y < Map.Height; y++)      
		for (int x = 0; x < Map.Width; x++){

			if (!points.Contains(IDArray[x,y ]))
				continue;
			
			var pointVect = new Vector2I(x, y);
			var pointPassabilityTile = PassabilityLayer.GetCellTileData(pointVect);

			foreach (var dir in Directions.Four) {
				var neighbourVect = pointVect + dir;

				if (neighbourVect.X < 0 || neighbourVect.X >= Map.Width || neighbourVect.Y < 0 || neighbourVect.Y >= Map.Height)
					continue;
				
				if (!astar.HasPoint(IDArray[neighbourVect.X, neighbourVect.Y]))
					continue;
				
				if (pointPassabilityTile == null) {
					astar.ConnectPoints(IDArray[x, y], IDArray[neighbourVect.X, neighbourVect.Y], false);
					continue; 
				}
				
				string tileVariable = dir switch {
					{ X: 1, Y: 0 }  => "RightBlocked",
					{ X: -1, Y: 0 } => "LeftBlocked",
					{ X: 0, Y: 1 }  => "DownBlocked",
					{ X: 0, Y: -1 } => "UpBlocked",
					_ => null
				};
				
				if (pointPassabilityTile.GetCustomData(tileVariable).As<bool>())
					continue;
				
				astar.ConnectPoints(IDArray[x, y], IDArray[neighbourVect.X, neighbourVect.Y], false);
			}
		}


	}



}
public static class Directions
{
	public static readonly Vector2I[] Four = new Vector2I[]
	{
		new Vector2I(1, 0),
		new Vector2I(-1, 0),
		new Vector2I(0, 1),
		new Vector2I(0, -1)
	};

	public static readonly Vector2I[] Eight = new Vector2I[]
	{
		new Vector2I(1, 0),
		new Vector2I(-1, 0),
		new Vector2I(0, 1),
		new Vector2I(0, -1),
		new Vector2I(1, 1),
		new Vector2I(-1, 1),
		new Vector2I(1, -1),
		new Vector2I(-1, -1)
	};
}
