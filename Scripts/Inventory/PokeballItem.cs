using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Create new pokeball Item")]
public class PokeballItem : ItemBase
{
    public override bool Use(PokemonInfo pokemon)
    {
        return true;
    }
}
