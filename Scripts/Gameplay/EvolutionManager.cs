using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    [SerializeField] AudioClip evoMusic;

    public event Action OnStartEvo;
    public event Action OnEndEvo;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(PokemonInfo pokemon, Evolution evolution)
    {
        OnStartEvo?.Invoke();

        string prevPok = pokemon.Base.Name;
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evoMusic);

        pokemonImage.sprite = pokemon.Base.Frontsprite;
        yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} is changing!");

        pokemon.Evolve(evolution);

        pokemonImage.sprite = pokemon.Base.Frontsprite;
        yield return DialogueManager.Instance.ShowDialogueText($"{prevPok} has changed into {pokemon.Base.Name}!");

        evolutionUI.SetActive(false);

        OnEndEvo?.Invoke();
    }
}
