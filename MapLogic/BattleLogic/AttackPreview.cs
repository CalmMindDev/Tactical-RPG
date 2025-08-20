using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

enum CombatActionsEnum {
    NOTHING,
    HIT,
    CHANCE
}
enum CombatPositionsEnum {
    ATTACKER, 
    DEFENDER, 
    ATCSUPPORT,
    DEFSUPPORT
}

public partial class AttackPreview : Node 
{
    private MapCharacter Attacker;
    private WeaponData AtcWeapon;
    private MapCharacter Defender;
    private WeaponData DefWeapon;

    private MapCharacter AtcSupport;
    private MapCharacter DefSupport;

    public AttackPreview(MapCharacter attacker, MapCharacter defender, 
    MapCharacter atcSupport = null, MapCharacter defSupport = null) 
    {
        Attacker = attacker;
        Defender = defender;

        AtcWeapon = Attacker.MapCharacterData.Equipment.Weapon(Attacker.ChosenWeapon);
        DefWeapon = Defender.MapCharacterData.Equipment.Weapon(Defender.ChosenWeapon);

        CalcHits();
        GetRounds();
    }


    private float AtcHits;
    private float DefHits;

    private void CalcHits() 
    {
        var atcSkill = Attacker.MapCharacterData.GetStat(StatsEnum.SKILL);
        var defSkill = Attacker.MapCharacterData.GetStat(StatsEnum.SKILL);

        var atcReact = Defender.MapCharacterData.GetStat(StatsEnum.REACT);
        var defReact = Defender.MapCharacterData.GetStat(StatsEnum.REACT);

        var atcWSpeed = AtcWeapon.Speed;
        var defWSpeed = DefWeapon.Speed;

        //OnHitCalculation

        float atcHits = (atcSkill/50f) * 5f - defWSpeed;
        if (atcSkill < defReact)
            atcHits = (atcSkill/defReact)*atcHits;
        atcHits = (float)Math.Round(atcHits,2);
        AtcHits = atcHits;

        float defHits = (defSkill/50f) * 2.5f - atcWSpeed;
        if (defSkill < atcReact)
            atcHits = (atcSkill/defReact)*atcHits;
        defHits = (float)Math.Round(defHits, 2);
        DefHits = defHits;

        GD.Print($"Attacker Hits: {AtcHits}");
        GD.Print($"Defender Hits: {DefHits}");
    }
    

    private List<(CombatActionsEnum Atc, CombatActionsEnum Def)> Rounds;
    private void GetRounds()
    {
    
    List<CombatActionsEnum> atcActions = new();

    if ((AtcHits - Math.Floor(AtcHits)) > 0)
        atcActions.Add(CombatActionsEnum.CHANCE);
    for (int i = 0; i < Math.Floor(AtcHits); i++){
        atcActions.Add(CombatActionsEnum.HIT);
    }
    
    List<CombatActionsEnum> defActions = new();

    if ((DefHits - Math.Floor(DefHits)) > 0)
        defActions.Add(CombatActionsEnum.CHANCE);
    for (int i = 0; i < Math.Floor(DefHits); i++){
        defActions.Add(CombatActionsEnum.HIT);
    }

    int diff = Math.Abs(atcActions.Count - defActions.Count);
    
    if (atcActions.Count<defActions.Count)
        Enumerable.Range(0, diff).ToList().ForEach(_ =>
        {
            atcActions.Add(CombatActionsEnum.NOTHING);
        });

    if (atcActions.Count>defActions.Count)
        Enumerable.Range(0, diff).ToList().ForEach(_ =>
        {
            defActions.Add(CombatActionsEnum.NOTHING);
        });
    
    Rounds =    atcActions
                .Zip(defActions.AsEnumerable().Reverse(), (a, d) => (Atc: a, Def: d))
                .ToList();
    for (int i = 0; i < Rounds.Count; i++) {
            GD.Print($"Runde {i+1}: Angreifer = {Rounds[i].Atc}, Verteidiger = {Rounds[i].Def}");
        }
    }
}