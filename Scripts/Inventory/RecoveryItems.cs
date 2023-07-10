using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Create new Recovery Item")] 
public class RecoveryItems : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmnt;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionsID status;
    [SerializeField] bool restoreAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(PokemonInfo pokemon)
    {
        if(revive || maxRevive)
        {
            if (pokemon.HP > 0)
                return false;

            if(revive)
                pokemon.IncreaseHP(hpAmnt/2);
            else if(maxRevive)
                pokemon.IncreaseHP(pokemon.MaxHp);

            pokemon.CureStatus();

            return true;
        }

        if (pokemon.HP == 0)
            return false;

        if(restoreMaxHP || hpAmnt > 0)
        {
            if(pokemon.HP == pokemon.MaxHp)
            {
                return false;
            }

            if(restoreMaxHP)
                pokemon.IncreaseHP(pokemon.MaxHp);
            else
                pokemon.IncreaseHP(hpAmnt);
 
        }

        if (restoreAllStatus || status != ConditionsID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            if(restoreAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                {
                    pokemon.CureStatus();
                }
                else if (pokemon.VolatileStatus.Id == status)
                    pokemon.CureVolatileStatus();
                else
                    return false;
            }
        }

        if(restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
            pokemon.Boosts.ForEach(m => m.IncreasePP(m.BBase.PP));
        }
        else if(ppAmount > 0)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
            pokemon.Boosts.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}
