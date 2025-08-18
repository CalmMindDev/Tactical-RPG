using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


public partial class EquipmentObject : Resource
{
    private Dictionary<string, EquipmentSlot<WeaponData, CSWeaponKinds>> _weaponSlots;
    public int[] StatBonuses {get; private set;}

	public EquipmentObject() {
        _weaponSlots = new Dictionary<string, EquipmentSlot<WeaponData, CSWeaponKinds>>{};
    }

	public void Initialize(Dictionary<string, CSWeaponKinds[]> weaponSlots) {
        foreach (var kvp in weaponSlots){
            _weaponSlots.Add(kvp.Key, new EquipmentSlot<WeaponData, CSWeaponKinds>(kvp.Value));
        }
    }

    public void Equip(int numb, WeaponData weapon) {
        var key = "Weapon" + numb.ToString();
        if (!_weaponSlots.ContainsKey(key)){
            GD.PrintErr($"Weaponslot {key} doesn't exist");
            return;
        }
        if (!_weaponSlots[key].AllowedKinds.Contains(weapon.GetWeaponKind())){
            GD.PrintErr($"Can't equip {weapon.Tag} in {key}");
            return;
        }
        _weaponSlots[key].Item = weapon;
    }

    public WeaponData Weapon(int numb) {
        var key = "Weapon" + numb.ToString();
        if (!_weaponSlots.ContainsKey(key)){
            GD.PrintErr($"Weapon(int): Weaponslot {key} doesn't exist");
            return null;
        }
        return _weaponSlots[key].Item;
    }
    public CSWeaponKinds[] WeaponSlot(int numb) {
        var key = "Weapon" + numb.ToString();
        if (!_weaponSlots.ContainsKey(key)){
            GD.PrintErr($"WeaponSlot(int): {key} doesn't exist");
            return null;
        }
        return _weaponSlots[key].AllowedKinds;
    }

    private class EquipmentSlot<TData, TType>
    {
        public TData Item { get; set; }
        public TType[] AllowedKinds { get; set; }

        public EquipmentSlot() {}

        public EquipmentSlot(TType[] allowedKinds)
        {
            AllowedKinds = allowedKinds;
        }
    }
}