using Godot;
using System;

public partial class MapCharacter : Node 
{
	public CharacterData MapCharacterData {get; private set;}

	[Export] public int MaxDefense {get; private set;}
	[Export] public int CurrentDefense {get; set;}
	
	public EquipmentObject MapEquipment {get; private set;}

	public string ChosenWeapon = "Weapon0";

	public MapCharacter() {}

	public MapCharacter(CharacterData charData)
	{
		MapCharacterData = charData;
		MapEquipment = MapCharacterData.Equipment;


		MaxDefense = MapCharacterData.GetStat(StatsEnum.DEFENSE);
		CurrentDefense = MapCharacterData.GetStat(StatsEnum.DEFENSE);
	}

}
