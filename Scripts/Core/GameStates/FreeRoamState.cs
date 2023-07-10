using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameController>
{
    /*public static FreeRoamState i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
    }
    public override void Execute()
    {
        PlayerMovement.i.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gC.StateMachine.Push(GameMenuState.i);
        }
    }*/
}
