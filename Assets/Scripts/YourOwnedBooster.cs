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
    
    public void InitializeBooster(PlayerBoosterPackProtected boosterPackProtected)
    {
        BoosterIMG.sprite = UIManager.Instance.BoosterPackIcons[boosterPackProtected.ImageIndex];
        ExpiresAt.text = $"{boosterPackProtected.FinalTimeExpire} DAYS LEFT";
        Price.text = $"{boosterPackProtected.Price} BNB";
        ID.text = "ID: "+  boosterPackProtected.ID.ToString();
        BoosterTitle.text = boosterPackProtected.Title;
        float bnbEarned = boosterPackProtected.BNBEarnPerClick;
        BNBPerClick.text = $"{bnbEarned.ToString("0.0000000")} BNB";

        AvailableClicks.text = $"{boosterPackProtected.AvailableClicks} LEFT";
        CurrentMultiplier.text = $"{boosterPackProtected.CurrentMultiplier}X ";
        TotalBNBEarned.text = $"{boosterPackProtected.TotalBNBEarned} BNB";
        DailyResets.text = $"{boosterPackProtected.DailyTimeExpire} HRS";
        YourBooster = boosterPackProtected;

        UsePlay.onClick.AddListener(() => PlayFabManager.Instance.CheckCurrentBoosterBeforeStart(boosterPackProtected));

    }
}
