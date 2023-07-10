using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;
using System.Linq;

public class SwitchPokemonState : State<GameController>
{
   /* [SerializeField] PokemonPCUI pokemonPCUI;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] PokemonParty pokemonParty;
    public static SwitchPokemonState i { get; private set; }

    private void Awake()
    {
        i = this;

    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        SwitchPokemon();
    }

    void SwitchPokemon()
    {
        var pcPok = pokemonPCUI.list.pokemonList[pokemonPCUI.selectedItem];

        pokemonParty.Pokemon[partyScreen.selectedItem] = pcPok;
        pcPok.Init();
        pokemonParty.PartyUpdated();
        gC.StateMachine.Pop();
    }*/
}
