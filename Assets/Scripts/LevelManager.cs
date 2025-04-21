using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GUPS.AntiCheat.Protected;
using Unity.VisualScripting;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json;
[Serializable]
public class GetXPResult
{
    public bool IsLevelUp;
    public string PlayerLevelInfo;
    public int TapTicketsEarned;
}
[Serializable]
public class PlayerLevel
{
    public int CurrentLevel;
    public int BaseXP;
    public int CurrentXP;
    public int RequiredXP;
}
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<LevelXPBar> LevelXPBars = new List<LevelXPBar>();
    public  Action LevelValuesSync;
    public int XPGainTest;
    [SerializeField]
    PlayerLevel _playerLevel;
    [Header("Values")]
    ProtectedInt32 currentLevel;
    ProtectedInt32 baseXP;
    ProtectedInt32 currentXP;
    ProtectedInt32 requiredXP;
    public ProtectedInt32 CurrentLevel {
        get => currentLevel;
        set 
        {
            ShowableCurrentLevel = value;
            currentLevel = value;
            SyncValuesCurrentLevel();
        } 
    }
    public ProtectedInt32 BaseXP {
        get => baseXP;
        set 
        {
            baseXP = value;
            SyncValuesBaseXP();
        } 
    }
    public ProtectedInt32 CurrentXP {
        get => currentXP;
        set 
        {
            currentXP = value;
            ShowableCurrentXP = currentXP;
            SyncValuesCurrentXP();
        } 
    }
    public ProtectedInt32 RequiredXP {
        get => requiredXP;
        set 
        {
            ShowableRequiredXP = value;
            requiredXP = value;
            SyncValuesRequiredXP();
        } 
    }
    public int ShowableCurrentLevel;
    public int ShowableCurrentXP;
    public int ShowableRequiredXP;
    public GetXPResult LatestGetXPResult;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //TestInit();
    }
    
    [ContextMenu("TestXPGain")]
    public void TestXPGain()
    {
        XPGain(XPGainTest);
    }
    public void TestInit()
    {
        CurrentLevel = 1;
        BaseXP = 50;
        CurrentXP = 0;
        RequiredXP = 150;
    }

    public void XPGain(int xpGain)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getXP",
            FunctionParameter = new {
                playerId = PlayFabManager.Instance.PlayFabID
            }
        };
        
        PlayFabClientAPI.ExecuteCloudScript(request,result => {
            var json = result.FunctionResult.ToString();

            var getXPResult = JsonUtility.FromJson<GetXPResult>(json);
            
            _playerLevel = JsonUtility.FromJson<PlayerLevel>(getXPResult.PlayerLevelInfo);
            LatestGetXPResult = getXPResult;

            if (getXPResult.IsLevelUp)
            {
                RequiredXP = _playerLevel.RequiredXP;
                CurrentLevel = _playerLevel.CurrentLevel;
            }
            CurrentXP = _playerLevel.CurrentXP;
            
        }, error => Debug.Log("error")) ;

    }
    public void SetLevelInfos(PlayerLevel playerLevel)
    {
        _playerLevel = playerLevel;

        CurrentLevel =  _playerLevel.CurrentLevel;
        BaseXP =  _playerLevel.BaseXP;
        
        RequiredXP =  _playerLevel.RequiredXP;
        CurrentXP =  _playerLevel.CurrentXP;
    }
    public void SyncValuesCurrentLevel()
    {
        foreach (var item in LevelXPBars)
        {
            item.CurrentLevel = CurrentLevel;
        }
    }
    public void SyncValuesBaseXP()
    {
        foreach (var item in LevelXPBars)
        {
            item.BaseXP = BaseXP;
        }
    }
    public void SyncValuesCurrentXP()
    {
        foreach (var item in LevelXPBars)
        {
            item.CurrentXP = CurrentXP;
        }
    }
    public void SyncValuesRequiredXP()
    {
        foreach (var item in LevelXPBars)
        {
            item.RequiredXP = RequiredXP;
        }
    }
    void Update()
    {
        
    }
}
