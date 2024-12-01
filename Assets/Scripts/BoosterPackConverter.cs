using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GUPS.AntiCheat.Protected;

public static class BoosterPackConverter 
{
    public static BoosterPackProtected ConvertToBoosterPackProtected(BoosterPackUnProtected boosterPack)
    {
        return new BoosterPackProtected
        {
            Price = new ProtectedFloat(boosterPack.Price),
            BoosterPacksTypes = new ProtectedString(boosterPack.BoosterPacksTypes),
            FinalTimeExpire = new ProtectedInt64(boosterPack.FinalTimeExpire),
            OriginalMultiplier = new ProtectedFloat(boosterPack.OriginalMultiplier)
        };
    }

    public static BoosterPackUnProtected ConvertToBoosterPackUnProtected(BoosterPackProtected boosterPackProtected)
    {
        return new BoosterPackUnProtected
        {
            Price = (float)boosterPackProtected.Price,
            BoosterPacksTypes = (string) boosterPackProtected.BoosterPacksTypes,
            FinalTimeExpire = (int)boosterPackProtected.FinalTimeExpire,
            OriginalMultiplier = (float)boosterPackProtected.OriginalMultiplier
        };
    }
    public static PlayerBoosterPackUnProtected ConvertToPlayerBoosterPackUnProtected(PlayerBoosterPackProtected boosterPackProtected)
    {
        return new PlayerBoosterPackUnProtected
        {
            Price = (float)boosterPackProtected.Price,
            BoosterPacksTypes = (string) boosterPackProtected.BoosterPacksTypes,
            FinalTimeExpire = (int)boosterPackProtected.FinalTimeExpire,
            OriginalMultiplier = (float)boosterPackProtected.OriginalMultiplier,

            ID = (int) boosterPackProtected.ID,
            DailyTimeExpire = (int) boosterPackProtected.DailyTimeExpire,
            CurrentMultiplier = (float) boosterPackProtected.CurrentMultiplier,
            BNBEarnPerClick = (float) boosterPackProtected.BNBEarnPerClick,
            AvailableClicks = (int) boosterPackProtected.AvailableClicks,
            TotalBNBEarned = (float) boosterPackProtected.TotalBNBEarned,
        };
    }
    public static PlayerBoosterPackProtected ConvertToPlayerBoosterPackProtected(PlayerBoosterPackUnProtected boosterPackProtected)
    {
        return new PlayerBoosterPackProtected
        {
            Price = new ProtectedFloat(boosterPackProtected.Price),
            BoosterPacksTypes = new ProtectedString(boosterPackProtected.BoosterPacksTypes),
            FinalTimeExpire = new ProtectedInt64(boosterPackProtected.FinalTimeExpire),
            OriginalMultiplier = new ProtectedFloat(boosterPackProtected.OriginalMultiplier),

            ID = new ProtectedInt64(boosterPackProtected.ID) ,
            DailyTimeExpire = new ProtectedInt64(boosterPackProtected.DailyTimeExpire) ,
            CurrentMultiplier = new ProtectedFloat(boosterPackProtected.CurrentMultiplier) ,
            BNBEarnPerClick = new ProtectedFloat(boosterPackProtected.BNBEarnPerClick) ,
            AvailableClicks = new ProtectedInt64(boosterPackProtected.AvailableClicks) ,
            TotalBNBEarned = new ProtectedFloat(boosterPackProtected.TotalBNBEarned) ,
        };
    }
}
