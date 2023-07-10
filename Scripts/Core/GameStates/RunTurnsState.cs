using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTurnsState : State<BattleSystem>
{
    public static RunTurnsState i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    BattleDialogueBox dialogueBox;
    PartyScreen partyScreen;
    bool isTrainerBattle;
    PokemonParty playerParty;
    PokemonParty trainerParty;

    BattleSystem bs;
   /* public override void Enter(BattleSystem owner)
    {
        bs = owner;

        playerUnit = bs.PlayerUnit;
        enemyUnit = bs.EnemyUnit;
        dialogueBox = bs.DialogueBox;
        partyScreen = bs.PartyScreen;
        isTrainerBattle = bs.IsTrainerBattle;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;

        StartCoroutine(RunTurns(bs.SelectedAction));
    }
    public IEnumerator RunTurnPlayer(BattleAction playerAction)
    {
        bs.Stat
        bs.CRBar.PauseUnits();

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon = bs.CRBar.AttackingPokemon;
            enemyUnit.Pokemon = bs.CRBar.DefendingPokemon;

            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[bs.SelectedMove];

            var currentUnit = bs.CRBar.AttackingPokemon;

            //take turn
            yield return RunMove(playerUnit, enemyUnit, currentUnit.CurrentMove);
            yield return RunAfterTurns(playerUnit);
            if (bs.IsBattleOver) yield break;
        }
        else if (playerAction == BattleAction.Boost)
        {
            playerUnit.Pokemon.CurrentBoost = playerUnit.Pokemon.Boosts[bs.SelectedBoost];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();


            yield return RunBoost(playerUnit, playerUnit.Pokemon.CurrentBoost);
            yield return RunAfterTurns(playerUnit);
            if (bs.IsBattleOver) yield break;
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = partyScreen.SelectedMember;
                //yield return SwitchPokemon(selectedPokemon);
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

            if (bs.IsBattleOver) yield break;
        }

        bs.CRBar.UnPauseUnits();
    }

    public IEnumerator RunTurnEnemy()
    {
        bs.CRBar.PauseUnits();
        state = BattleState.RunningTurn;

        enemyUnit.Pokemon = bs.CRBar.AttackingPokemon;
        playerUnit.Pokemon = bs.CRBar.DefendingPokemon;

        var currentUnit = bs.CRBar.AttackingPokemon;

        //take turn
        yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.GetRandomMove());
        yield return RunAfterTurns(enemyUnit);
        if (bs.IsBattleOver) yield break;

        yield return RunAfterTurns(enemyUnit);
        if (bs.IsBattleOver) yield break;

        bs.CRBar.UnPauseUnits();

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
                if (move.Base.Power > 0 && targetUnit.Pokemon.IsGuarding)
                {
                    yield return dialogueBox.TypeDialogue($"{targetUnit.Pokemon.Base.Name} guarded against the attack!");
                    targetUnit.Pokemon.isGuarding = false;
                }
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon, Field.Weather);
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

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogueBox.TypeDialogue(bs.missMessage);
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





        if (dodgeChance > 0)
        {
            if (UnityEngine.Random.Range(1, 101) <= moveAccuracy)
            {
                bs.missMessage = $"{target.Base.Name} dodged the attack!";
                return UnityEngine.Random.Range(1, 101) >= dodgeChance;
            }
        }

        bs.missMessage = $"{source.Base.Name}'s attack missed";
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
                bs.BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                bs.BattleOver(true);
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    //send out next pokemon
                    StartCoroutine(NewPokemon(nextPokemon));
                else
                {
                    bs.BattleOver(true);
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
        else if (damageDetails.TypeEffectiveness < 1f && damageDetails.TypeEffectiveness > .1f)
            yield return dialogueBox.TypeDialogue("It was not very effective...");
        else if (damageDetails.TypeEffectiveness == 0f)
            yield return dialogueBox.TypeDialogue("It had no effect");
    }

    public IEnumerator TryToRun()
    {
        yield return dialogueBox.TypeDialogue($"You can't run from trainer battles!");
    }*/
}
