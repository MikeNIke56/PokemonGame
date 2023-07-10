using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePartyState : State<GameController>
{
    /*[SerializeField] PartyScreen partyScreen;
    public static GamePartyState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }
    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }
    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }
    void OnPokemonSelected(int selection)
    {
        if(gC.StateMachine.GetPrevState() == BagState.i)
        {
            StartCoroutine(GoToItemUseState());
        }
        else if (gC.StateMachine.GetPrevState() == PCState.i)
        {
            StartCoroutine(GoToPCSwitchUseState());
        }
        else if (gC.StateMachine.GetPrevState() == GameMenuState.i)
        {
            StartCoroutine(GoToSummaryState());
        }
        else
        {

        }
    }

    IEnumerator GoToItemUseState()
    {
        yield return gC.StateMachine.PushAndWait(UseItemState.i);
        gC.StateMachine.Pop();
    }
    IEnumerator GoToPCSwitchUseState()
    {
        yield return gC.StateMachine.PushAndWait(SwitchPokemonState.i);
        gC.StateMachine.Pop();
    }
    IEnumerator GoToSummaryState()
    {
        yield return gC.StateMachine.PushAndWait(SummaryState.i);
        gC.StateMachine.Pop();
    }
    void OnBack()
    {
        gC.StateMachine.Pop();
    }*/

}
