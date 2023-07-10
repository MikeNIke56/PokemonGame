using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStartState : State<GameController>
{
    /*[SerializeField] BattleSystem battleSystem;
    public static BattleStartState i { get; private set; }
    public TrainerController trainerController { get; set; }

    [SerializeField] GameObject inventoy;
    [SerializeField] GameObject partyScreen;

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;


        battleSystem.gameObject.SetActive(true);
        gC.WorldCamera.gameObject.SetActive(false);
        inventoy.SetActive(false);
        partyScreen.SetActive(false);

        var playerParty = gC.PlayerMovement.GetComponent<PokemonParty>();

        if(trainerController == null)
        {
            var wildPokemon = gC.CurrentScene.GetComponent<MapArea>().GetComponent<MapArea>().GetRandomwildPokemon();
            var wildPokCopy = new PokemonInfo(wildPokemon.Base, wildPokemon.Level);
            battleSystem.StartBattle(playerParty, wildPokCopy);
        }
        else
        {
            var trainerParty = trainerController.GetComponent<PokemonParty>();

            battleSystem.StartTrainerBattle(playerParty, trainerParty);
        }
        battleSystem.OnBattleOver += EndBattle;


    }

    public override void Exit()
    {
        battleSystem.gameObject.SetActive(false);
        gC.WorldCamera.gameObject.SetActive(true);

        battleSystem.OnBattleOver -= EndBattle;
    }

    public override void Execute()
    {
        battleSystem.HandleUpdate();
    }

    void EndBattle(bool won)
    {
        if (trainerController != null && won == true)
        {
            trainerController.BattleLost();
            trainerController = null;
        }
        gC.StateMachine.Pop();
    }*/
}
