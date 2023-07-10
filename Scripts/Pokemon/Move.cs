using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MovesBase Base { get; set; }
    public int PP { get; set; }

    public Move(MovesBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
    public MoveSaveData GetSaveData()
    {
        var saveDataM = new MoveSaveData()
        {
            name = Base.name,
            pp = PP
        };
        return saveDataM;
    }

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetObjectByName(saveData.name);
        PP = saveData.pp;
    }

    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, Base.PP);
    }
}


public class Boost
{
    public ABoostBase BBase { get; set; }
    public int PP { get; set; }

    public Boost(ABoostBase bBase)
    {
        BBase = bBase;
        PP = bBase.PP;
    }

    public BoostSaveData GetSaveData()
    {
        var saveDataB = new BoostSaveData()
        {
            name = BBase.name,
            pp = PP
        };
        return saveDataB;
    }
    public Boost(BoostSaveData saveData)
    {
        BBase = BoostDB.GetObjectByName(saveData.name);
        PP = saveData.pp;
    }
    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, BBase.PP);
    }

}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}

[System.Serializable]
public class BoostSaveData
{
    public string name;
    public int pp;
}
