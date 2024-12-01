using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]

public class PlayerBoosterPack : BoosterPack
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
}
[Serializable]

public class BoosterPack 
{ 
    public string BoosterPacksTypes;
    public int FinalTimeExpire;
    public float OriginalMultiplier;
}
[Serializable]
public class ShopBoosterPack : BoosterPack
{ 
    public bool AvailableToBuy = true;
}
