using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class PokemonInfo
{
    [SerializeField] PokemonBase _base;
    [SerializeField] MovesBase[] enemyList;
    [SerializeField] int level;

    public int count = 0;
    public bool isGuarding = false;


    public PokemonInfo(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        Init();
    }

    public PokemonBase Base
    {
        get
        {
            return _base;
        }
    }
           
    public int Level
    {
        get
        {
            return level;
        }
    }

    public bool IsGuarding
    {
        get
        {
            return isGuarding;
        }
    }
    public MovesBase[] EnemyList
    {
        get
        {
            return enemyList;
        }
    }
    public int HP { get; set; }
    public int Exp { get; set; }
    public List<Move> Moves { get; set; }
    public List<Boost> Boosts { get; set; }
    public Move CurrentMove { get; set; }
    public Boost CurrentBoost { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoost { get; private set; }
    public Conditions Status { get; private set; }
    public Conditions VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }

    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public Queue<string> StatusChanges { get; private set; }
    public void Init()
    {
        HP = MaxHp;

        // Generates Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count > 4)
                break;
        }
        // Generates Boosts
        Boosts = new List<Boost>();
        foreach (var boost in Base.LearnableBoosts)
        {
            if (boost.BLevel <= level)
                Boosts.Add(new Boost(boost.BBase));

            if (Boosts.Count > 2)
                break;
        }

        Exp = Base.GetExpForLvl(Level);

        count = 0;
        CalculateStats();

        HP = MaxHp;
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public List<string> GetMoves()
    {
        List<string> moveList = new List<string>();

        foreach (var moveL in Base.LearnableMoves)
        {
            foreach(var move in Moves)
            {
                if (move.Base.Name == moveL.Base.Name && !moveList.Contains(move.Base.Name))
                    moveList.Add(move.Base.Name);
            }

            if (Moves.Count > 4)
                break;
        }

        foreach (var moveTM in Base.LearnableMovesByTms)
        {
            foreach (var move in Moves)
            {
                if (move.Base.Name == moveTM.Name && !moveList.Contains(move.Base.Name))
                    moveList.Add(move.Base.Name);
            }

            if (Moves.Count > 4)
                break;
        }

        return moveList;
    }

    public PokemonInfo(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetObjectByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;

        if (saveData.status != null)
            Status = ConditionsDB.Conditions[saveData.status.Value];
        else
            Status = null;

        Moves = saveData.moves.Select(s => new Move(s)).ToList();
        Boosts = saveData.boosts.Select(s => new Boost(s)).ToList();

        count = 0;
        CalculateStats();

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            name = Base.name,
            hp = HP,
            level = Level,
            exp = Exp,
            status = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList(),
            boosts = Boosts.Select(b => b.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void CalculateStats()
    {
        var bAtk = this.Base.BuildUpAmntAtk;
        var bDef = this.Base.BuildUpAmntDef;
        var bSpA = this.Base.BuildUpAmntSpAtk;
        var bSpDef = this.Base.BuildUpAmntSpDef;
        var bSpd = this.Base.BuildUpAmntSpd;

        List<double> statsBoost = new List<double>() { bAtk, bDef, bSpA, bSpDef, bSpd };

        for (int i = 0; i < statsBoost.Count; i++)
        {
            if (statsBoost[i] > 0)
            {
                statsBoost[i] *= .1;
                if (statsBoost[i] < 1)
                    statsBoost[i] = 1;
            }
            else
                statsBoost[i] = 0;
        }

        /*if(count == 0)
        {
            Stats = new Dictionary<Stat, int>();
            Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);
            Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * level) / 100f) + 5);
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * level) / 100f) + 5);
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * level) / 100f) + 5);
            Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * level) / 100f) + 5);
        }
        else
        {
            Stats = new Dictionary<Stat, int>();
            Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5 + (int)statsBoost[0]);
            Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * level) / 100f) + 5 + (int)statsBoost[1]);
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * level) / 100f) + 5 + (int)statsBoost[2]);
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * level) / 100f) + 5 + (int)statsBoost[3]);
            Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * level) / 100f) + 5 + (int)statsBoost[4]);
        }*/

        if (count == 0)
        {
            Stats = new Dictionary<Stat, int>();
            var tempatk = 2 * Base.Attack + 31 + (252 / 4);
            Stats.Add(Stat.Attack, Mathf.FloorToInt((tempatk * level) / 100) + 5 + (int)statsBoost[0]);
            var tempdef = 2 * Base.Defense + 31 + (252 / 4);
            Stats.Add(Stat.Defense, Mathf.FloorToInt((tempdef * level) / 100) + 5 + (int)statsBoost[1]);
            var tempspatk = 2 * Base.SpAttack + 31 + (252 / 4);
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((tempspatk * level) / 100) + 5 + (int)statsBoost[2]);
            var tempspdef = 2 * Base.SpDefense + 31 + (252 / 4);
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((tempspdef * level) / 100) + 5 + (int)statsBoost[3]);
            var tempspd = 2 * Base.Speed + 31 + (252 / 4);
            Stats.Add(Stat.Speed, Mathf.FloorToInt((tempspd * level) / 100) + 5 + (int)statsBoost[4]);
        }
        else
        {
            Stats = new Dictionary<Stat, int>();
            var tempatk = 2 * Base.Attack + 31 + (252 / 4);
            Stats.Add(Stat.Attack, Mathf.FloorToInt((tempatk * level) / 100) + 5);
            var tempdef = 2 * Base.Defense + 31 + (252 / 4);
            Stats.Add(Stat.Defense, Mathf.FloorToInt((tempdef * level) / 100) + 5);
            var tempspatk = 2 * Base.SpAttack + 31 + (252 / 4);
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((tempspatk * level) / 100) + 5);
            var tempspdef = 2 * Base.SpDefense + 31 + (252 / 4);
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((tempspdef * level) / 100) + 5);
            var tempspd = 2 * Base.Speed + 31 + (252 / 4);
            Stats.Add(Stat.Speed, Mathf.FloorToInt((tempspd * level) / 100) + 5);
        }


        int oldMaxHP = MaxHp;
        //MaxHp = Mathf.FloorToInt((Base.MaxHp * level) / 100f) + 10 + Level;
        var temphlth = 2 * Base.MaxHp + 31 + (252 / 4);
        MaxHp = Mathf.FloorToInt((temphlth * level) / 100) + Level + 10;

        if (oldMaxHP != 0)
            HP += MaxHp - oldMaxHP;
    }

    void ResetStatBoost()
    {
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defense, 0 },
            {Stat.SpAttack, 0 },
            {Stat.SpDefense, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 },
            {Stat.Speed, 0 }
        };
    }

    public void ResetDefBoost()
    {
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Defense, 0 },
            {Stat.SpDefense, 0 },
        };
    }
    public void ResetOffBoost()
    {
        StatBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.SpAttack, 0 },
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoost[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void BuildUp()
    {
        var bAtk = this.Base.BuildUpAmntAtk;
        var bDef = this.Base.BuildUpAmntDef;
        var bSpA = this.Base.BuildUpAmntSpAtk;
        var bSpDef = this.Base.BuildUpAmntSpDef;
        var bSpd = this.Base.BuildUpAmntSpd;

        var atk = Attack;
        var def = Defense;
        var spA = SpAttack;
        var spDef = SpDefense;
        var spd = Speed;

        List<int> stats = new List<int>() { atk, def, spA, spDef, spd };
        List<double> statsBoost = new List<double>(){bAtk, bDef, bSpA, bSpDef, bSpd };

        for (int i = 0; i < statsBoost.Count; i++)
        {
            if (statsBoost[i] > 0)
            {
                statsBoost[i] *= .1;
                if(statsBoost[i] < 1)
                {
                    statsBoost[i] = 1;
                }

                stats[i] += (int)statsBoost[i];
            }
        }

        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, stats[0]);
        Stats.Add(Stat.Defense, stats[1]);
        Stats.Add(Stat.SpAttack, stats[2]);
        Stats.Add(Stat.SpDefense, stats[3]);
        Stats.Add(Stat.Speed, stats[4]);
    }


    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoost[stat] =  Mathf.Clamp(StatBoost[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose.");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell.");

            Debug.Log($"{stat} has been changed to {StatBoost[stat]}");
        }
    }
    public bool CheckForLvlUp()
    {
        if(Exp > Base.GetExpForLvl(level + 1))
        {
            level++;
            count = 0;
            CalculateStats();
            return true;
        }

        return false;
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.EvolveLvl <= level);
    }
    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.EvolveItem == item);
    }

    public void Evolve(Evolution evolution) 
    {
        _base = evolution.EvolvesInto;
        count = 0;
        CalculateStats();
    }
    public void Revert()
    {
        _base = _base.evolutions[0].EvolvesFrom;
        count = 0;
        CalculateStats();
    }

    public void Heal()
    {
        HP = MaxHp;

        foreach(var move in Moves)
        {
            move.IncreasePP(move.Base.PP);
        }
        foreach (var boost in Boosts)
        {
            boost.IncreasePP(boost.BBase.PP);
        }

        CureStatus();
        //OnStatusChanged?.Invoke();
        OnHPChanged?.Invoke();
    }

    public LearnableMoves GetLearnableMoveAtCurrLevel()
    {
       return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }
    public void LearnMove(MovesBase moveToLearn)
    {
        if (Moves.Count > 4)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MovesBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp { get; private set; }
  

    public DamageDetails TakeDamage(Move move, PokemonInfo attacker, Conditions weather)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;



        double damRdn = Base.DmgReductionAmnt;
        if (damRdn <= 0)
            damRdn = 1;
        else
            damRdn *= .01;



        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        float weatherMod = weather?.OnDamageModify?.Invoke(this, attacker, move) ?? 1f;




        if(Guard())
        {
            isGuarding = true;
            type = .5f;
        }




        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1.15f) * type * critical * weatherMod;
        float a = (2 * attacker.level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        double dmgToSub;

        if (damRdn > 1)
            dmgToSub = damage * damRdn;         
        else
            dmgToSub = 0;

       

        double newDmg = damage - dmgToSub;
        Debug.Log(newDmg);

        DecreaseHP((int)newDmg);

        return damageDetails;
    }
    public int TakeDamageTest(Move move, PokemonInfo attacker, Conditions weather=null)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;



        double damRdn = Base.DmgReductionAmnt;
        if (damRdn <= 0)
            damRdn = 1;
        else
            damRdn *= .01;



        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        float weatherMod = weather?.OnDamageModify?.Invoke(this, attacker, move) ?? 1f;




        if (Guard())
        {
            isGuarding = true;
            type = .5f;
        }




        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1.15f) * type * critical * weatherMod;
        float a = (2 * attacker.level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);


        double dmgToSub;

        if (damRdn > 1)
            dmgToSub = damage * damRdn;
        else
            dmgToSub = 0;

        double newDmg = damage - dmgToSub;

        return (int)newDmg;
    }
    public bool Guard()
    {
        return Random.Range(1, 101) <= this.Base.GuardChance;
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void SetStatus(ConditionsID conditionsId)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionsId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    public void SetVolatileStatus(ConditionsID conditionsId)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionsId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");      
    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;    
    }

    public Move GetBestMove(BattleUnit targetPok)
    {
        Move chosenMove = null;
        int mostDmg = 0;
        var movesWithPP = EnemyList.Where(x => x.PP > 0).ToList();

        bool damMove = Random.Range(0, 100) >= 25;

        List<Move> killingMoves = new List<Move>();
        List<Move> statusMoves = new List<Move>();

        bool hasStatusMove = false;

        foreach (var move in movesWithPP)
        {
            if (move.Power == 0 || move.Category == MoveCategory.Status)
                hasStatusMove = true;
        }

        if (hasStatusMove == false || damMove)
        {
            foreach (var move in movesWithPP)
            {
                if (move.Power > 0)
                {
                    Move tempMove = new Move(move);
                    int moveDam = targetPok.Pokemon.TakeDamageTest(tempMove, this);

                    if (moveDam > targetPok.Pokemon.HP)
                    {
                        killingMoves.Add(tempMove);
                    }
                    else
                    {
                        if(moveDam > mostDmg)
                        {
                            mostDmg = moveDam;
                            chosenMove = tempMove;
                        }
                    }
                }
            }

            if (killingMoves.Count > 0)
            {
                var chosenDamageMove = Random.Range(0, killingMoves.Count);
                chosenMove = killingMoves[chosenDamageMove];
            }
            else
                return chosenMove;
        }
        else
        {
            foreach (var move in movesWithPP)
            {
                if (move.Category == MoveCategory.Status || move.Power == 0)
                {
                    Move tempMove = new Move(move);
                    statusMoves.Add(tempMove);
                }
            }

            var chosenStatusMove = Random.Range(0, statusMoves.Count);
            chosenMove = statusMoves[chosenStatusMove];
        }

        return chosenMove;
    }

    public bool OnBeforeTurn()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeTurn != null)
        {
            if (Status.OnBeforeTurn(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeTurn != null)
        {
            if (VolatileStatus.OnBeforeTurn(this))
                canPerformMove = false;
        }
        return canPerformMove;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();

        if (this.Base.isMega == true)
            Revert();
    }
}
 public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable] 
public class PokemonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionsID? status;
    public List<MoveSaveData> moves;
    public List<BoostSaveData> boosts;
}