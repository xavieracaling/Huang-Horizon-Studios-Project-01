using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GUPS.AntiCheat.Protected;

public static class BoosterPackConverter 
{
    public static BoosterPackProtected ConvertToBoosterPackProtected(BoosterPackUnProtected boosterPackUnProtected)
    {
        return new BoosterPackProtected
        {
            Price = new ProtectedFloat(boosterPackUnProtected.Price),
            BoosterPacksTypes = new ProtectedString(boosterPackUnProtected.BoosterPacksTypes),
            FinalTimeExpire = new ProtectedInt64(boosterPackUnProtected.FinalTimeExpire),
            OriginalMultiplier = new ProtectedFloat(boosterPackUnProtected.OriginalMultiplier),

            ClickRate = new ClickRateProtected(){
                Win = new ProtectedFloat(boosterPackUnProtected.ClickRate.Win),
                Lose = new ProtectedFloat(boosterPackUnProtected.ClickRate.Lose),
            },
            Title = boosterPackUnProtected.Title,
            ImageIndex = boosterPackUnProtected.ImageIndex
        };
    }

    public static BoosterPackUnProtected ConvertToBoosterPackUnProtected(BoosterPackProtected boosterPackProtected)
    {
        return new BoosterPackUnProtected
        {
            Price = (float)boosterPackProtected.Price,
            BoosterPacksTypes = (string) boosterPackProtected.BoosterPacksTypes,
            FinalTimeExpire = (int)boosterPackProtected.FinalTimeExpire,
            OriginalMultiplier = (float)boosterPackProtected.OriginalMultiplier,

            ClickRate = new ClickRateUnProtected(){
                Win = (float)boosterPackProtected.ClickRate.Win,
                Lose = (float)boosterPackProtected.ClickRate.Lose,
            },
            Title = boosterPackProtected.Title,
            ImageIndex = boosterPackProtected.ImageIndex
        };
    }
    public static PlayerBoosterPackUnProtected ConvertToPlayerBoosterPackUnProtected(PlayerBoosterPackProtected playerBoosterPackProtected)
    {
        return new PlayerBoosterPackUnProtected
        {
            Price = (float)playerBoosterPackProtected.Price,
            BoosterPacksTypes = (string) playerBoosterPackProtected.BoosterPacksTypes,
            FinalTimeExpire = (int)playerBoosterPackProtected.FinalTimeExpire,
            OriginalMultiplier = (float)playerBoosterPackProtected.OriginalMultiplier,

            ID = (int) playerBoosterPackProtected.ID,
            DailyTimeExpire = (int) playerBoosterPackProtected.DailyTimeExpire,
            CurrentMultiplier = (float) playerBoosterPackProtected.CurrentMultiplier,
            BNBEarnPerClick = (float) playerBoosterPackProtected.BNBEarnPerClick,
            AvailableClicks = (int) playerBoosterPackProtected.AvailableClicks,
            TotalBNBEarned = (float) playerBoosterPackProtected.TotalBNBEarned,

            ClickRate = new ClickRateUnProtected(){
                Win = (float)playerBoosterPackProtected.ClickRate.Win,
                Lose = (float)playerBoosterPackProtected.ClickRate.Lose,
            },
            Title = playerBoosterPackProtected.Title,
            ImageIndex = playerBoosterPackProtected.ImageIndex
        };
    }
    public static PlayerBoosterPackProtected ConvertToPlayerBoosterPackProtected(PlayerBoosterPackUnProtected playerBoosterPackUnProtected)
    {
        return new PlayerBoosterPackProtected
        {
            Price = new ProtectedFloat(playerBoosterPackUnProtected.Price),
            BoosterPacksTypes = new ProtectedString(playerBoosterPackUnProtected.BoosterPacksTypes),
            FinalTimeExpire = new ProtectedInt64(playerBoosterPackUnProtected.FinalTimeExpire),
            OriginalMultiplier = new ProtectedFloat(playerBoosterPackUnProtected.OriginalMultiplier),

            ID = new ProtectedInt64(playerBoosterPackUnProtected.ID) ,
            DailyTimeExpire = new ProtectedInt64(playerBoosterPackUnProtected.DailyTimeExpire) ,
            CurrentMultiplier = new ProtectedFloat(playerBoosterPackUnProtected.CurrentMultiplier) ,
            BNBEarnPerClick = new ProtectedFloat(playerBoosterPackUnProtected.BNBEarnPerClick) ,
            AvailableClicks = new ProtectedInt64(playerBoosterPackUnProtected.AvailableClicks) ,
            TotalBNBEarned = new ProtectedFloat(playerBoosterPackUnProtected.TotalBNBEarned) ,
            
            ClickRate = new ClickRateProtected(){
                Win = new ProtectedFloat(playerBoosterPackUnProtected.ClickRate.Win),
                Lose = new ProtectedFloat(playerBoosterPackUnProtected.ClickRate.Lose),
            },
            Title = playerBoosterPackUnProtected.Title,
            ImageIndex = playerBoosterPackUnProtected.ImageIndex


        };
    }
}
