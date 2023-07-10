using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, Bag, BattleOver, ABoostSelection, AboutToUsePokemon, MoveForget, CRBar, EnemyTurn, Summary }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run, Boost}

public enum BattleTrigger { LongGrass, Water }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] public BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] public BattleDialogueBox dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveForgetSelection moveForgetSelection;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] SummaryOpt summaryOpt;

    [SerializeField] public Image bg;
    [SerializeField] public Image arceusBg;


    public Field Field { get; private set; }

    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;


    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    bool aboutToUseChoice = true;

    int currentBoost;

    PokemonParty playerParty;
    PokemonParty trainerParty;
    PokemonInfo wildPokemon;

    PlayerMovement player;
    TrainerController trainer;

    bool isTrainerBattle = false;

    MovesBase moveToLearn;

    string missMessage;


    [SerializeField] CRBar cRBar;
    public PokemonInfo[] activePokemon = new PokemonInfo[2];

    public bool didMegaPlay = false;
    public bool didMegaEn = false;
    int playerMCount = 0;
    int enemyMCount = 0;

    int megaChance = 40;
    public GameObject[] weatherfxs;

    public bool didFail = false;

    [SerializeField] public Text curHP;

    public static BattleSystem i { get; private set; }


    private void Awake()
    {
        i = this;
    }

    public void StartBattle(PokemonParty playerParty, PokemonInfo wildPokemon, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        isTrainerBattle = false;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;

        player = playerParty.GetComponent<PlayerMovement>();

        AudioManager.i.PlayMusic(wildBattleMusic);

        StartCoroutine(SetupBattle());
        dialogueBox.actionSelector.gameObject.SetActive(false);
    }
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;

        trainer = trainerParty.GetComponent<TrainerController>();
        player = playerParty.GetComponent<PlayerMovement>();

        AudioManager.i.PlayMusic(trainerBattleMusic);

        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        Field = new Field();
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            //wild pokemon battle
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);

            dialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);
            dialogueBox.SetBoostNames(playerUnit.Pokemon.Boosts);
            yield return dialogueBox.TypeDialogue($"A wild {enemyUnit.Pokemon.Base.Name} appears.");
        }
        else
        {
            //trainer battle
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogueBox.TypeDialogue($"{trainer.Name} wants to battle");

            //trainer sends out first pokemon
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPok = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPok);      

            activePokemon[0] = enemyPok;



            yield return dialogueBox.TypeDialogue($"{trainer.Name} sends out {enemyPok.Base.Name}");

            if (enemyUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
            {
                Field.SetWeather(enemyUnit.Pokemon.Base.FieldEffect);
                WeatherToPlay(enemyUnit.Pokemon.Base.FieldEffect);
                yield return new WaitForSeconds(1);
                yield return dialogueBox.TypeDialogue(Field.Weather.StartMessage);

                if (Field.Weather.Id == ConditionsID.creatorsWill)
                {
                    if (bg.gameObject.activeInHierarchy == true)
                    {
                        bg.gameObject.SetActive(false);
                        arceusBg.gameObject.SetActive(true);
                    }
                }               
            }

            //player sends out first pokemon
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPok = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPok);

            activePokemon[1] = playerPok;


            yield return dialogueBox.TypeDialogue($"Go {playerPok.Base.Name}!");

            if (playerUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
            {

                Field.SetWeather(playerUnit.Pokemon.Base.FieldEffect);
                WeatherToPlay(playerUnit.Pokemon.Base.FieldEffect);
                yield return new WaitForSeconds(1);
                yield return dialogueBox.TypeDialogue(Field.Weather.StartMessage);

                if (Field.Weather.Id == ConditionsID.creatorsWill)
                {
                    if (bg.gameObject.activeInHierarchy == true)
                    {
                        bg.gameObject.SetActive(false);
                        arceusBg.gameObject.SetActive(true);
                    }
                }
            }

            dialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);
            dialogueBox.SetBoostNames(playerUnit.Pokemon.Boosts);
        }


        cRBar.gameObject.SetActive(true);

        partyScreen.Init();
        yield return new WaitForSeconds(1f);
        state = BattleState.CRBar;
    }
    public void SetupMegaBattlePlayer()
    {
        cRBar.gameObject.SetActive(false);
        Field = new Field();
        playerUnit.Clear();

        playerUnit.gameObject.SetActive(false);

        var playerPok = playerUnit.Pokemon;
        playerUnit.Setup(playerPok);

        activePokemon[1] = playerPok;

        dialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);
        dialogueBox.SetBoostNames(playerUnit.Pokemon.Boosts);

        playerUnit.gameObject.SetActive(true);

        cRBar.gameObject.SetActive(true);
        cRBar.UpdateListMegaPlayer(playerPok);

        cRBar.attackingPokemon = playerUnit.Pokemon;
        cRBar.defendingPokemon = enemyUnit.Pokemon;

        if (playerUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
        {
            Field.SetWeather(playerUnit.Pokemon.Base.FieldEffect);
            WeatherToPlay(playerUnit.Pokemon.Base.FieldEffect);
            StartCoroutine(dialogueBox.TypeDialogue(Field.Weather.StartMessage));
        }

        partyScreen.Init();
    }
    public void SetupMegaBattleEnemy()
    {
        cRBar.gameObject.SetActive(false);
        Field = new Field();
        enemyUnit.Clear();

        enemyUnit.gameObject.SetActive(false);

        var enemyPok = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(enemyPok);

        if (enemyUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
        {
            Field.SetWeather(enemyUnit.Pokemon.Base.FieldEffect);
            StartCoroutine(dialogueBox.TypeDialogue(Field.Weather.StartMessage));
        }

        activePokemon[0] = enemyPok;

        enemyUnit.gameObject.SetActive(true);

        cRBar.gameObject.SetActive(true);
        cRBar.UpdateListMegaEn(enemyPok);

        cRBar.attackingPokemon = enemyUnit.Pokemon;
        cRBar.defendingPokemon = playerUnit.Pokemon;
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemon.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();

        didMegaPlay = false;
        didMegaEn = false;

        cRBar.attackingPokemon = null;
        cRBar.defendingPokemon = null;
        cRBar.gameObject.SetActive(false);
        ResetWeather();

        bg.gameObject.SetActive(true);
        arceusBg.gameObject.SetActive(true);

        megaChance = 50;

        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogueBox.SetDialogue("Choose an action");
        dialogueBox.EnableActionSelector(true);
        cRBar.PauseUnits();
    }
    void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }
    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
        dialogueBox.EnableBoostSelector(false);
    }
    IEnumerator NewPokemon(PokemonInfo newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{trainer.Name} is about to send out {newPokemon.Base.Name}. Will you switch pokemon?");

        state = BattleState.AboutToUsePokemon;
        dialogueBox.EnableChoiceBox(true);
    }
    IEnumerator ChooseMoveToForget(PokemonInfo pokemon, MovesBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"Choose a move you want to forget.");
        moveForgetSelection.gameObject.SetActive(true);
        moveForgetSelection.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = BattleState.MoveForget;
    }
    void BoostSelection()
    {
        state = BattleState.ABoostSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableBoostSelector(true);
        dialogueBox.EnableMoveSelector(false);
    }

    private void Update()
    {
        if (didMegaPlay == true && playerMCount == 0)
        {
            cRBar.ExtraTurn(cRBar.pokemonUIList[1]);
            playerMCount+=1;
        }
        if (didMegaEn == true && enemyMCount == 0)
        {
            cRBar.ExtraTurn(cRBar.pokemonUIList[0]);
            enemyMCount+=1;
        }
    }

    IEnumerator RunCRBar()
    {
        cRBar.Move();
        if (cRBar.AttackingPokemon != null)
        {
            if (cRBar.AttackingPokemon == playerUnit.Pokemon)
            {
                ActionSelection();
            }     
            else if (cRBar.AttackingPokemon == enemyUnit.Pokemon)
                state = BattleState.EnemyTurn;
        }

        yield return null;
    }

    public IEnumerator RunTurnPlayer(BattleAction playerAction)
    {
        cRBar.PauseUnits();
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon = cRBar.AttackingPokemon;
            enemyUnit.Pokemon = cRBar.DefendingPokemon;

            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];

            //take turn
            yield return RunMove(playerUnit, enemyUnit, playerUnit.Pokemon.CurrentMove);
            yield return RunAfterTurns(playerUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        else if (playerAction == BattleAction.Boost)
        {
            playerUnit.Pokemon.CurrentBoost = playerUnit.Pokemon.Boosts[currentBoost];


            yield return RunBoost(playerUnit, playerUnit.Pokemon.CurrentBoost);
            yield return RunAfterTurns(playerUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                //skips player turn and goes to enemy turn
                dialogueBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToRun();
            }

            if (state == BattleState.BattleOver) yield break;
        }

        if (Field.Weather != null)
        {
            ConditionsID prevWeather = Field.Weather.Id;

            yield return dialogueBox.TypeDialogue(Field.Weather.EffectMessage);
            Field.Weather.OnWeather?.Invoke(playerUnit.Pokemon);

            if (Field.Weather.Id == ConditionsID.sandstorm)
            {
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ground || playerUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        playerUnit.Pokemon.Base.Type1 == PokemonType.Steel || playerUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        playerUnit.Pokemon.Base.Type2 == PokemonType.Rock || playerUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    yield break;
                }
                else
                {
                    yield return ShowStatusChanges(playerUnit.Pokemon);
                    int prevHP = playerUnit.Pokemon.HP;
                    if (playerUnit.Pokemon.HP != prevHP) playerUnit.PlayHitAnimation();
                    playerUnit.Hud.UpdateHP();
                    if (playerUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} fainted");
                        playerUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(playerUnit);
                        yield break;
                    }
                }
            }
            if (Field.Weather.Id == ConditionsID.veil)
            {
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ice || playerUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                {
                    yield break;
                }
                else
                {
                    yield return ShowStatusChanges(playerUnit.Pokemon);
                    int prevHP = playerUnit.Pokemon.HP;
                    if (playerUnit.Pokemon.HP != prevHP) playerUnit.PlayHitAnimation();
                    playerUnit.Hud.UpdateHP();
                    if (playerUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} fainted");
                        playerUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(playerUnit);
                        yield break;
                    }
                }
            }
            if (Field.Weather.Id == ConditionsID.creatorsWill)
            {
                if(bg.gameObject.activeInHierarchy == true)
                {
                    bg.gameObject.SetActive(false);
                    arceusBg.gameObject.SetActive(true);
                }

                if (playerUnit.Pokemon.Base.Name != "Arceus")
                {
                    yield return ShowStatusChanges(playerUnit.Pokemon);
                    int prevHP = playerUnit.Pokemon.HP;
                    if (playerUnit.Pokemon.HP != prevHP) playerUnit.PlayHitAnimation();
                    playerUnit.Hud.UpdateHP();
                    if (playerUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} fainted");
                        playerUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(playerUnit);
                        yield break;
                    }
                }
            }      


            Field.WeatherDuration--;
            if (Field.WeatherDuration == 0)
            {
                Field.Weather = null;

                if (prevWeather != ConditionsID.creatorsWill)
                    yield return dialogueBox.TypeDialogue($"The atmosphere went back to normal");
                else
                    yield return dialogueBox.TypeDialogue($"The weather has changed back to normal");


                //if weather was sandstorm
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ground || playerUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        playerUnit.Pokemon.Base.Type1 == PokemonType.Steel || playerUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        playerUnit.Pokemon.Base.Type2 == PokemonType.Rock || playerUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    if (prevWeather == ConditionsID.sandstorm)
                        playerUnit.Pokemon.ResetDefBoost();
                }

                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ground || enemyUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        enemyUnit.Pokemon.Base.Type1 == PokemonType.Steel || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        enemyUnit.Pokemon.Base.Type2 == PokemonType.Rock || enemyUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    if (prevWeather == ConditionsID.sandstorm)
                        enemyUnit.Pokemon.ResetDefBoost();
                }


                //if weather was veil

                if (prevWeather == ConditionsID.veil)
                {
                    if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ice || playerUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                    {
                        playerUnit.Pokemon.ResetDefBoost();
                        playerUnit.Pokemon.ResetOffBoost();
                    }
                }
                if (prevWeather == ConditionsID.veil)
                {
                    if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ice || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                    {
                        enemyUnit.Pokemon.ResetDefBoost();
                        enemyUnit.Pokemon.ResetOffBoost();
                    }
                }

                if(prevWeather == ConditionsID.creatorsWill)
                {
                    bg.gameObject.SetActive(true);
                    arceusBg.gameObject.SetActive(false);
                }
            }
        }


        cRBar.UnPauseUnits();
        state = BattleState.CRBar;
    }

    IEnumerator RunTurnEnemy()
    {
        cRBar.PauseUnits();
        state = BattleState.RunningTurn;

        enemyUnit.Pokemon = cRBar.AttackingPokemon;
        playerUnit.Pokemon = cRBar.DefendingPokemon;

        if(enemyUnit.Pokemon.Base.Evolutions.Length > 0 && didMegaEn != true)
        {
            if (UnityEngine.Random.Range(0, 101) <= megaChance)
            {
                yield return EvolutionManager.i.Evolve(enemyUnit.Pokemon, enemyUnit.Pokemon.Base.Evolutions[0]);
                didMegaEn = true;
                SetupMegaBattleEnemy();
            }
            else
                megaChance += 25;
        }


        enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetBestMove(playerUnit);


        //take turn
        yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.CurrentMove);
        yield return RunAfterTurns(enemyUnit);
        if (state == BattleState.BattleOver) yield break;

        if (Field.Weather != null)
        {
            ConditionsID prevWeather = Field.Weather.Id;

            yield return dialogueBox.TypeDialogue(Field.Weather.EffectMessage);
            Field.Weather.OnWeather?.Invoke(enemyUnit.Pokemon);

            if (Field.Weather.Id == ConditionsID.sandstorm)
            {
                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ground || enemyUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        enemyUnit.Pokemon.Base.Type1 == PokemonType.Steel || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        enemyUnit.Pokemon.Base.Type2 == PokemonType.Rock || enemyUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    yield break;
                }
                else
                {
                    int prevHPEn = enemyUnit.Pokemon.HP;
                    yield return ShowStatusChanges(enemyUnit.Pokemon);
                    if (enemyUnit.Pokemon.HP != prevHPEn) playerUnit.PlayHitAnimation();
                    enemyUnit.Hud.UpdateHP();
                    if (enemyUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.Base.Name} fainted");
                        enemyUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(enemyUnit);
                        yield break;
                    }
                }
            }
            if (Field.Weather.Id == ConditionsID.veil)
            {
                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ice || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                {
                    yield break;
                }
                else
                {
                    int prevHPEn = enemyUnit.Pokemon.HP;
                    yield return ShowStatusChanges(enemyUnit.Pokemon);
                    if (enemyUnit.Pokemon.HP != prevHPEn) playerUnit.PlayHitAnimation();
                    enemyUnit.Hud.UpdateHP();
                    if (enemyUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.Base.Name} fainted");
                        enemyUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(enemyUnit);
                        yield break;
                    }
                }
            }
            if (Field.Weather.Id == ConditionsID.creatorsWill)
            {
                if (bg.gameObject.activeInHierarchy == true)
                {
                    bg.gameObject.SetActive(false);
                    arceusBg.gameObject.SetActive(true);
                }

                if (enemyUnit.Pokemon.Base.Name != "Arceus")
                {
                    yield return ShowStatusChanges(enemyUnit.Pokemon);
                    int prevHP = enemyUnit.Pokemon.HP;
                    if (enemyUnit.Pokemon.HP != prevHP) enemyUnit.PlayHitAnimation();
                    enemyUnit.Hud.UpdateHP();
                    if (enemyUnit.Pokemon.HP <= 0)
                    {
                        yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.Base.Name} fainted");
                        enemyUnit.PlayFaintAnimation();
                        yield return new WaitForSeconds(1.5f);

                        CheckForBattleOver(enemyUnit);
                        yield break;
                    }
                }
            }
        }

        cRBar.UnPauseUnits();
        state = BattleState.CRBar;

        yield return null;
    }
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if(playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetBestMove(playerUnit);

            bool playerGoesFirst = true;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            //first turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurns(firstUnit);
            if (state == BattleState.BattleOver) yield break;
            
            if (secondPokemon.HP > 0)
            {
                //second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurns(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }         
        }
        else if (playerAction == BattleAction.Boost)
        {
            playerUnit.Pokemon.CurrentBoost = playerUnit.Pokemon.Boosts[currentBoost];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetBestMove(playerUnit);

            int playerBoostPriority = playerUnit.Pokemon.CurrentBoost.BBase.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerBoostPriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerBoostPriority)
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            //first turn
            if (firstUnit == playerUnit)
            {
                yield return RunBoost(firstUnit, firstUnit.Pokemon.CurrentBoost);
                yield return RunAfterTurns(firstUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            else
            {
                yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
                yield return RunAfterTurns(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }


            if (secondPokemon.HP > 0)
            {
                //second turn
                if (secondUnit == playerUnit)
                {
                    yield return RunBoost(secondUnit, secondUnit.Pokemon.CurrentBoost);
                    yield return RunAfterTurns(secondUnit);
                    if (state == BattleState.BattleOver) yield break;
                }
                else
                {
                    yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                    yield return RunAfterTurns(secondUnit);
                    if (state == BattleState.BattleOver) yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if(playerAction == BattleAction.UseItem)
            {
                //skips player turn and goes to enemy turn
                dialogueBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToRun();
            }

            //enemy turn
            var enemyMove = enemyUnit.Pokemon.GetBestMove(playerUnit);
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurns(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if(Field.Weather != null)
        {
            ConditionsID prevWeather = Field.Weather.Id;

            yield return dialogueBox.TypeDialogue(Field.Weather.EffectMessage);        
            Field.Weather.OnWeather?.Invoke(playerUnit.Pokemon);

            if (Field.Weather.Id == ConditionsID.sandstorm)
            {
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ground || playerUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        playerUnit.Pokemon.Base.Type1 == PokemonType.Steel || playerUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        playerUnit.Pokemon.Base.Type2 == PokemonType.Rock || playerUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    yield break;
                }
            }
            else
            {
                yield return ShowStatusChanges(playerUnit.Pokemon);
                int prevHP = playerUnit.Pokemon.HP;
                if (playerUnit.Pokemon.HP != prevHP) playerUnit.PlayHitAnimation();
                playerUnit.Hud.UpdateHP();
                if (playerUnit.Pokemon.HP <= 0)
                {
                    yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} fainted");
                    playerUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(1.5f);

                    CheckForBattleOver(playerUnit);
                    yield break;
                }
            }

            if (Field.Weather.Id == ConditionsID.sandstorm)
            {
                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ground || enemyUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        enemyUnit.Pokemon.Base.Type1 == PokemonType.Steel || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        enemyUnit.Pokemon.Base.Type2 == PokemonType.Rock || enemyUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    yield break;
                }
            }
            else
            {
                int prevHPEn = enemyUnit.Pokemon.HP;
                yield return ShowStatusChanges(enemyUnit.Pokemon);
                if (enemyUnit.Pokemon.HP != prevHPEn) playerUnit.PlayHitAnimation();
                enemyUnit.Hud.UpdateHP();
                if (enemyUnit.Pokemon.HP <= 0)
                {
                    yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.Base.Name} fainted");
                    enemyUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(1.5f);

                    CheckForBattleOver(enemyUnit);
                    yield break;
                }
            }

            if (Field.Weather.Id == ConditionsID.veil)
            {
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ice || playerUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                {
                    yield break;
                }
            }
            else
            {
                yield return ShowStatusChanges(playerUnit.Pokemon);
                int prevHP = playerUnit.Pokemon.HP;
                if (playerUnit.Pokemon.HP != prevHP) playerUnit.PlayHitAnimation();
                playerUnit.Hud.UpdateHP();
                if (playerUnit.Pokemon.HP <= 0)
                {
                    yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} fainted");
                    playerUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(1.5f);

                    CheckForBattleOver(playerUnit);
                    yield break;
                }
            }

            if (Field.Weather.Id == ConditionsID.sandstorm)
            {
                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ice || enemyUnit.Pokemon.Base.Type1 == PokemonType.Ice)
                {
                    yield break;
                }
            }
            else
            {
                int prevHPEn = enemyUnit.Pokemon.HP;
                yield return ShowStatusChanges(enemyUnit.Pokemon);
                if (enemyUnit.Pokemon.HP != prevHPEn) playerUnit.PlayHitAnimation();
                enemyUnit.Hud.UpdateHP();
                if (enemyUnit.Pokemon.HP <= 0)
                {
                    yield return dialogueBox.TypeDialogue($"{enemyUnit.Pokemon.Base.Name} fainted");
                    enemyUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(1.5f);

                    CheckForBattleOver(enemyUnit);
                    yield break;
                }
            }

            Field.WeatherDuration--;
            if(Field.WeatherDuration == 0)
            {
                Field.Weather = null;
                yield return dialogueBox.TypeDialogue($"The weather has changed back to normal");

                //if weather was sandstorm
                if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ground || playerUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        playerUnit.Pokemon.Base.Type1 == PokemonType.Steel || playerUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        playerUnit.Pokemon.Base.Type2 == PokemonType.Rock || playerUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    if (prevWeather == ConditionsID.sandstorm)
                        playerUnit.Pokemon.ResetDefBoost();
                }

                if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ground || enemyUnit.Pokemon.Base.Type1 == PokemonType.Rock ||
                        enemyUnit.Pokemon.Base.Type1 == PokemonType.Steel || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ground ||
                        enemyUnit.Pokemon.Base.Type2 == PokemonType.Rock || enemyUnit.Pokemon.Base.Type2 == PokemonType.Steel)
                {
                    if (prevWeather == ConditionsID.sandstorm)
                        enemyUnit.Pokemon.ResetDefBoost();
                }


                //if weather was veil

                if (prevWeather == ConditionsID.veil)
                {
                    if (playerUnit.Pokemon.Base.Type1 == PokemonType.Ice || playerUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                    {
                        playerUnit.Pokemon.ResetDefBoost();
                        playerUnit.Pokemon.ResetOffBoost();
                    }
                }
                if (prevWeather == ConditionsID.veil)
                {
                    if (enemyUnit.Pokemon.Base.Type1 == PokemonType.Ice || enemyUnit.Pokemon.Base.Type2 == PokemonType.Ice)
                    {
                        enemyUnit.Pokemon.ResetDefBoost();
                        enemyUnit.Pokemon.ResetOffBoost();
                    }
                }
            }
        }

        //if (state != BattleState.BattleOver)
            //ActionSelection();
    } 
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //determines whether the pokemon can move if paralyzed
        bool canRunMove = sourceUnit.Pokemon.OnBeforeTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogueBox.TypeDialogue($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySfx(move.Base.Sound);
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            AudioManager.i.PlaySfx(AudioId.Hit);

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                if(move.Base.Power > 0 && targetUnit.Pokemon.IsGuarding)
                {
                    yield return dialogueBox.TypeDialogue($"{targetUnit.Pokemon.Base.Name} guarded against the attack!");
                    targetUnit.Pokemon.isGuarding = false;
                }
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon, Field.Weather);
                yield return targetUnit.PlayTypeAnimation(move.Base.Type);
                yield return targetUnit.Hud.WaitForHPUpdate();               
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondaryEffects in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondaryEffects.Chance)
                        yield return RunMoveEffects(secondaryEffects, sourceUnit.Pokemon, targetUnit.Pokemon, secondaryEffects.Target);
                }
            }

            if (targetUnit.Pokemon == playerUnit.Pokemon)
                curHP.text = targetUnit.Pokemon.HP.ToString();

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue(missMessage);
        }        
    }
    IEnumerator RunBoost(BattleUnit sourceUnit, Boost boost)
    {
        boost.PP--;
        yield return dialogueBox.TypeDialogue($"{sourceUnit.Pokemon.Base.Name} used {boost.BBase.Name}");

        if (boost.BBase.Category == BoostCategory.bStatus)
        {
            yield return RunBoostEffects(boost.BBase.Effects, sourceUnit.Pokemon, boost.BBase.Target);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, PokemonInfo source, PokemonInfo target, MoveTarget moveTarget)
    {
        //stat boosts
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        //status conditions
        if (effects.Status != ConditionsID.none)
        {
            target.SetStatus(effects.Status);
        }
       
        //volatile status conditions
        if (effects.VolatileStatus != ConditionsID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        if (effects.Weather != ConditionsID.none)
        {
            Field.SetWeather(effects.Weather);
            Field.WeatherDuration = 5;
            yield return dialogueBox.TypeDialogue(Field.Weather.StartMessage);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
       
    }
    IEnumerator RunBoostEffects(BoostEffects effects, PokemonInfo source, BoostTarget boostTarget)
    {
        //stat boosts
        if (effects.ABoosts != null)
        {
            if (boostTarget == BoostTarget.bSelf)
                source.ApplyBoosts(effects.ABoosts);
        }
        yield return ShowStatusChanges(source);

    }


    IEnumerator RunAfterTurns(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Statuses like burn or weather will hurt the pokemon after the turn
        //Also when build up abilities will occur
        sourceUnit.Pokemon.OnAfterTurn();

        sourceUnit.Pokemon.BuildUp();



        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }
    bool CheckIfMoveHits(Move move, PokemonInfo source, PokemonInfo target)
    {
        if (move.Base.AlwaysHits)
            return true;





        int dodgeChance = target.Base.DodgeChance;




        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoost[Stat.Accuracy];
        int evasion = target.StatBoost[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];





        if(dodgeChance > 0)
        {
            if(UnityEngine.Random.Range(1, 101) <= moveAccuracy)
            {
                missMessage = $"{target.Base.Name} dodged the attack!";
                return UnityEngine.Random.Range(1, 101) >= dodgeChance;
            }
        }

        missMessage = $"{source.Base.Name}'s attack missed";
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(PokemonInfo pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(message);
        }
    }
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
            {
                didFail = true;
                BattleOver(false);
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    //send out next pokemon
                    StartCoroutine(NewPokemon(nextPokemon));
                else
                {
                    BattleOver(true);
                }              
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogueBox.TypeDialogue("It was a critical hit!");
        else if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogueBox.TypeDialogue("It was super effective!");
        else if (damageDetails.TypeEffectiveness < .75f && damageDetails.TypeEffectiveness > .1f)
            yield return dialogueBox.TypeDialogue("It was not very effective...");
        else if (damageDetails.TypeEffectiveness == 0f)
            yield return dialogueBox.TypeDialogue("It had no effect");
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            //cRBar.Tranparent();
            HandlePartySelection();
        }
        else if (state == BattleState.Bag)
        {
            //cRBar.Tranparent();
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                state = BattleState.Busy;
                inventoryUI.gameObject.SetActive(false);
                //StartCoroutine(RunTurns(BattleAction.UseItem));
                StartCoroutine(RunTurnPlayer(BattleAction.UseItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.ABoostSelection)
        {
            HandleABoostSelection();
        }
        else if (state == BattleState.AboutToUsePokemon)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveForgetSelection.gameObject.SetActive(false);
                if (moveIndex == 4)
                {
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} did not learn {moveToLearn.Name}"));
                }
                else
                {
                    var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name} instead."));

                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            moveForgetSelection.HandleMoveSelection(onMoveSelected);
        }
        else if (state == BattleState.EnemyTurn)
        {
            HandleEnemyTurn();
        }
        else if (state == BattleState.CRBar)
        {
            StartCoroutine(RunCRBar());
        }
        else if (state == BattleState.Summary)
        {
            HandleSumOpt();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 4);

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //bag
                OpenBag();
            }
            else if (currentAction == 2)
            {
                //party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                BoostSelection();
            }
            else if (currentAction == 4)
            {
                //run
                StartCoroutine(RunTurnPlayer(BattleAction.Run));
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;

            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurnPlayer(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }
    void HandleABoostSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentBoost;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentBoost;

        currentBoost = Mathf.Clamp(currentBoost, 0, playerUnit.Pokemon.Boosts.Count - 1);

        dialogueBox.UpdateBoostSelection(currentBoost, playerUnit.Pokemon.Boosts[currentBoost]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var boostmove = playerUnit.Pokemon.Boosts[currentBoost];
            if (boostmove.PP == 0) return;

            dialogueBox.EnableBoostSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurnPlayer(BattleAction.Boost));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableBoostSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            /*var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon.");
                return;
            }

            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("That pokemon is already out!");
                return;
            }*/

            //partyScreen.gameObject.SetActive(false);

            /*if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                partyScreen.CalledFrom = null;
                StartCoroutine(RunTurnPlayer(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUsePokemon;
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }*/

            state = BattleState.Summary;

            //partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUsePokemon)
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();

            partyScreen.CalledFrom = null;
        };

        partyScreen.HandleUpdate(onSelected, onBack);
    }

    void HandleSumOpt()
    {
        summaryOpt.gameObject.SetActive(true);
        summaryOpt.HandleUpdate();
        var selectedMember = partyScreen.SelectedMember;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (selectedMember.HP <= 0 && summaryOpt.SelectedItem == 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon.");
                return;
            }
            if (selectedMember == playerUnit.Pokemon && summaryOpt.SelectedItem == 0)
            {
                partyScreen.SetMessageText("That pokemon is already out!");
                return;
            }
            else if (selectedMember.HP > 0 && summaryOpt.SelectedItem == 0 && partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                summaryOpt.gameObject.SetActive(false);
                partyScreen.gameObject.SetActive(false);
                partyScreen.CalledFrom = null;
                StartCoroutine(RunTurnPlayer(BattleAction.SwitchPokemon));
            }
            else if(partyScreen.CalledFrom != BattleState.ActionSelection)
            {
                summaryOpt.gameObject.SetActive(false);
                partyScreen.gameObject.SetActive(false);
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUsePokemon;
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
            
            if (summaryOpt.SelectedItem == 1)
            {
                GameController.Instance.pokemonSummary.gameObject.SetActive(true);
                GameController.Instance.pokemonSummary.ShowSummary(partyScreen.SelectedMember);               
            }

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            summaryOpt.gameObject.SetActive(false);
            GameController.Instance.pokemonSummary.gameObject.SetActive(false);
            state = BattleState.PartyScreen;
        }

        /*if (partyScreen.CalledFrom == BattleState.ActionSelection)
        {
            partyScreen.CalledFrom = null;
            StartCoroutine(RunTurnPlayer(BattleAction.SwitchPokemon));
        }
        else
        {
            state = BattleState.Busy;
            bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUsePokemon;
            StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
        }*/    
    }
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogueBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceBox(false);
            if(aboutToUseChoice == true)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    void HandleEnemyTurn()
    {
        StartCoroutine(RunTurnEnemy());
    }


    IEnumerator SwitchPokemon(PokemonInfo newPokemon, bool isTrainerAboutToUse=false)
    {
        if(playerUnit.Pokemon.HP > 0)
        {         
            yield return dialogueBox.TypeDialogue($"Come back {playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }

        activePokemon[1] = null;

        playerUnit.Setup(newPokemon);

        dialogueBox.SetMoveNames(newPokemon.Moves);
        dialogueBox.SetBoostNames(newPokemon.Boosts);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.Name}!");

        if (playerUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
        {
            Field.SetWeather(playerUnit.Pokemon.Base.FieldEffect);
            WeatherToPlay(playerUnit.Pokemon.Base.FieldEffect);
            yield return new WaitForSeconds(1);
            yield return dialogueBox.TypeDialogue(Field.Weather.StartMessage);
        }

        cRBar.UpdateList(newPokemon, 1);
        activePokemon[1] = newPokemon;

        yield return new WaitForSeconds(1f);


        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerPokemon());
        else
        {
            state = BattleState.RunningTurn;
        }  
    }

    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();

        activePokemon[0] = null;


        enemyUnit.Setup(nextPokemon);

        yield return dialogueBox.TypeDialogue($"{trainer.Name} sent out {nextPokemon.Base.Name}!");

        if (enemyUnit.Pokemon.Base.FieldEffect != ConditionsID.none)
        {
            Field.SetWeather(enemyUnit.Pokemon.Base.FieldEffect);
            WeatherToPlay(enemyUnit.Pokemon.Base.FieldEffect);
            yield return new WaitForSeconds(1);
            yield return dialogueBox.TypeDialogue(Field.Weather.StartMessage);
        }

        cRBar.UpdateList(nextPokemon, 0);

        activePokemon[0] = nextPokemon;
        state = BattleState.RunningTurn;
    }
    
    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogueBox.TypeDialogue($"{faintedUnit.Pokemon.Base.Name} fainted");
        faintedUnit.PlayFaintAnimation();     

        if(!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if(isTrainerBattle)
            {
                battleWon = trainerParty.GetHealthyPokemon() == null;
            }

            if(battleWon)
                AudioManager.i.PlayMusic(battleVictoryMusic);


            //gain exp
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} gained {expGain} experience.");
            yield return playerUnit.Hud.SetExpSmooth(); 

            //check lvl up
            while (playerUnit.Pokemon.CheckForLvlUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}.");

                //try to learn new move
                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrLevel();
                if(newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < 4)
                    {
                        playerUnit.Pokemon.LearnMove(newMove.Base);
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}.");
                        dialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        //forget move
                        yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} is trying to learn {newMove.Base.Name}.");
                        yield return dialogueBox.TypeDialogue($"However, it can't learn move than 4 moves.");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveForget);
                        yield return new WaitForSeconds(2f);
                    }
                }
                yield return playerUnit.Hud.SetExpSmooth(true);
            }
        }
        yield return new WaitForSeconds(1f);
        CheckForBattleOver(faintedUnit);
    }
    IEnumerator TryToRun()
    {
        yield return dialogueBox.TypeDialogue($"You can't run from trainer battles!");
    }

    void WeatherToPlay(ConditionsID weather)
    {
        switch(weather)
        {
            case ConditionsID.sunny:
                weatherfxs[0].SetActive(true);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.desLand:
                weatherfxs[0].SetActive(true);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.rain:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(true);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.primSea:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(true);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.deltaStream:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(true);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.veil:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(true);
                weatherfxs[4].SetActive(false);
                break;
            case ConditionsID.sandstorm:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(true);
                break;
            default:
                weatherfxs[0].SetActive(false);
                weatherfxs[1].SetActive(false);
                weatherfxs[2].SetActive(false);
                weatherfxs[3].SetActive(false);
                weatherfxs[4].SetActive(false);
                break;
        }
    }
    void ResetWeather()
    {
        for (int i = 0; i < weatherfxs.Length; i++)
        {
            weatherfxs[i].SetActive(false);
        }
    }
}
