using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GUPS.AntiCheat.Protected;

[Serializable]
public class PlayerBoosterPackProtected  : BoosterPackProtected
{ 
    //id
    //duration 

    //original multiplier
    //current multiplier

    //daily earn
    public ProtectedInt64 ID;
    public ProtectedInt64 DailyTimeExpire;
    public ProtectedFloat CurrentMultiplier;
    public ProtectedFloat BNBEarnPerClick;
    public ProtectedInt64 AvailableClicks;
    public ProtectedFloat TotalBNBEarned;
    public PlayerBoosterPackUnProtected GetReturnType(PlayerBoosterPackUnProtected boosterPack) //serialize
    {
        return BoosterPackConverter.ConvertToPlayerBoosterPackUnProtected(this);
    }
    public override string ToString()
    {
        return "Protectd PlayerBoosterPackProtected class";
    }

}
[Serializable]

public class BoosterPackProtected : BoosterProtect<BoosterPackUnProtected>
{ 
    public ClickRateProtected ClickRate;
    public int ImageIndex;
    public string Title;
    public ProtectedFloat Price;
    public ProtectedString BoosterPacksTypes;
    public ProtectedInt64 FinalTimeExpire;
    public ProtectedFloat OriginalMultiplier;
    
    public override BoosterPackUnProtected GetReturnType(BoosterPackUnProtected boosterPack) //serialize
    {
        return BoosterPackConverter.ConvertToBoosterPackUnProtected(this);
    }
    public override string ToString()
    {
        return "Protected BoosterPackProtected class";
    }

}


