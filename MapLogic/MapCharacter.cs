using Godot;
using System;

public partial class MapCharacter : Node2D, IMapObject
{
	//IMapObject Integration 
	public Vector2I GridPosition {get; set;}
	public Teams Team {get; private set;}

	public void RegisterPosition() {
		((MapObjects)GetParent().GetParent()).ObjectCache.Add(GridPosition, Team);
	}
	
	//Map Misc	
	public void GetMapDTO(Vector2I position) {
		if (position != GridPosition) 
			return;

		if (((MapObjects)GetParent().GetParent()).CharDTO != null){
			GD.Print("Tried to define multiple CharDTOs in MapObjects");
			return;
		}
		((MapObjects)GetParent().GetParent()).CharDTO = new CharacterSelectionInfo(GridPosition, Team, MovementMode.Infantry, 6, AttackMode.CloseCombat);
	}


	//CharacterData
	[Export] public CharacterData MapCharacterData {get; private set;}

	public int MaxDefense {get; private set;}
	public int CurrentDefense {get; set;}
	
	public EquipmentObject MapEquipment {get; private set;}

	public string ChosenWeapon = "Weapon0";
	public MapCharacter() {}

	public override void _Ready()
	{	
		MapCharacterData = new PCData("Karla");

		MapEquipment = MapCharacterData.Equipment;

		MaxDefense = MapCharacterData.GetStat(StatsEnum.DEFENSE);
		CurrentDefense = MapCharacterData.GetStat(StatsEnum.DEFENSE);

		Team = Teams.Player;
		var grid = GetNode<TileGrid>("%Map");
		GridPosition = grid.GetGridPosition(Position);

		AddToGroup("mapObjects");
		AddToGroup("characters");
	}
}
public partial class CharacterSelectionInfo : GodotObject {
	public Vector2I TilePosition {get;}

	public Teams Team;

	public MovementMode MovementMode {get;}
	public int MovementRange {get;}

	public AttackMode AttackMode {get;}
	public int MinimumAttackRange {get;}
	public int MaximumAttackRange {get;}

	public CharacterSelectionInfo(Vector2I position, Teams team, MovementMode movementMode, int movement, AttackMode attackMode) {
		TilePosition = position;
		Team = team;

		MovementMode = movementMode;
		MovementRange = movement;

		AttackMode = attackMode;
	}

}
