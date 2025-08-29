using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MapRangeHighlights : TileMapLayer
{ 
	private Grid _grid;
	private MapObjects _objects;

	public override void _Ready() {
		_grid = GetNode<Grid>("/root/TestScene/Grid");
		_objects = GetNode<MapObjects>("/root/TestScene/MapObjects");
	}


	public void HighlightMovement(Vector2I startingVektor, int movement, MovementMode mode) {
		
		_objects.GetMapObjects();
		
		var startingTile = _grid.IDArray[startingVektor.X, startingVektor.Y]; 

		AStar2D astar = mode switch {
			MovementMode.Infantry => _grid.AStarInfantry,
			MovementMode.Cavalry => _grid.AStarCavalry,
			MovementMode.Rogue  => _grid.AStarRogue
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
					if (!_objects.General.Contains(coord))
						SetCell(coord, 0, new Vector2I(0,0), 0);
				}
			}
		}
	}
	public  void HighlightAttack() {

		var usedCellsGD = GetUsedCells();
		Vector2I[] usedCells = usedCellsGD.Cast<Vector2I>().ToArray();

		var emptyNeighbours = 
				(from cell in usedCells
				let surrounding = GetSurroundingCells(cell)
									.Where(v => !usedCells.Contains(v))
									.Where(v => _grid.Map.Width > v.X && v.X >= 0 && _grid.Map.Height > v.Y && v.Y >= 0)            
									.ToArray()
				where surrounding.Length > 0
				select new {cell, surrounding})
				.ToDictionary(k => k.cell, v => v.surrounding);

		//Hashset.Contains() ist effizienter als Array.Contains()
		var points = new HashSet<long>(_grid.AStarInfantry.GetPointIds());

		foreach (var kvp in emptyNeighbours) {
			foreach (var targetV in kvp.Value){
				var startV = kvp.Key;
				if(!points.Contains(_grid.IDArray[targetV.X, targetV.Y]))
					continue;
				if (!_grid.AStarInfantry.ArePointsConnected(_grid.IDArray[startV.X, startV.Y], _grid.IDArray[targetV.X, targetV.Y], false))
					continue;
				if (GetCellTileData(targetV) == null)
					SetCell(targetV, 0, new Vector2I(1,0), 0);
			}

		}

	}
}
