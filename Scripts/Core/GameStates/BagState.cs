using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;

public class BagState : State<GameController>
{
    /*[SerializeField] InventoryUI inventoryUI;
    public static BagState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }
    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
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
