using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YourOwnedBooster : MonoBehaviour
{
    public Image BoosterIMG;
    public Text ExpiresAt;
    public Text Price;
    public Text BNBPerClick;
    public Text AvailableClicks;
    public Text CurrentMultiplier;
    public Text TotalBNBEarned;
    public Text DailyResets;
    public Text BoosterTitle;
    public Text ID;
    public Text LimitWithdrawUI;
    public Text RefreshDailyUI;
    public PlayerBoosterPackProtected YourBooster;
    public Button UsePlay;
    public Button Withdraw;
    public GameObject MinWithdrawGO;
    public bool Expired;
    public GameObject ExpiredPanel;
    void Update()
    {
        if (!Expired)
        {
            checkExpiration();
        }
        checkDailyReset();
    }
    public void DeleteBooster() 
    {
        PlayFabManager.Instance.DeletePlayerBooster(YourBooster,() => Destroy(gameObject) );
    } 
    void checkDailyReset()
    {
        if (YourBooster != null)
        {
            if (YourBooster.TimeExpirationsProtected.DailyResetTarget != 0)
            {
                long currentTime =  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                TimeSpan targetExpire = TimeSpan.FromMilliseconds(YourBooster.TimeExpirationsProtected.DailyResetTarget - currentTime);
                if (currentTime <= YourBooster.TimeExpirationsProtected.DailyResetTarget)
                {
                    if (UsePlay.interactable)
                    {
                        UsePlay.interactable = false;
                    }
                    RefreshDailyUI.text = $"{targetExpire.Hours}HRS/{targetExpire.Minutes}M/{targetExpire.Seconds}S";
                }
                else 
                {
                    AvailableClicks.text = "50";
                    YourBooster.AvailableClicks = 50;
                    YourBooster.TimeExpirationsProtected.DailyResetTarget = 0;
                    UsePlay.interactable = true;
                    RefreshDailyUI.text = "";
                    PlayFabManager.Instance.SavePlayerBoosterPackData();
                }
            }
        }
    }
    
    void checkExpiration()
    {
        if (YourBooster != null)
        {
            long currentTime =  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            TimeSpan targetExpire = TimeSpan.FromMilliseconds(YourBooster.TimeExpirationsProtected.ExpireTarget - currentTime);

            if (currentTime <= YourBooster.TimeExpirationsProtected.ExpireTarget)
            {
                ExpiresAt.text = $"{targetExpire.Days}D/{targetExpire.Hours}HRS/{targetExpire.Minutes}M/{targetExpire.Seconds}S";
            }
            else 
            {
                ExpiresAt.text = $"EXPIRED!!!";
                if (!Expired)
                {
                    ExpiredPanel.SetActive(true);
                    Expired = true;
                }
            }
        }
    }
    public void InitializeBooster(PlayerBoosterPackProtected boosterPackProtected)
    {
        BoosterIMG.sprite = UIManager.Instance.BoosterPackIcons[boosterPackProtected.ImageIndex];
        ExpiresAt.text = $"{boosterPackProtected.FinalTimeExpire} DAYS LEFT";
        Price.text = $"{boosterPackProtected.Price} BNB";
        ID.text = "ID: "+  boosterPackProtected.ID.ToString();
        BoosterTitle.text = boosterPackProtected.Title;
        float bnbEarned = boosterPackProtected.BNBEarnPerClick;
        float bnbEarnedTotal = boosterPackProtected.TotalBNBEarned;
        BNBPerClick.text = $"{bnbEarned.ToString("0.0000000")} ";

        AvailableClicks.text = $"{boosterPackProtected.AvailableClicks} LEFT";
        float _currentMultiplier = (float)boosterPackProtected.CurrentMultiplier;
        float totalReferralMultiplier = PlayFabManager.Instance._PlayerReferral.TotalReferralMultiplier;

        if (PlayFabManager.Instance._PlayerReferral.TotalReferrals > 0)
        {
            CurrentMultiplier.text = $"{_currentMultiplier.ToString("0.000")}x(<color=green>+{totalReferralMultiplier.ToString("0.000")}</color>)";
        }
        else
        {
            CurrentMultiplier.text = $"{_currentMultiplier.ToString("0.000")}x";
        }
        TotalBNBEarned.text = $"{bnbEarnedTotal.ToString("0.0000000")} ";
        DailyResets.text = $"{boosterPackProtected.DailyTimeExpire} HRS";
        YourBooster = boosterPackProtected;
        decimal totalBNBEarned = (decimal)(float)boosterPackProtected.TotalBNBEarned;
        decimal minBNBWithdraw = (decimal)(float)boosterPackProtected.Price * (decimal) PlayFabManager.Instance.GetMultiplier(boosterPackProtected.BoosterPacksTypes) * (decimal)0.04;
        


        if (totalBNBEarned >= minBNBWithdraw)
        {
            if (boosterPackProtected.TotalBNBEarned > 0)
            {
                MinWithdrawGO.SetActive(false);
                Withdraw.interactable = true;
            }
        }
        else
        {
            MinWithdrawGO.SetActive(true);
            LimitWithdrawUI.text =  minBNBWithdraw.ToString("0.0000000");
            Withdraw.interactable = false;
        }

        if (boosterPackProtected.TotalBNBEarned > 0)
        {
            
            
            Withdraw.onClick.AddListener(() => 
            {
                Withdraw.interactable = false;
                boosterPackProtected.TotalBNBEarned = 0;
                PlayFabManager.Instance.SavePlayerBoosterPackData(() => _EVM.Instance.Withdraw(totalBNBEarned));
            }  );
        }
        else
        {
            Withdraw.interactable = false;
        }
        UsePlay.onClick.AddListener(() => PlayFabManager.Instance.CheckCurrentBoosterBeforeStart(boosterPackProtected));

    }
}
