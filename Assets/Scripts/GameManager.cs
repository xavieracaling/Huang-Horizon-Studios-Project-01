using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using GUPS.AntiCheat.Protected;
using System;
using PlayFab.ClientModels;
using PlayFab;
public class CheckLevelUnlockInfo
{
    public bool UnlockedLevel;
    public int MyCurrentTotalReferrals;
    public int RequiredTotalReferrals;
}
public class GameManager : MonoBehaviour
{
    public Coroutine IStartSpawnRushers;
    public static GameManager Instance;
    public GameObject PrefabGoingLeftCharacter;
    public GameObject PrefabGoingRightCharacter;
    public GameObject GameContainer;
    public Transform MaskGameContainer;
    public GameObject MainMenuGO;
    public GameObject MenuCenterBG;
    public GameObject PrefabBomb;
    public GameObject PrefabBNBUIText;
    public List<GameObject>  ListAllRushes = new List<GameObject>();
    public List<GameObject>  ListEffectRush = new List<GameObject>();
    [Header("Game Whole Data Protected")]

    public PlayerGameDataProtected _PlayerGameDataProtected;
    [Header("Current Used")]
    public PlayerBoosterPackProtected CurrentUsedPlayerBoosterPackProtected;
    public List<Action> ListOfOnSavePlayerData = new List<Action>();
    public Coroutine ISavePlayerData;
    public bool AbleToSavePlayerData;

    public Modes CurrentMode;
    public GameObject GameContainer_Adventure;
    public GameObject GameContainer_BoosterMode;
    void Awake()
    {
        AbleToSavePlayerData = true;
        Instance = this;
    }
    
    public void GotoMenu()
    {
        CurrentUsedPlayerBoosterPackProtected = null;
        GameContainer.SetActive(false);
        GameContainer_Adventure.SetActive(false);
        GameContainer_BoosterMode.SetActive(false);
        MainMenuGO.SetActive(true);
        MenuCenterBG.SetActive(true);
        if (CurrentMode == Modes.BoosterMode)
        {
            gameStopBoosterMode();
            StopISavePlayerData();
        }
        else if (CurrentMode == Modes.Adventure)
        {
            
        }
    }
    #region AdventureMode

    public void StartGameAdventure() => PlayFabManager.Instance.ExecuteWithSessionCheck( () => 
    {
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "checkLevelUnlock",
            FunctionParameter = new {
                playerId =  PlayFabManager.Instance.PlayFabID
            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request,result => 
        {
            
            CheckLevelUnlockInfo checkLevelUnlockInfo = JsonConvert.DeserializeObject<CheckLevelUnlockInfo> (result.FunctionResult.ToString());
            if (!checkLevelUnlockInfo.UnlockedLevel)
            {
                Destroy(loading);
                UIManager.Instance.InstantiateMessagerPopPrefab_Message(
                    $"Unable to enter.\n \n To continue leveling from <color=#00FF00>{LevelManager.Instance.CurrentLevel}</color>, you need at least <color=#00FF00>{checkLevelUnlockInfo.RequiredTotalReferrals}</color> Total Referrals. \n \n Right now you only have <color=#00FF00>{checkLevelUnlockInfo.MyCurrentTotalReferrals} </color> Total referrals "
                );
                return;
            }
            Destroy(loading);

            CurrentMode = Modes.Adventure;
            GameContainer.SetActive(true);
            GameContainer_BoosterMode.SetActive(false);
            GameContainer_Adventure.SetActive(true);

               
        }, error => { Destroy(loading); Debug.Log("error") ;}  );

        
        
    });


    #endregion


    #region  BoosterMode
    public void StartGameBoosterMode() => PlayFabManager.Instance.ExecuteWithSessionCheck( () => 
    {
        CurrentMode = Modes.BoosterMode;
        if(_PlayerGameDataProtected.OwnedBoosterPacks.Count > 0 )
        {
            Debug.Log($"_PlayerGameDataProtected.OwnedBoosterPacks.Count {_PlayerGameDataProtected.OwnedBoosterPacks.Count}");
            BoosterManager.Instance.BoosterShow();
        }
        else
        {
            Debug.Log("No boosters!");
            BoosterManager.Instance.NoBooster();
        }
    });
    public void StartISavePlayerData()
    {
        if(ISavePlayerData != null)
            StopCoroutine(ISavePlayerData);
        ISavePlayerData = StartCoroutine(IESavePlayerData());
    }
    public void StopISavePlayerData()
    {
        if(ISavePlayerData != null)
            StopCoroutine(ISavePlayerData);
        ListOfOnSavePlayerData.Clear();
    }
    
    IEnumerator IESavePlayerData()
    {
        while (true)
        {
            if(ListOfOnSavePlayerData.Count > 0 && AbleToSavePlayerData)
            {
                ListOfOnSavePlayerData[ListOfOnSavePlayerData.Count - 1]?.Invoke();
                ListOfOnSavePlayerData.Remove(ListOfOnSavePlayerData[ListOfOnSavePlayerData.Count - 1]);
                yield return new WaitForEndOfFrame();
            }
                yield return new WaitForSeconds(1.5f);

        }
    }
    public void FinalStartGameBoosterMode(PlayerBoosterPackProtected playerBoosterPackProtected)
    {
        CurrentUsedPlayerBoosterPackProtected = playerBoosterPackProtected;
        GameContainer.SetActive(true);
        GameContainer_BoosterMode.SetActive(true);
        GameContainer_Adventure.SetActive(false);
        MainMenuGO.SetActive(false);
        MenuCenterBG.SetActive(false);
        StartISavePlayerData();
        IStartSpawnRushers = StartCoroutine(StartSpawnRushers());
        Debug.Log("Game has started!");
    }
  
    public void CompletedBooster()
    {
        gameStopBoosterMode();
        // StopISavePlayerData();
        GameContainer.SetActive(false);
        UIManager.Instance.InstantiateMessagerPopPrefabFull("Great job! No more available clicks, please check your booster.", () =>GotoMenu(),false );
    }
    void gameStopBoosterMode()
    {
        if (IStartSpawnRushers != null)
        {
            StopCoroutine(IStartSpawnRushers);
            IStartSpawnRushers = null;
        }
        for (int i = ListAllRushes.Count - 1; i >= 0; i--)
        {
            if (ListAllRushes[i] != null)
            {
                Destroy(ListAllRushes[i]);
            }
            ListAllRushes.RemoveAt(i);
        }
        ListAllRushes.Clear();
    }
    public IEnumerator StartSpawnRushers()
    {
        PlayerBoosterPackProtected playerBoosterPackProtected = GameManager.Instance.CurrentUsedPlayerBoosterPackProtected;
        UIManager.Instance.UpdateUIClicks((int)playerBoosterPackProtected.AvailableClicks,(float)playerBoosterPackProtected.TotalBNBEarned );

        while(true)
        {
            bool goingLeftChar = false;
            int indexRand =  UnityEngine.Random.Range(0,2);
            int indexRandLeft =  UnityEngine.Random.Range(0,2);
            
            float randomXStartPos = 0;
            float targetXEndPos = 0;
            float yPosStarting = 0;
            GameObject rusher = null;
            if(indexRand == 0)
                goingLeftChar = true;
            
            if(goingLeftChar) //left
            {
                rusher = Instantiate(PrefabGoingLeftCharacter,MaskGameContainer);
                targetXEndPos = -1120f;
                randomXStartPos = UnityEngine.Random.Range(524,924);
                if(indexRandLeft == 0)
                    yPosStarting = 273;
                else
                    yPosStarting = -291;
            }
            else //right
            {
                rusher = Instantiate(PrefabGoingRightCharacter,MaskGameContainer);
                targetXEndPos = 1130f;
                randomXStartPos =  UnityEngine.Random.Range(-987,-605);
                yPosStarting = -48;
            }
            int rand =  UnityEngine.Random.Range(0,2);
            if(rand == 1)
            {
                Rusher _rusher = rusher.GetComponent<Rusher>();
                _rusher.Explode = true;
            }
            rusher.transform.localPosition = new UnityEngine.Vector2(randomXStartPos,yPosStarting);
            rusher.transform.DOLocalMoveX(targetXEndPos, UnityEngine.Random.Range(3f,4.6f)).SetEase(Ease.Linear).OnComplete(() => {if(rusher!= null) Destroy(rusher);} );
            ListAllRushes.Add(rusher);
            yield return new WaitForSeconds( UnityEngine.Random.Range(0.1f,1.2f));
        }
        
    }
    #endregion
}
