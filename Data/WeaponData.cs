using Godot;
using System;
using System.Linq;

public partial class WeaponData : Resource
{
	public string Tag {get; private set;}
	public int Damage {get; private set;}
	public float Speed {get; private set;}
	public float Crit {get; private set;}

	public int Kind {get{
		GD.PrintErr("Use WeaponData.GetKind() instead of WeaponData.Kind");
		return _kind;
	} 
	private set{  
		_kind = value;
		_csKind = (CSWeaponKinds)value;
	}}
	private int _kind;
	private CSWeaponKinds _csKind;


	public Godot.Collections.Array Attributes {
	get { 
		GD.PrintErr("Use WeaponData.GetAttributes() instead of WeaponData.Attributes");
		return _gdAttributes;
	}
	private set
	{
		if (_attributesSet){
			GD.PrintErr($"{Tag} class WeaponSlot2 already set");
			return;
		}
		
		_gdAttributes = value;

		if (value.Count == 0)
				_csAttributes = null;
		else    _csAttributes = value
				.Select(v => v.As<string>())
				.ToArray();

		_attributesSet = true;
	}}

	private bool _attributesSet;
	private Godot.Collections.Array _gdAttributes;
	private string[] _csAttributes;

	
	public void SetWeaponTag(string tag){
		if (Tag == null) Tag = tag;
		else GD.PrintErr($"{Tag}'s Tag already set");
	}    
	
	private bool _numbersSet;
	public void SetNumbers(int damage, float speed, float crit){
		if (!_numbersSet) {
			Damage = damage;
			Speed = speed;
			Crit = crit;
		}
		else GD.PrintErr($"{Tag}'s Damage, Speed and Crit already set");
	}   

	private bool _kindSet;
	public void SetKind(int weaponKind){
		if (!_kindSet) Kind = weaponKind;
		else GD.PrintErr($"{Tag}'s Kind already set");
	}


	public CSWeaponKinds GetWeaponKind() => (CSWeaponKinds)_csKind;


	public void SetAttributes(Godot.Collections.Array array) {
		if (_attributesSet) GD.PrintErr($"{Tag} Attributes already set");;
		if (array != null)
			Attributes = array;
		_attributesSet = true;}

	public string[] GetAttributes() => _csAttributes; 

}
