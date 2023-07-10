using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;

public class PCState : State<GameController>
{
    /*[SerializeField] PokemonPCUI pokemonPCUI;
    public static PCState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        pokemonPCUI.gameObject.SetActive(true);
        pokemonPCUI.OnSelected += OnItemSelected;
        pokemonPCUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        pokemonPCUI.HandleUpdate();
    }
    public override void Exit()
    {
        pokemonPCUI.gameObject.SetActive(false);
        pokemonPCUI.OnSelected -= OnItemSelected;
        pokemonPCUI.OnBack -= OnBack;
    }
    void OnItemSelected(int selection)
    {
        gC.StateMachine.Push(GamePartyState.i);
    }
    void OnBack()
    {
        gC.StateMachine.Pop();
    }*/
}