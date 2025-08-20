using Godot;
using System;

public partial class TestScript : Node
{
		public override void _Ready()
	{
		var mapChar =  new MapCharacter(new PCData("Karla"));
		var mapChar2 = new MapCharacter(new PCData("Roman"));
		
		mapChar.MapCharacterData.Equipment.Equip(0, GDScriptData.LoadWeapon("Heavy Spear"));
		mapChar2.MapCharacterData.Equipment.Equip(0, GDScriptData.LoadWeapon("Light Spear"));
		
		var attack = new AttackPreview(mapChar, mapChar2);
	}

};
