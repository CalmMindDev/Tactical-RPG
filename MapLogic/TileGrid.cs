using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class TileGrid : Node2D
{
	public int Height {get; private set;}
	public int Width {get; private set;}

	public Vector2 TileSize;

	[Export] public TileMapLayer GroundLayer;

	private TerrainPassability _passability;
	private MapRangeHighlights _range;
	private MapPathHighlight _path;

	public override void _EnterTree() {

		var sizeVect = GroundLayer.GetUsedRect().Size;

		Width = sizeVect[0];
		Height = sizeVect[1];

		TileSize = (Vector2)GroundLayer.TileSet.TileSize;

		_passability = GetNode<TerrainPassability>("Passability");
		_range = GetNode<MapRangeHighlights>("RangeHighlight");
		_path = GetNode<MapPathHighlight>("PathHighlight");

		CallDeferred("Subscribe");
	}

	private void Subscribe() {
		var cursor = GetNode<Cursor>("%Cursor");
		cursor.CursorDeselected += OnDeselected;
		
		var mapObjects = GetNode<MapObjects>("%MapObjects");
		mapObjects.CharSelected += OnCharSelected;
	}

	public void OnCharSelected(CharacterSelectionInfo charDTO, Godot.Collections.Array<Vector2I> unpassable, Godot.Collections.Array<Vector2I> unattackable, bool pc) {

		_range.HighlightMovement(charDTO.TilePosition, charDTO.MovementMode, charDTO.MovementRange, unpassable);

		if (charDTO.AttackMode == AttackMode.CloseCombat || charDTO.AttackMode == AttackMode.Mixed)
			_range.HighlightCloseAttack(charDTO.TilePosition, unattackable);

		if (charDTO.AttackMode == AttackMode.Ranged || charDTO.AttackMode == AttackMode.Mixed)
			_range.HighlightRangedAttack(charDTO.TilePosition, charDTO.MinimumAttackRange, charDTO.MaximumAttackRange, unattackable);

		if (pc){
			_path.SetParameters(charDTO.TilePosition, charDTO.MovementMode, charDTO.MovementRange, unpassable);
			_path.SetProcess(true);
		}
	}

	public void OnDeselected() {
		_range.Clear();
		_path.Clear();
		_path.SetProcess(false);
	}

	public Vector2I GetGridPosition(Vector2 globalPosition) {
		return GroundLayer.LocalToMap(globalPosition);
	}

	public Vector2 GetGlobalPosition(Vector2I gridPosition) {
		return GroundLayer.MapToLocal(gridPosition);
	}

}
