using Godot;
using System;

public partial class Cursor : ReferenceRect
{
	public int MapHeight { get; private set; }
	public int MapWidth { get; private set; }
	public Vector2I GridPosition { get; private set; }
	private int _tileSize;

	enum CursorState {
		Normal, 
		CharSelected
	}

	private CursorState _state = CursorState.Normal;

	[Signal] public delegate void CursorSelectedEventHandler(Vector2I position);
	[Signal] public delegate void CursorDeselectedEventHandler();

	public override void _Ready()
	{
		var map = GetNode<TileGrid>("%Map"); 
		
		MapWidth = map.Width;
		MapHeight = map.Height;
		Size = map.TileSize;
		_tileSize = (int)map.TileSize[0];
		GridPosition = new Vector2I(0, 0);

		CallDeferred("SubscribeToMapObjects");
	}

	public override void _Process(double delta)
	{
		Vector2I move = new Vector2I(0, 0);

		if (Input.IsActionJustPressed("ui_right")) move.X = 1;
		else if (Input.IsActionJustPressed("ui_left")) move.X = -1;
		if (Input.IsActionJustPressed("ui_down")) move.Y = 1;
		else if (Input.IsActionJustPressed("ui_up")) move.Y = -1;

		var newPos = GridPosition + move;
		newPos.X = Mathf.Clamp(newPos.X, 0, MapWidth);
		newPos.Y = Mathf.Clamp(newPos.Y, 0, MapHeight);

		if (newPos != GridPosition){
			GridPosition = newPos;
			this.Position = GridPosition * _tileSize;
		}
		
		switch (_state) { 
		case CursorState.Normal: 
			if (Input.IsActionJustPressed("ui_accept")){
				EmitSignal(SignalName.CursorSelected, GridPosition);
			}
			break;
		case CursorState.CharSelected: 
			if (Input.IsActionJustPressed("ui_cancel")){
				EmitSignal(SignalName.CursorDeselected);
				_state = CursorState.Normal;
			}
			break;
		}
	}

	private void SubscribeToMapObjects(){
		var mapObjects = GetNode<MapObjects>("%MapObjects");
		mapObjects.CharSelected += OnCharSelected;
	}

	public void OnCharSelected(CharacterSelectionInfo charDTO, Godot.Collections.Array<Vector2I> unpassable, Godot.Collections.Array<Vector2I> unattackable, bool pc) {
		_state = CursorState.CharSelected;
	}


}
