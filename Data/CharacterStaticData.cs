using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class CharacterStaticData {
	public string Tag  {get; private set;}
	public int Level  {get; private set;}
	public Dictionary<CSStats, int> CharStats  {get; private set;}
	public RoleData Role  {get; private set;}

	public CharacterStaticData() {}

	public CharacterStaticData(string tag, int level, Dictionary<CSStats, int> charStats, RoleData charRole) {

		Tag = tag;
		Level = level;
		CharStats = charStats;
		Role = charRole;
	}


}
