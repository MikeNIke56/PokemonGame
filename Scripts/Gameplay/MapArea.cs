using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounteredRecord> wildPokemons;

    private void Start()
    {
        int totalChance = 0;

        foreach(var record in wildPokemons)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chancePer;

            totalChance = totalChance + record.chancePer;
        }
    }
    public PokemonInfo GetRandomwildPokemon(BattleTrigger trigger)
    {
        int randVal = Random.Range(0, 101);
        var pokemonRecorded = wildPokemons.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var lvlRange = pokemonRecorded.lvlRange;

        int level = lvlRange.y == 0 ? lvlRange.x : Random.Range(lvlRange.x, lvlRange.y + 1);

        var wildPok = new PokemonInfo(pokemonRecorded.pokemon, level);

        wildPok.Init();
        return wildPok;
    }
}

[System.Serializable]
public class PokemonEncounteredRecord
{
    public PokemonBase pokemon;
    public Vector2Int lvlRange;
    public int chancePer;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
