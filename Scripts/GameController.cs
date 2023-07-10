using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public enum GameState { FreeRoam, Battle, Dialogue, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution, Summary, PC, Restart }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] PartyScreen partyScreen2;
    [SerializeField] InventoryUI inventoryUi;
    [SerializeField] PokemonPCUI pc;
    [SerializeField] public PokemonSummary pokemonSummary;

    GameState state;
    GameState prevState;
    GameState stateBeforeEvolution;
    [SerializeField] MenuController menuController;

    public static GameController Instance { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    private void Awake()
    {
        Instance = this;

        ItemDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        BoostDB.Init();
        ConditionsDB.Init();
    }
    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();
        partyScreen2.Init();

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            prevState = state;
            state = GameState.Dialogue;
        };

        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.Dialogue)
                state = prevState;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.onMenuSelected += OnMenuSelected;

        EvolutionManager.i.OnStartEvo += () =>
        {
            stateBeforeEvolution = state;
            state = GameState.Evolution;
        };
        EvolutionManager.i.OnEndEvo += () =>
        {
            partyScreen.SetPartyData();
            partyScreen2.SetPartyData();
            state = stateBeforeEvolution;

            //AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);
        };
    }
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

    public void StartBattle(BattleTrigger trigger)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerMovement.GetComponent<PokemonParty>();
        var wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomwildPokemon(trigger);

        var wildPokemonCopy = new PokemonInfo(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger);
    }
    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerMovement.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>(); 

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerMovement));
    }
    void EndBattle(bool won)
    {
        if(trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        LeagueCounter.i.CheckForChamp();
        LeagueCounter.i.CheckForLoss();
        LeagueCounter.i.CheckForBeatGame();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        var party = playerMovement.GetComponent<PokemonParty>();
        bool hasEvos = party.CheckForEvo();

        if(hasEvos)
            StartCoroutine(party.RunEvos());
        else
           AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);


        partyScreen.SetPartyData();
        partyScreen2.SetPartyData();
    }


    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerMovement.HandleUpdate();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            if(prevState != GameState.PC)
                prevState = GameState.Menu;

            inventoryUi.gameObject.SetActive(false);
            partyScreen.gameObject.SetActive(true);
            partyScreen.ClearMemberSlotMessages();
            //prevState = GameState.Menu;

            Action onSelected = () =>
            {
                // Switch Pokemon
                if (prevState == GameState.PC)
                {
                    partyScreen.SwitchPokemon();
                }
                // Summary Screen
                else if (prevState == GameState.Menu || prevState == GameState.Summary)
                {
                    state = GameState.Summary;
                }
            };

            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
                prevState = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUi.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUi.HandleUpdate(onBack);
        }
        else if (state == GameState.PC)
        {
            Action onSelected = () =>
            {
                partyScreen.gameObject.SetActive(true);
                pc.gameObject.SetActive(false);

                state = GameState.PartyScreen;
                prevState = GameState.PC;
            };

            Action onBack = () =>
            {
                pc.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
            pc.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.Summary)
        {
            prevState = GameState.PartyScreen;
            partyScreen.gameObject.SetActive(false);
            pokemonSummary.gameObject.SetActive(true);
            pokemonSummary.ShowSummary(partyScreen.SelectedMember);

            if (Input.GetKeyDown(KeyCode.X))
            {
                state = GameState.PartyScreen;
                prevState = GameState.Summary;
                pokemonSummary.gameObject.SetActive(false);
                partyScreen.gameObject.SetActive(true);
            }
        }
    }

    public void SetCurrentScene(SceneDetails currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
        {
            //pokemon
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            //bag
            inventoryUi.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            //pc
            pc.gameObject.SetActive(true);
            state = GameState.PC;
        }
        else if (selectedItem == 3)
        {
            //save
            SavingSystem.i.Save("saveSlot1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 4)
        {
            //load
            SavingSystem.i.Load("saveSlot1");
            state = GameState.FreeRoam;
        }
    }

    public PlayerMovement PlayerMovement => playerMovement;
    public Camera WorldCamera => worldCamera;

    public GameState State => state;
    public GameState PrevState => prevState;
}

