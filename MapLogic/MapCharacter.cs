using Godot;
using System;

public partial class MapCharacter : Node, IMapObject
{
	//IMapObject Integration 
	public Vector2I GridPosition {get; set;}
	public Teams Team {get; private set;}
	
	public void SubscribeToCache() {
		var mapObjects = GetParent().GetParent() as MapObjects;
		mapObjects.MapObjectsAsked += OnMapObjectsAsked;
		mapObjects.PCSelected += OnPCSelected;
	}
	
	public void OnMapObjectsAsked(object cache, EventArgs e) {
		var typedCache = cache as MapObjects;
		typedCache.General.Add(GridPosition);
		switch(Team) {
			case Teams.Player: 
				typedCache.PlayerTeam.Add(GridPosition);
				break;
			case Teams.Enemy1:
				typedCache.EnemyTeam1.Add(GridPosition);
				break;
			case Teams.Friendly1:
				typedCache.FriendlyTeam1.Add(GridPosition);
				break;
			default: 
				break;
		}
	}
	//Map Misc
	public MapRangeHighlights MapRange;

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
		GridPosition = new Vector2I(6,1);
		SubscribeToCache();

		MapRange = GetNode<MapRangeHighlights>("/root/TestScene/Map/Highlight");

	}

	public void OnPCSelected(Vector2I position) {
		MapRange.HighlightMovement(new Vector2I(6, 1), 5, MovementMode.Infantry);
		MapRange.HighlightAttack();
	}

}
