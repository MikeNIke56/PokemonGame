using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public virtual string Name => name;

    public virtual string Description => description;

    public Sprite Icon => icon;

    public virtual bool Use(PokemonInfo pokemon)
    {
        return false;
    }
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
