using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class MapObjects : Node {
	public Dictionary<Vector2I, Teams> ObjectCache = new Dictionary<Vector2I, Teams>();

	public CharacterSelectionInfo CharDTO;

	[Signal] public delegate void CharSelectedEventHandler(CharacterSelectionInfo charDTO, Godot.Collections.Array<Vector2I> unpassable, Godot.Collections.Array<Vector2I> unattackable, bool pc);

	public override void _Ready() {
		CallDeferred("SubscribeToCursor");
	}

	public void GetMapObjects() {
		ObjectCache.Clear();
		GetTree().CallGroup("mapObjects", "RegisterPosition");
	}

	public void SubscribeToCursor() {
		var cursor = GetNode<Cursor>("%Cursor");
		cursor.CursorSelected += OnSelected;
	}
	public void OnSelected(Vector2I position) {
		GetMapObjects();
		if (ObjectCache.ContainsKey(position) && ObjectCache[position] != Teams.Environment) {
			GetTree().CallGroup("characters", "GetMapDTO", position);
			Godot.Collections.Array<Vector2I> unpassable = new Godot.Collections.Array<Vector2I>(ObjectCache.Keys);
			var teamMates = ObjectCache
				.Where(kvp => kvp.Value == CharDTO.Team) 
				.Select(kvp => kvp.Key)                 
				.ToArray();  
			var unattackable = new Godot.Collections.Array<Vector2I>(teamMates); 
			bool pc = false;
			if (CharDTO.Team == Teams.Player)
				pc = true;
			EmitSignal(nameof(CharSelected), CharDTO, unpassable, unattackable, pc);
			CharDTO = null;
		}
	}
}
interface IMapObject
{   
	Vector2I GridPosition {get;}
	Teams Team {get;}
	void RegisterPosition();
}
