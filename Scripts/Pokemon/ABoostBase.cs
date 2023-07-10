using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boost", menuName = "Pokemon/Create new boost")]
public class ABoostBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;

    [SerializeField] BoostCategory category;
    [SerializeField] BoostEffects effects;
    [SerializeField] BoostTarget target;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
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

    public BoostCategory Category
    {
        get { return category; }
    }
    public BoostEffects Effects
    {
        get { return effects; }
    }
    public BoostTarget Target
    {
        get { return target; }
    }
}

[System.Serializable]
public class BoostEffects
{
    [SerializeField] List<StatBoost> aboosts;

    public List<StatBoost> ABoosts
    {
        get { return aboosts; }
    }
}


[System.Serializable]
public class AB
{
    public Stat bstat;
    public int bboost;
}
public enum BoostCategory
{
    bStatus
}
public enum BoostTarget
{
    bSelf
}

