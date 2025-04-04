using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GUPS.AntiCheat.Protected;
[Serializable]
public class PlayerInfo
{
    public int Level;
    public int Experience;
    public int RequiredExperience;
}
[Serializable]

public class TapTicketsInfo
{
    public int NewReferralTapTickets;
    public int CurrentTapTickets;
}
[Serializable]
public class PlayerReferral
{
    public int TotalReferrals;              // Change to int for count
    public float TotalReferralMultiplier;   // Change to float for multiplier
    public int TotalFirstReferrals;         // Change to int for count
    public int TotalSecondReferrals;        // Change to int for count
    public int TotalThirdReferrals;
}
[Serializable]
public class TimeExpirationsUnProtected
{
    public Int64 DailyResetTarget;
    public Int64 ExpireTarget;
}
[Serializable]
public class TimeExpirationsProtected
{
    public ProtectedInt64 DailyResetTarget;
    public ProtectedInt64 ExpireTarget;
}
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
    public List<PlayerBoosterPackUnProtected> OwnedBoosterPacksUnProtected = new List<PlayerBoosterPackUnProtected>(); //list u own boosters
    public PlayerGameDataProtected ConvertToPlayerGameDataProtected()
    {
        List<PlayerBoosterPackProtected> ownedBoosterPacksProtected = new List<PlayerBoosterPackProtected>();
        foreach (var item in OwnedBoosterPacksUnProtected)
            ownedBoosterPacksProtected.Add(item.GetReturnType(new PlayerBoosterPackProtected{}));
        
        return new PlayerGameDataProtected{
            TotalReferralMultiplierPoints = new ProtectedFloat(TotalReferralMultiplierPoints) ,
            OwnedBoosterPacks = ownedBoosterPacksProtected,
          
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
    public List<PlayerBoosterPackProtected> OwnedBoosterPacks = new List<PlayerBoosterPackProtected>(); //list u own boosters
    public PlayerGameDataUnProtected ConvertToPlayerGameDataUnProtected()
    {
        List<PlayerBoosterPackUnProtected> ownedBoosterPacks = new List<PlayerBoosterPackUnProtected>();
        foreach (var item in OwnedBoosterPacks)
            ownedBoosterPacks.Add(item.GetReturnType(new PlayerBoosterPackUnProtected{}));
    
        return new PlayerGameDataUnProtected{
            TotalReferralMultiplierPoints = (float) TotalReferralMultiplierPoints,
            OwnedBoosterPacksUnProtected = ownedBoosterPacks,
            
        };
    }
    public override string ToString()
    {
        return "Protected PlayerGameDataProtected";
    }
}