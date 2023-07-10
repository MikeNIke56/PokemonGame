using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<PokemonInfo> pokemons;

    public event Action OnUpdated;

    public List<PokemonInfo> Pokemon
    {
        get
        {
            return pokemons;
        }
        set
        {
            pokemons = value;
            OnUpdated?.Invoke();
        }
    }

    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }
    private void Start()
    {
        
    }

    public PokemonInfo GetHealthyPokemon()
    {
       return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddPokemon(PokemonInfo newPok)
    {
        if(pokemons.Count < 6)
        {
            pokemons.Add(newPok);
            OnUpdated?.Invoke();
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerMovement>().GetComponent<PokemonParty>();
    }

    public bool CheckForEvo()
    {
        return pokemons.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvos()
    {
        foreach (var pokemon in pokemons)
        {
            var evolution = pokemon.CheckForEvolution();
            if(evolution != null)
            {
                yield return EvolutionManager.i.Evolve(pokemon, evolution);
            }
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }
}
