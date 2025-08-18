using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public static class GDScriptData {

	private static string PathName(string folder, string objectName){
		if (!objectName.EndsWith(".tres"))
			objectName += ".tres";
		objectName = objectName.Replace(" ", "");
		var path = $"{folder}{objectName}";
		return path;
	}
	private static string FileName(string objectName){
		if (!objectName.EndsWith(".tres"))
			objectName = objectName.Replace(".tres", "");
		objectName = objectName.Replace(" ", "");
		return objectName;
	}


	//-------------------------------------------------------------------
	//--------------------------- Role ---------------------------------
	//-------------------------------------------------------------------

	private static Dictionary<string, RoleData> _roleCache = new();
	
	public static RoleData LoadRole(string roleTag) 
	{
		var folder = "res://Data/Roles/";
		var path = "";
		var fileName = "";

		if (roleTag.Contains(folder)){
			path = roleTag;
			fileName = Path.GetFileNameWithoutExtension(roleTag);
		}
		else {
			path = PathName(folder, roleTag);
			fileName = FileName(roleTag);
		}

		if (_roleCache.TryGetValue(fileName, out var existing))
			return existing;

		//else
		var resource = GD.Load<Resource>(path);
		if (resource == null)
			GD.PrintErr($"Failed to load Role Data from {path}");

		var typedResource = TransferRoleData(resource);

		if (typedResource.GetWeaponSlot0() == null) GD.PrintErr($"No Equipment Slot 0 set for {path}");

		_roleCache.Add(fileName, typedResource);
		return typedResource;	
	}

	private static RoleData TransferRoleData(Resource gdRoleData)
	{
		var roleData = new RoleData();
		roleData.SetRoleTag(gdRoleData.Get("Tag").As<string>());

		roleData.SetWeaponSlot0((Godot.Collections.Array)gdRoleData.Get("WeaponSlot0"));
		roleData.SetWeaponSlot1((Godot.Collections.Array)gdRoleData.Get("WeaponSlot1"));
		roleData.SetWeaponSlot2((Godot.Collections.Array)gdRoleData.Get("WeaponSlot2"));

		return roleData;	

	}

	//-------------------------------------------------------------------
	// -------------------------- Weapon --------------------------------
	//-------------------------------------------------------------------

	private static Dictionary<string, WeaponData> _weaponCache = new();

	public static WeaponData LoadWeapon(string weaponTag) 
	{		
		var folder = "res://Data/Weapons/";
		var path = "";
		var fileName = "";

		if (weaponTag.Contains(folder)){
			path = weaponTag;
			fileName = Path.GetFileNameWithoutExtension(weaponTag);
		}
		else {
			path = PathName(folder, weaponTag);
			fileName = FileName(weaponTag);
		}

		if (_weaponCache.TryGetValue(fileName, out var existing))
			return existing;

		var resource = GD.Load<Resource>(path);
		if (resource == null)
			GD.PrintErr($"Failed to load Weapon Data from {path}");

		var typedResource = TransferWeaponData(resource);
		_weaponCache.Add(fileName, typedResource);
		return typedResource;
	}
	private static WeaponData TransferWeaponData(Resource gdWeaponData)
	{
		var weaponData = new WeaponData();
		
		weaponData.SetWeaponTag(gdWeaponData.Get("Tag").As<string>());
		weaponData.SetAttributes((Godot.Collections.Array)gdWeaponData.Get("Attributes"));
		weaponData.SetKind(gdWeaponData.Get("Kind").As<int>());
		
		var damage = gdWeaponData.Get("Damage").As<int>();
		var speed = gdWeaponData.Get("Speed").As<float>();
		var crit = gdWeaponData.Get("Crit").As<float>();
		weaponData.SetNumbers(damage, speed, crit);

		return weaponData;

	}

	//-------------------------------------------------------------------
	// ------------------------ Character -------------------------------
	//-------------------------------------------------------------------

	public static CharacterStaticData LoadCharacter(string charName) 
	{
		var folder = "res://Data/Characters/";
		var path = "";
		
		if (charName.Contains(folder))
			path = charName;
		else
			path = PathName(folder, charName);

		var resource = GD.Load<Resource>(path);
		if (resource == null)
			GD.PrintErr($"Failed to load Character Data from {path}");

		var typedResource = TransferCharData(resource);
		return typedResource;
	}

	private static CharacterStaticData TransferCharData(Resource gdCharData)
	{
		var tag = gdCharData.Get("Tag").As<string>();
		var level = gdCharData.Get("Level").As<int>();
		var stats = new Dictionary<CSStats, int>{
			{CSStats.DEFENSE, gdCharData.Get("Defense").As<int>()},
			{CSStats.SKILL, gdCharData.Get("Skill").As<int>()},
			{CSStats.REACT, gdCharData.Get("React").As<int>()},
			{CSStats.FOCUS, gdCharData.Get("Focus").As<int>()},
			{CSStats.AURA, gdCharData.Get("Aura").As<int>()},
			{CSStats.PRECISION, gdCharData.Get("Precision").As<int>()},
		};
		var roleResource = (Godot.Resource)gdCharData.Get("Role");
		var roleData = LoadRole(roleResource.ResourcePath);
		var csCharData = new CharacterStaticData(tag, level, stats, roleData);
		return csCharData;
	}

} 	
