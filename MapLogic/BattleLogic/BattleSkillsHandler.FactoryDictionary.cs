using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class CombatTraitsHandler
{  
    private Dictionary<CombatTraitsEnum, Action<CombatPositionsEnum>> CombatTraitsFactory = new Dictionary<CombatTraitsEnum, Action<CombatPositionsEnum>>{

        //Trait Overwhelm
        {CombatTraitsEnum.OVERWHELM, pos => {
            if (pos == CombatPositionsEnum.ATTACKER)
                ActiveCombatTraits.Add(new OverwhelmTrait(pos));
            }},
};

}