using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Grid : Node
{
	public int Height {get; private set;}
	public int Width {get; private set;}

	[Export] public TileMapLayer GroundLayer;
	[Export] public TileMapLayer PassabilityLayer;
	[Export] public TileMapLayer RangeLayer;

	public MapObjectCache MapObjectCache = new MapObjectCache();

	public AStar2D AStarInfantry = new AStar2D();
	public AStar2D AStarCavalry = new AStar2D();
	public AStar2D AStarRogue = new AStar2D();
	private AStar2D AStarFree = new AStar2D();
	public 	int[,] IDArray;

	public override void _Ready()
	{   

		

		var sizeVect = GroundLayer.GetUsedRect().Size;
		Width = sizeVect[0];
		Height = sizeVect[1];

		//Cursor 
		var tileSize = (Vector2)GroundLayer.TileSet.TileSize;

		var refRect = new ReferenceRect();
		refRect.Size = tileSize;
		refRect.BorderWidth = 3;
		refRect.Modulate = Colors.Black;
		refRect.SetEditorOnly(false);
		refRect.ZIndex = 20;
		AddChild(refRect);

		var cursorInstance = new Cursor();
		cursorInstance.Initialize(Width, Height, (int)tileSize.X);
		refRect.AddChild(cursorInstance);


		//Astar Punkte
		IDArray = new int[Width, Height];

		var id = 0;
		for (int y = 0; y < Height; y++)      
		for (int x = 0; x < Width; x++){
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
		
		
		CallDeferred("HighlightMovement", new Vector2I(6, 1), 6, (int)MovementMode.Infantry	);
		CallDeferred("HighlightAttack");

	}

	private void ConnectPoints(AStar2D astar) {
		
		HashSet<long> points = new HashSet<long>(astar.GetPointIds());  
		
		for (int y = 0; y < Height; y++)      
		for (int x = 0; x < Width; x++){

			if (!points.Contains(IDArray[x,y ]))
				continue;
			
			var pointVect = new Vector2I(x, y);
			var pointPassabilityTile = PassabilityLayer.GetCellTileData(pointVect);

			foreach (var dir in Directions.Four) {
				var neighbourVect = pointVect + dir;

				if (neighbourVect.X < 0 || neighbourVect.X >= Width || neighbourVect.Y < 0 || neighbourVect.Y >= Height)
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


	public void HighlightMovement(Vector2I startingVektor, int movement, MovementMode mode) {
		MapObjectCache.GetMapObjects();
		
		var startingTile = IDArray[startingVektor.X, startingVektor.Y]; 

		AStar2D astar = mode switch {
			MovementMode.Infantry => AStarInfantry,
			MovementMode.Cavalry => AStarCavalry,
			MovementMode.Rogue  => AStarRogue
		};

		foreach (var tileId in astar.GetPointIds()) {
			var path = astar.GetIdPath(startingTile, tileId).ToList();
			path.RemoveAt(0);

			if (path.Count <= movement) {
				float pathWeight = 0;
				foreach (var id in path)
					pathWeight += astar.GetPointWeightScale(id);
				if (pathWeight < movement) {
					var coord = (Vector2I)astar.GetPointPosition(tileId);
					if (!MapObjectCache.General.Contains(coord))
						RangeLayer.SetCell(coord, 0, new Vector2I(0,0), 0);
				}
			}
		}
	}
	public  void HighlightAttack() {

		var usedCellsGD = RangeLayer.GetUsedCells();
		Vector2I[] usedCells = usedCellsGD.Cast<Vector2I>().ToArray();

		var emptyNeighbours = 
				(from cell in usedCells
				let surrounding = RangeLayer.GetSurroundingCells(cell)
											.Where(v => !usedCells.Contains(v))
											.Where(v => Width > v.X && v.X >= 0 && Height > v.Y && v.Y >= 0)            
											.ToArray()
				where surrounding.Length > 0
				select new {cell, surrounding})
				.ToDictionary(k => k.cell, v => v.surrounding);

		//Hashset.Contains() ist effizienter als Array.Contains()
		var points = new HashSet<long>(AStarInfantry.GetPointIds());

		foreach (var kvp in emptyNeighbours) {
			foreach (var targetV in kvp.Value){
				var startV = kvp.Key;
				if(!points.Contains(IDArray[targetV.X, targetV.Y]))
					continue;
				if (!AStarInfantry.ArePointsConnected(IDArray[startV.X, startV.Y], IDArray[targetV.X, targetV.Y], false))
					continue;
				if (RangeLayer.GetCellTileData(targetV) == null)
					RangeLayer.SetCell(targetV, 0, new Vector2I(1,0), 0);
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
