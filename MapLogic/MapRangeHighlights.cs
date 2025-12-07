using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MapRangeHighlights : TileMapLayer
{ 
	private TerrainPassability _passability;

	public override void _Ready() {
		_passability = GetParent().GetNode<TerrainPassability>("Passability");
	}
	public void HighlightMovement(Vector2I position, MovementMode mode, int range, Godot.Collections.Array<Vector2I> objects) {
		
		var startingTile = _passability.IDArray[position.X, position.Y]; 

		AStar2D astar;
		switch (mode) {
			case MovementMode.Infantry:
				astar = _passability.AStarInfantry;
				break;
			case MovementMode.Cavalry:
				astar = _passability.AStarCavalry;
				break;
			case MovementMode.Rogue:
				astar = _passability.AStarRogue;
				break;
			case MovementMode.None:
				return;
			default:
				return;
		}

		foreach (var tileId in astar.GetPointIds()) {
			var path = astar.GetIdPath(startingTile, tileId).ToList();
			path.RemoveAt(0);

			if (path.Count <= range) {
				float pathWeight = 0;
				foreach (var id in path)
					pathWeight += astar.GetPointWeightScale(id);
				if (pathWeight < range) {
					var coord = (Vector2I)astar.GetPointPosition(tileId);
					if (!objects.Contains(coord))
						SetCell(coord, 0, new Vector2I(0,0), 0);
				}
			}
		}
	}
	public void HighlightCloseAttack(Vector2I starting, Godot.Collections.Array<Vector2I> unattackable) {

		var usedCellsGD = GetUsedCells();

		var usedCells = usedCellsGD.Count == 0
			? new HashSet<Vector2I> { starting }
			: new HashSet<Vector2I>(usedCellsGD.Cast<Vector2I>());

		var emptyNeighbours = 
				(from cell in usedCells
				let surrounding = GetSurroundingCells(cell)
									.Where(v => !usedCells.Contains(v))
									.Where(v => ((TileGrid)GetParent()).Width > v.X && v.X >= 0 && ((TileGrid)GetParent()).Height > v.Y && v.Y >= 0)            
									.ToArray()
				where surrounding.Length > 0
				select new {cell, surrounding})
				.ToDictionary(k => k.cell, v => v.surrounding);

		//Hashset.Contains() ist effizienter als Array.Contains()
		var points = new HashSet<long>(_passability.AStarInfantry.GetPointIds());

		foreach (var kvp in emptyNeighbours) {
			foreach (var targetV in kvp.Value){
				var startV = kvp.Key;
				if(!points.Contains(_passability.IDArray[targetV.X, targetV.Y]))
					continue;
				if (!_passability.AStarInfantry.ArePointsConnected(_passability.IDArray[startV.X, startV.Y], _passability.IDArray[targetV.X, targetV.Y], false))
					continue;
				if (GetCellTileData(targetV) == null)
					SetCell(targetV, 0, new Vector2I(1,0), 0);
			}

		}

	}
	public void HighlightRangedAttack(Vector2I position, int min, int max, Godot.Collections.Array<Vector2I> unattackable) {}
}
