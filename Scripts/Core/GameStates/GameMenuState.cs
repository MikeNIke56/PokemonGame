using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    /*public static GameMenuState i { get; private set; }

    [SerializeField] MenuController menuController;

    private void Awake()
    {
        i = this;
    }
    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;
    }

    public override void Execute()
    {
        menuController.HandleUpdate();
    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }

    void OnMenuItemSelected(int selection)
    {
        if (selection == 0) //party
            gC.StateMachine.Push(GamePartyState.i);
        else if (selection == 1) //bag
            gC.StateMachine.Push(BagState.i);
        else if (selection == 2) //PC
            gC.StateMachine.Push(PCState.i);
    }
    void OnBack()
    {
        gC.StateMachine.Pop();
    }*/
}
