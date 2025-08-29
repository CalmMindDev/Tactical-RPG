using Godot;
using System;

public partial class Cursor : ReferenceRect
{
	public int MapHeight { get; private set; }
	public int MapWidth { get; private set; }
	public Vector2I GridPosition { get; private set; }
	private int _tileSize;

	[Signal]
	public delegate void CursorSelectedEventHandler(Vector2I position);

	public override void _Ready()
	{
		var map = GetNode<MapParameters>("/root/TestScene/Map"); 
		
		MapWidth = map.Width;
		MapHeight = map.Height;
		Size = map.TileSize;
		_tileSize = (int)map.TileSize[0];
		GridPosition = new Vector2I(0, 0);
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

		if (newPos != Position)
		{
			GridPosition = newPos;
			this.Position = GridPosition * _tileSize;
		}
		
		if (Input.IsActionJustPressed("ui_accept"))
{
			EmitSignal(SignalName.CursorSelected, GridPosition);
			GD.Print("Cursor Signal Emitted");
}
		}
}
