using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABoost 
{
    public ABoostBase BoostBase { get; set; }
    public int bPP { get; set; }

    public ABoost(ABoostBase bBase)
    {
        BoostBase = bBase;
        bPP = bBase.PP;
    }

    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

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

    public int HP { get; set; }

    public List<ABoost> Boosts { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoost { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public void Init()
    {
        HP = MaxHp;

        CalculateStats();

        HP = MaxHp;

        ResetStatBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Attack * level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * level) / 100f) + 10 + Level;
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

    public void ApplyBoosts(List<AB> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.bstat;
            var boost = statBoost.bboost;

            StatBoost[stat] = Mathf.Clamp(StatBoost[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose.");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell.");

            Debug.Log($"{stat} has been changed to {StatBoost[stat]}");
        }
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

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

