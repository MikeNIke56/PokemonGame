using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForgetState : State<GameController>
{
    /*public static MoveForgetState i { get; private set; }
    public List<MovesBase> CurrentMoves { get; set; }
    public int Selection { get; set; }
    public MovesBase NewMove { get; set; }

    [SerializeField] MoveForgetSelection moveForgetSelection;

    private void Awake()
    {
        i = this;
    }
    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        Selection = 0;

        moveForgetSelection.gameObject.SetActive(true);
        moveForgetSelection.SetMoveData(CurrentMoves, NewMove);
        moveForgetSelection.OnSelected += OnMoveSelected;
        moveForgetSelection.OnBack += OnBack;
    }

    public override void Execute()
    {
        moveForgetSelection.HandleUpdate();
    }

    public override void Exit()
    {
        moveForgetSelection.gameObject.SetActive(false);
        moveForgetSelection.OnSelected -= OnMoveSelected;
        moveForgetSelection.OnBack -= OnBack;
    }

    void OnMoveSelected(int selection)
    {
        Selection = selection;
        gC.StateMachine.Pop();
    }
    void OnBack()
    {
        Selection = -1;
        gC.StateMachine.Pop();
    }*/
}