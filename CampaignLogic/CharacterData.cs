using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public abstract partial class CharacterData : Resource
{
	public int Level {get; protected set;}
	protected Dictionary<StatsEnum, int> _baseStats;

	public EquipmentObject Equipment {get; protected set;}

	//Das englische Wort Class kann vlt etwas verwirrend mit Class in C# sein, deswegen das Deutsche
	public RoleData Role {get; protected set;}


	public CharacterData() 
	{
		Level = 1;

		_baseStats = new Dictionary<StatsEnum, int> {
			{ StatsEnum.DEFENSE, 		1},
			{ StatsEnum.SKILL,   		1}, 
			{ StatsEnum.REACT,  		1}, 
			{ StatsEnum.FOCUS,   		1}, 
			{ StatsEnum.AURA,   		1}, 
			{ StatsEnum.PRECISION, 	1}
		};

		Equipment = new EquipmentObject();
	}

	public Dictionary<StatsEnum, int> GetStats() {
		return _baseStats;
	}
	public int GetStat(StatsEnum stat) {
		return _baseStats[stat];
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
