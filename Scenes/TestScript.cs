using Godot;
using System;

public partial class TestScript : Node
{

	public override void _Ready()
	{
		var TestPCData2 = new PCData("Karla");
		GD.Print($"Karla2: {TestPCData2.Role.Tag	}");
		
		var TestPCData = new PCData("Karla");
		GD.Print($"Char Name: {TestPCData.Tag}");
		GD.Print($"Char Name: {TestPCData.Level}");
		
		GD.Print("=== Stats Check ===");
		foreach (CSStats stat in Enum.GetValues(typeof(CSStats)))
		{
			if (TestPCData._baseStats.TryGetValue(stat, out int value))
			{
				GD.Print($"{stat}: {value}");
			}
			else
			{
				GD.Print($"{stat}: NOT SET");
			}
		}
		GD.Print($"{TestPCData.Role.Tag	}");
		GD.Print($"allowed Types Weapon 0{string.Join(", ", TestPCData.Equipment.WeaponSlot(0))}");
		

	}
}
