using Godot;
using System;
using System.Collections.Generic;


public partial class MapObjects : Node {
	public List<Vector2I> General = new List<Vector2I>();
	public List<Vector2I> PlayerTeam = new List<Vector2I>();
	public List<Vector2I> EnemyTeam1 = new List<Vector2I>();
	public List<Vector2I> FriendlyTeam1 = new List<Vector2I>();


	public event EventHandler MapObjectsAsked;
	[Signal] public delegate void PCSelectedEventHandler(Vector2I position);

	public override void _Ready() {
		CallDeferred("SubscribeToCursor");
	}

	public void GetMapObjects() {
		General.Clear();
		PlayerTeam.Clear();
		EnemyTeam1.Clear();
		FriendlyTeam1.Clear();
		MapObjectsAsked.Invoke(this, EventArgs.Empty);
	}

	public void SubscribeToCursor() {
		GD.Print("Subscribed to Cursor");
		var cursor = GetNode<Cursor>("/root/TestScene/Cursor");
		cursor.CursorSelected += OnSelected;
	}
	public void OnSelected(Vector2I position) {
		GD.Print($"On Selected läuft in {position}");
		GetMapObjects();
		if (PlayerTeam.Contains(position))
			EmitSignal(SignalName.PCSelected, position);
		}

}
interface IMapObject
{   
	Vector2I GridPosition {get;}
	Teams Team {get;}
	void SubscribeToCache();
	
	void OnMapObjectsAsked(object cache, EventArgs e) {
		var typedCache = cache as MapObjects;
		typedCache.General.Add(GridPosition);
	}
}
