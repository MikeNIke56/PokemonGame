using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moves", menuName = "Pokemon/Create new move")]
public class MovesBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] public bool alwaysHits;
    [SerializeField] int priority;
    [SerializeField] int pp;
    [SerializeField] MoveTarget target;

    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    

    [SerializeField] AudioClip sound;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }
    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int Priority
    {
        get { return priority; }
    }
    public int PP
    {
        get { return pp; }
    }

    public MoveCategory Category
    {
        get { return category; }
    }
    public MoveEffects Effects
    {
        get { return effects; }
    }
    public List<SecondaryEffects> SecondaryEffects
    {
        get { return secondaryEffects; }
    }
    public MoveTarget Target
    {
        get { return target; }
    }
    public AudioClip Sound
    {
        get { return sound; }
    }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionsID status;
    [SerializeField] ConditionsID volatilestatus;
    [SerializeField] ConditionsID weather;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
    public ConditionsID Status
    {
        get { return status; }
    }
    public ConditionsID VolatileStatus
    {
        get { return volatilestatus; }
    }
    public ConditionsID Weather
    {
        get { return weather; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance
    {
        get { return chance; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
 public class StatBoost
{
    public Stat stat;
    public int boost;
}
public enum MoveCategory
{
    Physical, Special, Status
}
 public enum MoveTarget
{
    Foe, Self
}
