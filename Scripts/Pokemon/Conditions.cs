using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conditions 
{
    public ConditionsID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string  StartMessage { get; set; }
    public string EffectMessage { get; set; }
    public Action<PokemonInfo> OnStart { get; set; }
    public Func<PokemonInfo, bool> OnBeforeTurn { get; set; }
    public Action<PokemonInfo> OnAfterTurn { get; set; }
    public Action<PokemonInfo> OnWeather { get; set; }
    public Func<PokemonInfo, PokemonInfo, Move, float> OnDamageModify { get; set; }
}
