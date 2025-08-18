using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class CharacterData : Resource
{
	public int Level;

	//wieder protected machen
	public Dictionary<CSStats, int> _baseStats;

	public EquipmentObject Equipment;

	//Das englische Wort Class kann vlt etwas verwirrend mit Class in C# sein, deswegen das Deutsche
	public RoleData Role;


	public CharacterData() 
	{
		Level = 1;

		_baseStats = new Dictionary<CSStats, int> {
			{ CSStats.DEFENSE, 		1},
			{ CSStats.SKILL,   		1}, 
			{ CSStats.REACT,  		1}, 
			{ CSStats.FOCUS,   		1}, 
			{ CSStats.AURA,   		1}, 
			{ CSStats.PRECISION, 	1}
		};

		Equipment = new EquipmentObject();
	}
}

public partial class PCData : CharacterData {
	
	public string Tag;
	public PCData() {}

	public PCData(string charName) {
		var charData = GDScriptData.LoadCharacter(charName);
		Tag = charData.Tag;
		Level = charData.Level;
		Role = charData.Role;
		_baseStats = charData.CharStats;
		Equipment.Initialize(Role.GetWeaponSlots());
	}




}
