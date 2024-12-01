using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GUPS.AntiCheat.Protected;

[Serializable]
public class PlayerBoosterPackUnProtected : BoosterPackUnProtected
{ 
    //id
    //duration 

    //original multiplier
    //current multiplier

    //daily earn
    
    public int ID;
    public int DailyTimeExpire;
    public float CurrentMultiplier;
    public float BNBEarnPerClick;
    public int AvailableClicks;
    public float TotalBNBEarned;
    public PlayerBoosterPackProtected GetReturnType(PlayerBoosterPackProtected boosterPackProtected) //convert to PlayerBoosterPackProtected
    {
        return BoosterPackConverter.ConvertToPlayerBoosterPackProtected(this);
    }
    public override string ToString()
    {
        return "UnProtectd PlayerBoosterPackUnProtected class";
    }
}
[Serializable]

public class BoosterPackUnProtected : BoosterProtect<BoosterPackProtected>
{ 
    public ClickRateUnProtected ClickRate;
    public int ImageIndex;
    public string Title;
    public float Price;
    public string BoosterPacksTypes;
    public int FinalTimeExpire;
    public float OriginalMultiplier;
    public override BoosterPackProtected GetReturnType(BoosterPackProtected boosterPackProtected) //convert to BoosterPackProtected
    {
        return BoosterPackConverter.ConvertToBoosterPackProtected(this);
    }
    public override string ToString()
    {
        return "UnProtectd BoosterPack class";
    }
    
}

