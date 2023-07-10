using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    //describes the physical attributes of the pokemon
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base Stats
    [SerializeField] public int maxHp;
    [SerializeField] public int attack;
    [SerializeField] public int defense;
    [SerializeField] public int spAttack;
    [SerializeField] public int spDefense;
    [SerializeField] public int speed;

    //Base Changeable Stats
    [SerializeField] public int attackBase;
    [SerializeField] public int defenseBase;
    [SerializeField] public int spAttackBase;
    [SerializeField] public int spDefenseBase;
    [SerializeField] public int speedBase;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] int catchRate = 255;

    [SerializeField] LearnableMoves[] learnableMoves;
    [SerializeField] LearnableBoosts[] learnableBoosts;

    [SerializeField] MovesBase[] learnableMovesByTms;

    [SerializeField] public Evolution[] evolutions;


    [Header("PASSIVE ABILITIES")]
    [SerializeField] int dodgeChance;
    [SerializeField] int guardChance;
    [SerializeField] double dmgReductionAmnt;
    [SerializeField] double buildUpAmntAtk = 0;
    [SerializeField] double buildUpAmntDef = 0;
    [SerializeField] double buildUpAmntSpAtk = 0;
    [SerializeField] double buildUpAmntSpDef = 0;
    [SerializeField] double buildUpAmntSpd = 0;


    [SerializeField] ConditionsID fieldEffect;
    [SerializeField] public bool isMega;

    public int GetExpForLvl(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5; 
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level; 
        }
        else if (growthRate == GrowthRate.Medium)
        {
            return (level * level * level) + 5; 
        }
        else if (growthRate == GrowthRate.MediumSlow)
        {
            return 6 * (level * level * level) / 5 - 15 * (level * level) + 100 * level - 140; 
        }
        else if (growthRate == GrowthRate.Slow)
        {
            return 5 * (level * level * level) / 4; 
        }
        return -1;
    }

    public string GetName()
    {
        return name;
    }

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite Frontsprite
    {
        get { return frontSprite; }
    }

    public Sprite Backsprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public int AttackBase
    {
        get { return attackBase; }
    }

    public int DefenseBase
    {
        get { return defenseBase; }
    }

    public int SpAttackBase
    {
        get { return spAttackBase; }
    }

    public int SpDefenseBase
    {
        get { return spDefenseBase; }
    }

    public int SpeedBase
    {
        get { return speedBase; }
    }

    public int ExpYield
    {
        get { return expYield; }
    }
    public GrowthRate GrowthRate
    {
        get { return growthRate; }
    }

    public LearnableMoves[] LearnableMoves
    {
        get { return learnableMoves; }
    }
    public LearnableBoosts[] LearnableBoosts
    {
        get { return learnableBoosts; }
    }

    public MovesBase[] LearnableMovesByTms => learnableMovesByTms;
    public Evolution[] Evolutions => evolutions;
    public int DodgeChance => dodgeChance;
    public int GuardChance => guardChance;
    public double DmgReductionAmnt => dmgReductionAmnt;
    public double BuildUpAmntAtk => buildUpAmntAtk;
    public double BuildUpAmntDef => buildUpAmntDef;
    public double BuildUpAmntSpAtk => buildUpAmntSpAtk;
    public double BuildUpAmntSpDef => buildUpAmntSpDef;
    public double BuildUpAmntSpd => buildUpAmntSpd;
    public ConditionsID FieldEffect => fieldEffect;

    public int CatchRate
    {
        get { return catchRate; }
    }
}

[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MovesBase movesBase;
    [SerializeField] int level;

    public MovesBase Base
    {
        get { return movesBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

[System.Serializable]
public class LearnableBoosts
{
    [SerializeField] ABoostBase boostBase;
    [SerializeField] int blevel;

    public ABoostBase BBase
    {
        get { return boostBase; }
    }
    public int BLevel
    {
        get { return blevel; }
    }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] PokemonBase evolvesInto;
    [SerializeField] public PokemonBase evolvesFrom;
    [SerializeField] int evolveLvl;
    [SerializeField] EvoItem evolveItem;

    public PokemonBase EvolvesInto => evolvesInto;
    public PokemonBase EvolvesFrom => evolvesFrom;
    public EvoItem EvolveItem => evolveItem;
    public int EvolveLvl => evolveLvl;
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Psychic,
    Fighting,
    Grass,
    Ground,
    Rock,
    Flying,
    Bug,
    Dark,
    Ghost,
    Steel,
    Fairy,
    Dragon,
    Poison,
    Ice
}
public enum GrowthRate
{
    Fast, MediumFast, Medium, MediumSlow, Slow
}
public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
        //                     NOR  FIR WAT ELE PSY FIG GRA GRD ROC FLY BUG DAR GHO  STE FAI DRA  POI  ICE
       /*normal*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 1f, 1f, 1f, 0f, .5f, 1f, 1f, 1f, 1f },
       /*fire*/ new float[] { 1f, .5f, .5f, 1f, 1f, 1f, 1.5f, 1f, .5f, 1f, 1.5f, 1f, 1f, 1.5f, 1f, .5f, 1f, 1.5f  },
       /*water*/ new float[] { 1f, 1.5f, .5f, 1f, 1f, 1f, .5f, 1.5f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 1f, 1f },
       /*elec*/ new float[] { 1f, 1f, 1.5f, .5f, 1f, 1f, .5f, 0f, 1f, 1.5f, 1f, 1f,  1f, 1f,  1f, .5f, 1f, 1f },
       /*psy*/ new float[] { 1f, 1f, 1f,  1f,  .5f, 1.5f, 1f, 1f, 1f, 1f, 1f, 0f, 1f, .5f, 1f, 1f, 2f, 1f },
       /*fight*/ new float[] { 1.5f, 1f, 1f, 1f, .5f, 1f, 1f, 1f, 1.5f, .5f, .5f, 1.5f, 0f, 1.5f, .5f, 1f, .5f, 1.5f },
       /*grass*/ new float[] { 1f, .5f, 1.5f, 1f, 1f, 1f, .5f, 1.5f, 1.5f, .5f, .5f, 1f, 1f, .5f, 1f, .5f, .5f, 1f },
       /*ground*/ new float[] { 1f, 1.5f, 1f, 1.5f, 1f, 1f, .5f, 1f, 1.5f, 0f, .5f, 1f, 1f, 1.5f, 1f, 1f, 1.5f, 1f },
       /*rock*/ new float[] { 1f, 1.5f, 1f, 1f, 1f, .5f,  1f, .5f, .5f, 1.5f, 1.5f, 1f, 1f, .5f, 1f, 1f, 1f, 1.5f },
       /*flying*/ new float[] { 1f, 1f, 1f, .5f, 1f, 1.5f, 1.5f, 1f, .5f, 1f, 1.5f, 1f, 1f, .5f, 1f, 1f, 1f, 1f },
       /*bug*/ new float[] { 1f, .5f, 1f, 1f, 1f, .5f, 1.5f, 1f, 1f, .5f, .5f, 1.5f, 1f, .5f, .5f, 1f, .5f, 1f },
       /*dark*/ new float[] { 1f, 1f, 1f, 1f, 1.5f, .5f, 1f, 1f, 1f, 1f, 1f, .5f, 1.5f, .5f, .5f, 1f, 1f, 1f },
       /*ghost*/ new float[] { 0f, 1f, 1f, 1f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 1.5f, .5f, 1f, 1f, 1f, 1f },
       /*steel*/ new float[] { 1f, .5f, .5f, .5f, 1f, 1f,  1f, 1f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1.5f, 1f, 1f, 1.5f },
       /*fairy*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1.5f, 1f, 1f, 1f, 1.5f, .5f, 1f },
       /*dragon*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 0f, 1.5f, 1f, 1f },
       /*poison*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1.5f, 1f, 1f, 1f, 1f, 1f, 1f, 0f, 1.5f, 1f, .5f, 1f },
       /*ice*/ new float[] { 1f, .5f, .5f, 1f, 1f, 1f, 1.5f, 1.5f, 1f, 1.5f, 1f, 1f, 1f, .5f, 1f, 1.5f, 1f, .5f }
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}

