using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Create new Evolution Item")]
public class EvoItem : ItemBase
{   
    public override bool Use(PokemonInfo pokemon)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;
}
