using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Create new TM item")]
public class TMItem : ItemBase
{
    [SerializeField] MovesBase move;

    public override string Name => $"{move.Name}";
    public override string Description => $"Teaches {move.Name} to a pokemon";

    public override bool Use(PokemonInfo pokemon)
    {
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(PokemonInfo pokemon)
    {
        return pokemon.Base.LearnableMovesByTms.Contains(move);
    }

    public override bool CanUseInBattle => false;

    public MovesBase Move => move;
}
