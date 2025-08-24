using Godot;
using System;

public partial class Cursor : Node
{
	public int MapHeight { get; private set; }
	public int MapWidth { get; private set; }
	private int _tileSize { get; set; }
	public Vector2I Position { get; private set; }

	public void Initialize(int width, int height, int tileSize)
	{
		MapWidth = width - 1;
		MapHeight = height - 1;
		_tileSize = tileSize;
		Position = new Vector2I(0, 0);
	}

	public override void _Process(double delta)
	{
		Vector2I move = new Vector2I(0, 0);

		if (Input.IsActionJustPressed("ui_right")) move.X = 1;
		else if (Input.IsActionJustPressed("ui_left")) move.X = -1;
		if (Input.IsActionJustPressed("ui_down")) move.Y = 1;
		else if (Input.IsActionJustPressed("ui_up")) move.Y = -1;

		var newPos = Position + move;
		newPos.X = Mathf.Clamp(newPos.X, 0, MapWidth);
		newPos.Y = Mathf.Clamp(newPos.Y, 0, MapHeight);

		if (newPos != Position)
		{
			Position = newPos;
			(GetParent() as Control).Position = Position * _tileSize;
		}
	}
}
