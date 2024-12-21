using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GUPS.AntiCheat.Protected;
[Serializable]
public class ClickRateUnProtected
{
    public float Win;
    public float Lose;
}
[Serializable]
public class ClickRateProtected
{
    public ProtectedFloat Win;
    public ProtectedFloat Lose;
}
[Serializable]
public class PlayerGameDataUnProtected 
{
    //referralMultiplierPoints
    public float TotalReferralMultiplierPoints;
    public List<PlayerBoosterPackUnProtected> OwnedBoosterPacks = new List<PlayerBoosterPackUnProtected>();
    public PlayerGameDataProtected ConvertToPlayerGameDataProtected()
    {
        List<PlayerBoosterPackProtected> ownedBoosterPacks = new List<PlayerBoosterPackProtected>();
        foreach (var item in OwnedBoosterPacks)
            ownedBoosterPacks.Add(item.GetReturnType(new PlayerBoosterPackProtected{}));
        
        return new PlayerGameDataProtected{
            TotalReferralMultiplierPoints = new ProtectedFloat(TotalReferralMultiplierPoints) ,
            OwnedBoosterPacks = ownedBoosterPacks,
          
        } ;
    }
    public override string ToString()
    {
        return "UnProtected PlayerGameDataUnProtected";
    }

}
[Serializable]
public class PlayerGameDataProtected 
{
    //referralMultiplierPoints
    public ProtectedFloat TotalReferralMultiplierPoints;
    public List<PlayerBoosterPackProtected> OwnedBoosterPacks = new List<PlayerBoosterPackProtected>();
    public PlayerGameDataUnProtected ConvertToPlayerGameDataUnProtected()
    {
        List<PlayerBoosterPackUnProtected> ownedBoosterPacks = new List<PlayerBoosterPackUnProtected>();
        foreach (var item in OwnedBoosterPacks)
            ownedBoosterPacks.Add(item.GetReturnType(new PlayerBoosterPackUnProtected{}));
    
        return new PlayerGameDataUnProtected{
            TotalReferralMultiplierPoints = (float) TotalReferralMultiplierPoints,
            OwnedBoosterPacks = ownedBoosterPacks,
            
        };
    }
    public override string ToString()
    {
        return "Protected PlayerGameDataProtected";
    }
}