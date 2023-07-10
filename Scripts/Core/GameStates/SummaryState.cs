using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;

public class SummaryState : State<GameController>
{
    /*[SerializeField] PokemonSummary pokemonSum;
    [SerializeField] PartyScreen partyScreen;
    public static SummaryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        pokemonSum.gameObject.SetActive(true);
        pokemonSum.OnBack += OnBack;

        pokemonSum.ShowSummary(partyScreen.SelectedMember);
    }
    public override void Execute()
    {
        pokemonSum.HandleUpdateBack();
    }
    public override void Exit()
    {
        pokemonSum.gameObject.SetActive(false);
        pokemonSum.OnBack -= OnBack;
    }
    void OnBack()
    {
        gC.StateMachine.Pop();
    }*/
}
