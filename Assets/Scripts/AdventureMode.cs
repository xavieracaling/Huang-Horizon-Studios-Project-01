using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;
using System;
public class AdventureMode : MonoBehaviour
{
    public static AdventureMode Instance;
    [Header("UI")]
    public Text CompletionXPGainUI;
    public Text ClicksRequiredUI;
    public Text CurrentClicksUI;
    public Text TimeCountdownUI;
    [Header("Values")]
    ProtectedInt32 clicksRequired;
    ProtectedInt32 currentClicks;
    ProtectedInt32 timeCountdown;
  
    public ProtectedInt32 ClicksRequired {
        get => clicksRequired;
        set 
        {
            clicksRequired = value;
            ClicksRequiredUI.text = clicksRequired.ToString();
        } 
    }
    public ProtectedInt32 CurrentClicks {
        get => currentClicks;
        set 
        {
            currentClicks = value;
            CurrentClicksUI.text = currentClicks.ToString();
        } 
    }
    public ProtectedInt32 TimeCountdown {
        get => timeCountdown;
        set 
        {
            timeCountdown = value;
            TimeCountdownUI.text = timeCountdown.ToString();
        } 
    }
    
    void Awake()
    {
        Instance = this;
        
    }
    [ContextMenu("GameCompleted")]
    public void GameCompleted()
    {
        LevelManager.Instance.XPGain();
    }
    
}
