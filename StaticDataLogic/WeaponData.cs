using Godot;
using System;
using System.Linq;

public enum DeBuffEnum {
	UNCHANGED,
	BUFFED, 
	DEBUFFED
} 

public enum WeaponPropEnum {
	DAMAGE,
	SPEED, 
	CRIT 
}

public partial class WeaponData : Resource
{
	public string Tag {get; protected set;}
	public int Damage {get; protected set;}
	public float Speed {get; protected set;}
	public float Crit {get; protected set;}

	public int Kind {get{
		GD.PrintErr("Use WeaponData.GetKind() instead of WeaponData.Kind");
		return _kind;
	} 
	protected set{  
		_kind = value;
		_csKind = (WeaponKindsEnum)value;
	}}

	public WeaponData() {}

	public WeaponData(string tag, int damage, float speed, float crit) {
		Tag = tag;
		Damage = damage;
		Speed = speed;
		Crit = crit;

	}

	public Godot.Collections.Array Attributes {
	get { 
		GD.PrintErr("Use WeaponData.GetAttributes() instead of WeaponData.Attributes");
		return _gdAttributes;
	}
	protected set
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

	//Handeling Kind
	private int _kind;
	private WeaponKindsEnum _csKind;
	private bool _kindSet;

	public void SetKind(int weaponKind){
		if (!_kindSet) Kind = weaponKind;
		else GD.PrintErr($"{Tag}'s Kind already set");
	}

	public WeaponKindsEnum GetWeaponKind() => (WeaponKindsEnum)_csKind;


	//handeling Attributes

	private bool _attributesSet;
	private Godot.Collections.Array _gdAttributes;
	private string[] _csAttributes;

	public void SetAttributes(Godot.Collections.Array array) {
		if (_attributesSet) GD.PrintErr($"{Tag} Attributes already set");;
		if (array != null)
			Attributes = array;
		_attributesSet = true;}

	public string[] GetAttributes() => _csAttributes; 
}




public partial class ModifiedWeaponData : WeaponData
{
	public DeBuffEnum DamageModified {get; private set;}
	public DeBuffEnum SpeedModified {get; private set;}
	public DeBuffEnum CritModified {get; private set;}

	ModifiedWeaponData() {}

	ModifiedWeaponData(WeaponData baseWeapon, int damageMod = 0, float speedMod = 0f, float critMod = 0f ) {

		Tag = baseWeapon.Tag;
		Attributes = baseWeapon.Attributes;
		Kind = baseWeapon.Kind;

		
		Damage = baseWeapon.Damage + damageMod;
		DamageModified = SetModified(damageMod);

		
		Speed = baseWeapon.Speed + speedMod;
		SpeedModified = SetModified(speedMod);
		
		Crit = baseWeapon.Crit + critMod;
		CritModified = SetModified(critMod);

	}
	private DeBuffEnum SetModified(float value) => Math.Sign(value) switch {
	1 => DeBuffEnum.BUFFED,
	-1 => DeBuffEnum.DEBUFFED,
	0 => DeBuffEnum.UNCHANGED,
};
}
