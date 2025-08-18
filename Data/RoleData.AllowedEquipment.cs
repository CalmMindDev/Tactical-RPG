using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


public partial class RoleData : Resource
{
	private CSWeaponKinds[] _weaponSlot0; 
	private CSWeaponKinds[] _weaponSlot1; 
	private CSWeaponKinds[] _weaponSlot2; 
	
	private Godot.Collections.Array _gdWeaponSlot0;
	private Godot.Collections.Array _gdWeaponSlot1;
	private Godot.Collections.Array _gdWeaponSlot2;
	
	private bool _weaponSlot0set;
	private bool _weaponSlot1set;
	private bool _weaponSlot2set;
	
	public Godot.Collections.Array WeaponSlot0 {
	get { 
		GD.PrintErr("Use RoleData.GetWeponSlot0() instead of RoleData.WeaponSlot0");
		return _gdWeaponSlot0;
	}
	private set
	{	
		//SetWeaponSlot() prevents null value

		if (value.Count == 0) {
			_weaponSlot0 = null;
			_gdWeaponSlot0 = null;
			return;
		}

		_gdWeaponSlot0 = value;
		_weaponSlot0 = value
			.Select(v => (CSWeaponKinds)v.As<int>())
			.ToArray();
	}}


	public Godot.Collections.Array WeaponSlot1 {
	get { 
		GD.PrintErr("Use RoleData.GetWeponSlot1() instead of RoleData.WeaponSlot1");
		return _gdWeaponSlot1;
	}
	private set
	{
		//SetWeaponSlot() prevents null value

		if (value.Count == 0){
				_weaponSlot1 = null;
				_gdWeaponSlot1 = null;
				return;
		}  

		_gdWeaponSlot1 = value;
		_weaponSlot1 = value
			.Select(v => (CSWeaponKinds)v.As<int>())
			.ToArray();

		_weaponSlot1set = true;
	}}


	[Export] public Godot.Collections.Array WeaponSlot2 {
	get { 
		GD.PrintErr("Use RoleData.GetWeponSlot2() instead of RoleData.WeaponSlot2");
		return _gdWeaponSlot2;
	}
	private set
	{
		//SetWeaponSlot() prevents null value

		if (value.Count == 0){
				_weaponSlot2 = null;
				_gdWeaponSlot2 = null;
				return;
		}

		_gdWeaponSlot2 = value;
		_weaponSlot2 = value
			.Select(v => (CSWeaponKinds)v.As<int>())
			.ToArray();

		_weaponSlot2set = true;
	}}


	public void SetWeaponSlot0(Godot.Collections.Array array) {
		if (_weaponSlot0set) GD.PrintErr($"{Tag}'s WeaponSlot0 already set");;
		if (array != null)
			WeaponSlot0 = array;
		_weaponSlot0set = true;}
	public void SetWeaponSlot1(Godot.Collections.Array array) {
		if (_weaponSlot1set) GD.PrintErr($"{Tag}'s WeaponSlot1 already set");;
		if (array != null)
			WeaponSlot1 = array;
		_weaponSlot1set = true;}
	public void SetWeaponSlot2(Godot.Collections.Array array) {
		if (_weaponSlot2set) GD.PrintErr($"{Tag}'s WeaponSlot2 already set");;
		if (array != null)
			WeaponSlot2 = array;
		_weaponSlot2set = true;}


	public CSWeaponKinds[] GetWeaponSlot0() =>  _weaponSlot0;
	public CSWeaponKinds[] GetWeaponSlot1() =>  _weaponSlot1;
	public CSWeaponKinds[] GetWeaponSlot2() =>  _weaponSlot2;

	public Dictionary<string, Func<CSWeaponKinds>> WeaponSlots;
	
	
	public Dictionary<string, CSWeaponKinds[]> GetWeaponSlots()	{
		var weaponSlots = new Dictionary<string, CSWeaponKinds[]>{};
		if (GetWeaponSlot0() != null)
			weaponSlots.Add("Weapon0", GetWeaponSlot0());
		if (GetWeaponSlot1() != null)
			weaponSlots.Add("Weapon1", GetWeaponSlot1());
		if (GetWeaponSlot2() != null)
			weaponSlots.Add("Weapon2", GetWeaponSlot2());
		return weaponSlots;
	}
}
