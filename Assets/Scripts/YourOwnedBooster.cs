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
    public PlayerBoosterPackProtected YourBooster;
    public Button UsePlay;
    public Button Withdraw;
    
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
        CurrentMultiplier.text = $"{boosterPackProtected.CurrentMultiplier}X ";
        TotalBNBEarned.text = $"{bnbEarnedTotal.ToString("0.0000000")} ";
        DailyResets.text = $"{boosterPackProtected.DailyTimeExpire} HRS";
        YourBooster = boosterPackProtected;

        if (boosterPackProtected.TotalBNBEarned > 0)
        {
            Withdraw.interactable = true;
            Withdraw.onClick.AddListener(() => 
            {
                decimal totalBNBEarned = (decimal)(float)boosterPackProtected.TotalBNBEarned;
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
