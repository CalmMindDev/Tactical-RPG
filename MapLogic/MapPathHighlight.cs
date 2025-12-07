using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MapPathHighlight : TileMapLayer
{ 
	private Cursor _cursor;
	private TerrainPassability _passability;

	private int _startingID;
	private Vector2I _startingTile;


	private AStar2D _astar;
	private HashSet<Vector2I> _blockedTiles;
	private int _movementRange;

	public void SetParameters(Vector2I position, MovementMode mode, int range, Godot.Collections.Array<Vector2I> unpassable) {
		_startingTile = position;
		_startingID = _passability.IDArray[position.X, position.Y]; 

		switch (mode) {
			case MovementMode.Infantry:
				_astar = _passability.AStarInfantry;
				break;
			case MovementMode.Cavalry:
				_astar = _passability.AStarCavalry;
				break;
			case MovementMode.Rogue:
				_astar = _passability.AStarRogue;
				break;
			case MovementMode.None:
				return;
			default:
				return;
		}
		_movementRange = range;

		_blockedTiles = new HashSet<Vector2I>(unpassable.Cast<Vector2I>());
		GD.Print($"SetParameters called with mode={mode}, position={position}, range={range}");
		GD.Print($"_passability is null? {_passability == null}");
	}

	public override void _Ready() {
		SetProcess(false);
		_cursor = GetNode<Cursor>("%Cursor");
		_passability = GetParent().GetNode<TerrainPassability>("Passability");
	}

	public override void _Process(double delta) {
		var targetID = _passability.IDArray[_cursor.GridPosition.X, _cursor.GridPosition.Y];
		if (!_astar.HasPoint(targetID))
			return;

		var idArray = _astar.GetIdPath(_startingID, targetID);
		var idPath = new List<long>(idArray);
		idPath.RemoveAt(0);
		List<Vector2I> vectPath = new List<Vector2I>();

		foreach (var id in idPath)
		{
			var pos = (Vector2)_astar.GetPointPosition(id); 
			vectPath.Add((Vector2I)pos); 
		}
		
		float pathWeight = 0;
		foreach (var id in idPath)
			pathWeight += _astar.GetPointWeightScale(id);
		
		if (pathWeight > _movementRange) 
			return;
		
		if (vectPath.Cast<Vector2I>().Any(p => _blockedTiles.Contains(p)))
			return;
		
		Clear();
		var gdPathVect = new Godot.Collections.Array<Vector2I>(vectPath);
		if (vectPath.Count > 0) {
			var pathDirection = vectPath[0] - _startingTile;
			switch (pathDirection) {
				case (1, 0):
					SetCell(_startingTile, 0, new Vector2I(0, 2));
					break; 
				case (-1, 0):
					SetCell(_startingTile, 0, new Vector2I(1, 2));
					break; 
				case (0, 1):
					SetCell(_startingTile, 0, new Vector2I(0, 0));
					break; 
				case (0, -1):
					SetCell(_startingTile, 0, new Vector2I(0, 1));
					break; 
			}
		}
		SetCellsTerrainPath(gdPathVect, 0, 0, false);
	}
}
