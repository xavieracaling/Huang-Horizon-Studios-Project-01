using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;

public class PlayFabManager : MonoBehaviour
{
    public string LoginSessionCheckStatus;
    public string PlayerName;
    public string LoginStateSession;
    public string LoginStateUpdatedSession;
    public string CustomUserIDAddress;
    public static PlayFabManager Instance;
    public string PlayFabID {get; private set;}
    public PlayerReferral _PlayerReferral;
    public TapTicketsInfo _TapTicketsInfo;
    [SerializeField]
    PlayerLevel _PlayerLevel;

    public Text Referral1UI;
    public Text Referral2UI;
    long targetT;
    public string ReferredBy;
    void Awake()
    {
        Instance = this;
       // targetT = (DateTimeOffset.UtcNow + TimeSpan.FromSeconds(15)).ToUnixTimeMilliseconds();;
    }
    void Update()
    {
        // long epochTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        // TimeSpan remainingTime = TimeSpan.FromMilliseconds(targetT - epochTime);
        // //Debug.Log("Epoch Time (Seconds): " +(remainingTime.Seconds)) ;
        // Debug.Log("Epoch Time (Seconds): " +(epochTime)) ;
    }
    public void CheckCurrentBoosterBeforeStart(PlayerBoosterPackProtected playerBoosterPackProtected)
    {
        bool foundID = false;
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => 
        {
            if(result.Data.ContainsKey("PlayerGameData"))
            {
                PlayerGameDataUnProtected playerGameDataProtected = JsonConvert.DeserializeObject<PlayerGameDataUnProtected>(result.Data["PlayerGameData"].Value);
                PlayerGameDataProtected data_playerBoosterPackProtected = playerGameDataProtected.ConvertToPlayerGameDataProtected();

                foreach (var item in data_playerBoosterPackProtected.OwnedBoosterPacks)
                {
                        if(item.ID == playerBoosterPackProtected.ID)
                        {
                            foundID = true;
                            break;
                        }
                }
            }
            if(foundID)
            {
                if (playerBoosterPackProtected.AvailableClicks <= 0)
                {
                    UIManager.Instance.InstantiateMessagerPopPrefab_Message("You don't have any available clicks, please wait for your booster's daily reset.") ;
                }
                else
                {
                    BoosterManager.Instance.YourBoosterGO.SetActive(false);
                    GameManager.Instance.FinalStartGameBoosterMode(playerBoosterPackProtected);
                }
                Destroy(loading);

            }
            else
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Message("Error booster, please refresh the game.") ;
                BoosterManager.Instance.YourBoosterGO.SetActive(false);
                Debug.Log("Error!!");
                Destroy(loading);
            }
        },
        error => {  UIManager.Instance.InstantiateMessagerPopPrefab_Message("Error booster, please refresh the game.") ;
                    BoosterManager.Instance.YourBoosterGO.SetActive(false);
                    Destroy(loading);});
        
        
   
    }
    public void CustomLogin(string customID)
    {
        CustomUserIDAddress = customID;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customID,
            CreateAccount = true ,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true
            }// Creates a new account if one doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, onLoginSuccess, onLoginFailure);
    }
    [ContextMenu("TestServerPlayfab")]
    public void TestServerPlayfab()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "test",
            FunctionParameter = new {
                testText = "fuckk"
            }

        };
        PlayFabClientAPI.ExecuteCloudScript(request,testServerPlayfabSuccess, testServerPlayfabError);
    }
    void testServerPlayfabSuccess(ExecuteCloudScriptResult executeCloudScriptResult)
    {
        Debug.Log(executeCloudScriptResult.FunctionResult.ToString());

    }
    void testServerPlayfabError(PlayFabError error)
    {
        Debug.Log("ERROR");
    }
    [ContextMenu("SendReferral")]

    public void GotReferred()
    {
        Debug.Log($"WebglReferral.Instance.ReferralTest {WebglReferral.Instance.ReferralID}");
        string h = WebglReferral.Instance.ReferralID;

        var requestBooster = new GetUserDataRequest
        {
            Keys = new List<string> { "BoughtFirstBooster" }
        };

        PlayFabClientAPI.GetUserData(requestBooster, result =>
        {
            Debug.Log("User data received:");
            if (result.Data != null)
            {
                bool boughtBooster = Convert.ToBoolean(result.Data["BoughtFirstBooster"].Value) ;
                if (boughtBooster)
                {
                    var request = new ExecuteCloudScriptRequest
                    {
                        FunctionName = "addReferral",
                        FunctionParameter = new { 
                            referringPlayerIds = h ,
                            playerId = PlayFabSettings.staticPlayer.PlayFabId 
                            },
                        GeneratePlayStreamEvent = true
                    };
                    
                    PlayFabClientAPI.ExecuteCloudScript(request, result =>
                    {

                      //  UIManager.Instance.InstantiateTenMessagerReferralPop("10");
                        _TapTicketsInfo = JsonConvert.DeserializeObject<TapTicketsInfo>(result.FunctionResult.ToString());
                        UIManager.Instance.UpdateUITapTickets();
                        Debug.Log("Referral added!" );
                    }, error =>
                    {
                        Debug.LogError("Error adding referral: " + error.GenerateErrorReport());
                    });
                }
            }
            else
            {
                Debug.Log("No user data available.");
            }
        },
        error =>
        {
            Debug.LogError("Error getting user data: " + error.GenerateErrorReport());
        });
        
    }
    public void ValidateReferralCode(string referringId , Action wholeAction, Action completedAction = null)
    {
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "validateReferralCode",
            FunctionParameter = new
            {
                referringPlayerIds = referringId
            },
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, (Action<ExecuteCloudScriptResult>)(result =>
        {
            if (result.FunctionResult != null)
            {
                bool isValid = Convert.ToBoolean(result.FunctionResult);
                Debug.Log("Referral Code Valid: " + isValid);
                Destroy(loading);
                if (isValid)
                {
                    WebglReferral.Instance.ReferralID = referringId;
                    var request = new UpdateUserDataRequest
                    {
                        Data = new Dictionary<string, string>
                        {
                            { "ReferredBy",  referringId },
                        }
                    };
                    PlayFabClientAPI.UpdateUserData(request, result =>
                    {
                        Debug.Log($"ReferredBy updated successfully. ");
                        completedAction?.Invoke();
                    }, error =>
                    {
                        WebglReferral.Instance.ReferralID = "";
                        completedAction?.Invoke();
                        UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                        Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
                        return;
                    });
                    
                    wholeAction?.Invoke();
                }
                else
                {
                    completedAction?.Invoke();
                    WebglReferral.Instance.ReferralID = "";
                    UIManager.Instance.InstantiateMessagerPopPrefab_Message("Invalid Referral Code.") ;
                }
            }
            else
            {
                completedAction?.Invoke();
                WebglReferral.Instance.ReferralID = "";
                Destroy(loading);
            }
        }), error =>
            {
                WebglReferral.Instance.ReferralID = "";
                completedAction?.Invoke();
                Destroy(loading);
            });
    }
    void onLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        PlayFabID = result.PlayFabId;
        Referral1UI.text = $"https://bnb-clickrush.com/game?referralCode={PlayFabID}";
        Referral2UI.text = $"{PlayFabID}";
        Debug.Log("PlayFab ID: " + result.PlayFabId);
        if (result.NewlyCreated) // new created
        {
            UIManager.Instance.NameContainerUI.SetActive(true);
        }
        else
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result2 =>
            {
                string displayName = result2.AccountInfo.TitleInfo.DisplayName;
                if (string.IsNullOrEmpty (displayName)  )
                {
                    UIManager.Instance.NameContainerUI.SetActive(true);
                }
                else
                {
                    WebglReferral.Instance.ReferralID   = "";
                    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => {
                    try
                    {
                        WebglReferral.Instance.ReferralID = result.Data["ReferredBy"].Value;
                        if(result.Data.ContainsKey("PlayerGameData"))
                        {
                            PlayerGameDataUnProtected playerGameDataProtected = JsonConvert.DeserializeObject<PlayerGameDataUnProtected>(result.Data["PlayerGameData"].Value);
                            GameManager.Instance._PlayerGameDataProtected = playerGameDataProtected.ConvertToPlayerGameDataProtected();
                        }

                        _TapTicketsInfo = JsonConvert.DeserializeObject<TapTicketsInfo >(result.Data["TapTicketsInfo"].Value);
                        UIManager.Instance.UpdateUITapTickets();
                        _PlayerLevel = JsonConvert.DeserializeObject<PlayerLevel>(result.Data["PlayerLevel"].Value);
                        LevelManager.Instance.SetLevelInfos(_PlayerLevel);
                        _PlayerReferral = JsonConvert.DeserializeObject<PlayerReferral>(result.Data["PlayerReferral"].Value);
                        if (_PlayerReferral.TotalFirstReferrals > 0 && GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks.Count > 0)
                        {
                            RefreshReferrals();
                        }
                        if (_TapTicketsInfo.NewReferralTapTickets > 0)
                        {
                            UIManager.Instance.InstantiateEarnedMessagerReferralPop (_TapTicketsInfo.NewReferralTapTickets.ToString());
                            var request = new ExecuteCloudScriptRequest
                            {
                                FunctionName = "newTicketReferralRefresh",
                                FunctionParameter = new {
                                    playerID = PlayFabID
                                }
                            };

                            PlayFabClientAPI.ExecuteCloudScript(request, success => {Debug.Log("Refreshed newTicketReferralRefresh");}, error => {Debug.Log("Error newTicketReferralRefresh");});
                        
                        }

                    }
                    catch (System.Exception)
                    {
                        UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                        return;
                    }
                        
                    },error => {UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game."); return;});

                    Debug.Log("Existing account logged in.");
                    PlayerName = result.InfoResultPayload?.AccountInfo?.TitleInfo?.DisplayName;
                    UIManager.Instance.ConnectedSceneUI();
                }
            },
            error =>
            {
                Debug.LogError("Error getting account info: " + error.GenerateErrorReport());
            });

            
        }
        StartCoroutine( UpdateLoginSession());
    }
    void onLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
        UIManager.Instance.DisconnectedSceneUI();

    }
    public float GetMultiplier(string mult)
    {
        float _mult = 0;
        float _max = 0;
        switch(mult)
        {
            case "C": //2.0
                _mult= 1.4f;
                _max = 2.0f;
            break;

            case "B": //2.5
                _mult= 1.5f;
                _max = 2.5f;
            break;

            case "A": //3
                _mult= 1.7f;
                _max = 3.5f;
            break;
        }
        _mult +=  _PlayerReferral.TotalReferralMultiplier; 
        _mult = Mathf.Clamp(_mult,0,_max); 
        return _mult;
    }
    public void DeletePlayerBooster(PlayerBoosterPackProtected playerBoosterPackProtected, Action action= null)
    {
        if (GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks.Contains(playerBoosterPackProtected))
        {
            GameObject loading = UIManager.Instance.LoadingShow();
            loading.transform.SetAsLastSibling();
            GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks?.Remove(playerBoosterPackProtected);
            SavePlayerBoosterPackData( () => {Destroy(loading) ; action?.Invoke();} );
        }
    }
    public void SavePlayerBoosterPackData(Action action = null)
    {
        GameManager.Instance.AbleToSavePlayerData  = false;
        PlayerGameDataUnProtected playerGameDataUnProtected = GameManager.Instance._PlayerGameDataProtected.ConvertToPlayerGameDataUnProtected();
        bool checkZeroAvailableClicks = false; 
        if (GameManager.Instance.CurrentUsedPlayerBoosterPackProtected != null)
        {
            checkZeroAvailableClicks = GameManager.Instance.CurrentUsedPlayerBoosterPackProtected.AvailableClicks <= 0; 
        }
        string serial = JsonConvert.SerializeObject(playerGameDataUnProtected);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerGameData",  serial }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            if(GameManager.Instance.CurrentUsedPlayerBoosterPackProtected != null)
            {
                if(checkZeroAvailableClicks)
                {

                    GameManager.Instance.StopISavePlayerData();
                }
            }
            Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");
            GameManager.Instance.AbleToSavePlayerData = true;
            if (action != null)
            {
                action?.Invoke();
            }
        }, error =>
        {
            GameManager.Instance.AbleToSavePlayerData = true;
            // UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
            Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
        });
    }
    public void BoughtBoosterPack(BoosterPackProtected boosterPackProtected)
    {
        int newID = 0;
        bool containsID = false;

        boosterPackProtected.TimeExpirationsProtected.ExpireTarget = (DateTimeOffset.UtcNow + TimeSpan.FromHours(2) ).ToUnixTimeMilliseconds();
        if(GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks.Count > 0)
        {
            for (int i = 0; i < 1000; i++)
            {
                bool completed = false;
                for (int x= 0; x < GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks.Count; x++)
                {
                    int id = (int) GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks[x].ID;
                    if(id == i)
                    {
                        break;
                    }
                    if(id !=  i && x == GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks.Count - 1)
                    {   
                        newID = i;
                        completed = true;
                    }
                }
                if(completed)
                {   
                    break;
                }
            }
        }
        else
        {
            newID =  0;
        }
        if(newID > 1000)
        {
            return;
        }
        float currentMultiplier =GetMultiplier(boosterPackProtected.BoosterPacksTypes);
        PlayerBoosterPackUnProtected playerBoosterPackProtected = new PlayerBoosterPackUnProtected{
            ID =  newID,
            DailyTimeExpire = 24, // like 24 hours, resets all avail clicks
            CurrentMultiplier = currentMultiplier,
            BNBEarnPerClick = ((boosterPackProtected.Price * currentMultiplier) / 14) / 50, //14 = days , 50 =  times
            AvailableClicks = 50,
            TotalBNBEarned = 0,
            
            TimeExpirationsUnProtected = new TimeExpirationsUnProtected{
                DailyResetTarget =  (Int64)boosterPackProtected.TimeExpirationsProtected.DailyResetTarget,
                ExpireTarget =  (Int64)boosterPackProtected.TimeExpirationsProtected.ExpireTarget,
            } ,
            ClickRate = new ClickRateUnProtected {
                Win = (float) boosterPackProtected.ClickRate.Win,
                Lose =(float) boosterPackProtected.ClickRate.Lose
            } , 
            ImageIndex = boosterPackProtected.ImageIndex, 
            Title = boosterPackProtected.Title, 
            Price = (float) boosterPackProtected.Price, 
            BoosterPacksTypes = (string) boosterPackProtected.BoosterPacksTypes, 
            FinalTimeExpire = (int) boosterPackProtected.FinalTimeExpire, 
            OriginalMultiplier = (float) boosterPackProtected.OriginalMultiplier
        };
        PlayerGameDataUnProtected playerGameDataUnProtected = GameManager.Instance._PlayerGameDataProtected.ConvertToPlayerGameDataUnProtected();
        playerGameDataUnProtected.OwnedBoosterPacksUnProtected.Add(playerBoosterPackProtected);

        GameManager.Instance._PlayerGameDataProtected = playerGameDataUnProtected.ConvertToPlayerGameDataProtected();
        string serial = JsonConvert.SerializeObject(playerGameDataUnProtected);
        
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerGameData",  serial }
            }
        };
        var checkBooster = new GetUserDataRequest
        {
            Keys = new List<string> { "BoughtFirstBooster"}
        };
        PlayFabClientAPI.GetUserData(checkBooster, result =>
        {
            bool boughtBooster = Convert.ToBoolean(result.Data["BoughtFirstBooster"].Value) ;
            if (!boughtBooster && WebglReferral.Instance.ReferralID != "")
            {
                var updateBooster = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "BoughtFirstBooster",  "true" }
                    }
                };

                PlayFabClientAPI.UpdateUserData(updateBooster, result =>
                {
                    Debug.Log($"updateBooster successfully. {LoginStateSession} ");
                    PlayFabClientAPI.UpdateUserData(request, result =>
                    {
                        GotReferred();
                        Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");
                    }, error =>
                    {
                        UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                        Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
                    });
                }, error =>
                {
                    UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                    Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
                });
            }
            else
            {
                PlayFabClientAPI.UpdateUserData(request, result =>
                {
                    Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");
                }, error =>
                {
                    UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                    Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
                });
            }
        },
        error =>
        {
            Debug.LogError("Error getting user data: " + error.GenerateErrorReport());
        });
    }
    public void RefreshReferrals()
    {
        PlayerGameDataUnProtected playerGameDataUnProtected = GameManager.Instance._PlayerGameDataProtected.ConvertToPlayerGameDataUnProtected();
        foreach (var item in playerGameDataUnProtected.OwnedBoosterPacksUnProtected)
        {
            float currentMultiplier =GetMultiplier(item.BoosterPacksTypes);
            item.CurrentMultiplier = currentMultiplier;
            item.BNBEarnPerClick = ((item.Price * currentMultiplier) / 14) / 50; //14 = days , 50 =  times
        }
        GameManager.Instance._PlayerGameDataProtected = playerGameDataUnProtected.ConvertToPlayerGameDataProtected();

        string serial = JsonConvert.SerializeObject(playerGameDataUnProtected);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerGameData",  serial }
            }
        };
        
        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");
        }, error =>
        {
            UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
            Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
        });
    }
    public void SetNickname()
    {
        if(UIManager.Instance.NameUI.text.Count() > 0)
        {
            Debug.Log($"New account created!");
            PlayerGameDataUnProtected playerGameDataUnProtected = new PlayerGameDataUnProtected{
                OwnedBoosterPacksUnProtected = new List<PlayerBoosterPackUnProtected>()
            };
            
            string serial = JsonConvert.SerializeObject(playerGameDataUnProtected);
            _PlayerReferral =   new PlayerReferral{
                TotalReferrals = 0,
                TotalReferralMultiplier = 0,
                TotalFirstReferrals = 0,
                TotalSecondReferrals = 0,
                TotalThirdReferrals = 0
            };

            _TapTicketsInfo = new TapTicketsInfo{
                NewReferralTapTickets = 0,
                CurrentTapTickets = 0
            } ;
            _PlayerLevel = new PlayerLevel{
                CurrentLevel = 1, 
                BaseXP = 50,
                CurrentXP = 0,
                RequiredXP = 50
            } ;
            LevelManager.Instance.SetLevelInfos(_PlayerLevel);
            string serial_PlayerReferral = JsonConvert.SerializeObject(_PlayerReferral);
            string serial_TapTicketsInfo = JsonConvert.SerializeObject(_TapTicketsInfo);
            string serial_PlayerLevel = JsonConvert.SerializeObject(_PlayerLevel);

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "PlayerGameData",  serial },
                    { "PlayerReferral",  serial_PlayerReferral },
                    { "TapTicketsInfo",  serial_TapTicketsInfo },
                    { "PlayerLevel",  serial_PlayerLevel },
                    { "BoughtFirstBooster",  "false" },
                    { "ReferredBy",  "" },
                }
            };

            
            PlayFabClientAPI.UpdateUserData(request, result =>
            {
                    GameManager.Instance._PlayerGameDataProtected = playerGameDataUnProtected.ConvertToPlayerGameDataProtected();

                    var nicknameRequest = new UpdateUserTitleDisplayNameRequest
                    {
                        DisplayName = UIManager.Instance.NameUI.text
                    };
                    Action checkReferralAfterNickname = () => 
                    {
                        PlayFabClientAPI.UpdateUserTitleDisplayName(nicknameRequest, (Action<UpdateUserTitleDisplayNameResult>)(result =>
                        {
                            if (WebglReferral.Instance.ReferralID == "" )
                            {
                                UIManager.Instance.InstantiateNewbieReferral();
                            }
                            else
                            {
                                UIManager.Instance.InstantiateMessagerReferralPop("10");
                            }
                            Debug.Log("Nickname successfully set to: " + result.DisplayName);
                            PlayerName = result.DisplayName;
                            UIManager.Instance.ConnectedSceneUI();
                        }), error =>
                        {
                            Debug.LogError("Failed to set nickname: " + error.GenerateErrorReport());
                        });
                    };
                    if (WebglReferral.Instance.ReferralID != "" )
                    {
                        ValidateReferralCode(WebglReferral.Instance.ReferralID, () => Debug.Log("Check "),  ()  => checkReferralAfterNickname?.Invoke () );
                    }
                    else
                    {
                        checkReferralAfterNickname?.Invoke ();
                    }
                    Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");

            }, error =>
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
                return;
            });


            UIManager.Instance.NameContainerUI.SetActive(false);
          
            
        }
    }

    public void GetLoginSession()
    {
        LoginSessionCheckStatus = "";
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null  )
            {
                if(result.Data.Count > 0)
                {
                   if(result.Data.ContainsKey("LoginSessionID"))
                   {
                        LoginStateUpdatedSession = result.Data["LoginSessionID"].Value;
                        if(!string.IsNullOrEmpty(LoginStateSession))
                        {
                            if(LoginStateUpdatedSession != LoginStateSession)
                            {
                                Debug.Log("Session Login ID has expired.");
                                LoginSessionCheckStatus = "-1";
                            }
                            else
                            {
                                Debug.Log("Session Login ID is still usable.");
                                LoginSessionCheckStatus = "1";
                            }
                        }
                        else
                        {
                            Debug.Log("Session Login ID is still usable.");
                            LoginSessionCheckStatus = "1";
                        }

                   }
                   else
                   {
                        Debug.Log("Session Login ID is still usable.");
                        LoginSessionCheckStatus = "1";
                        LoginStateUpdatedSession = "-1";
                        Debug.Log("No user data found.");
                   }
                }
                else
                {
                    Debug.Log("Session Login ID is still usable.");
                    LoginSessionCheckStatus = "1";
                    LoginStateUpdatedSession = "-1";
                }
            }
            else
            {
                Debug.Log("Session Login ID is still usable.");
                LoginSessionCheckStatus = "1";
                LoginStateUpdatedSession = "-1";
                Debug.Log("No user data found.");
            }
        }, error =>
        {
            UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
            Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
        });

    }
    public IEnumerator UpdateLoginSession()
    {
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        GetLoginSession();
        while(string.IsNullOrEmpty( LoginStateUpdatedSession))
            yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(1f);
        Destroy(loading);
        if(LoginStateUpdatedSession != "-1") //if LoginStateSession exists already
        {
            string newLoginID = LoginStateUpdatedSession;
            while(newLoginID == LoginStateUpdatedSession)
            {
                newLoginID =  UnityEngine.Random.Range(0,10000).ToString();
                yield return new WaitForEndOfFrame();
            }
            LoginStateSession = newLoginID;
        }
        else
        {
            LoginStateSession =  UnityEngine.Random.Range(0,10000).ToString();
        }

        LoginStateUpdatedSession = LoginStateSession;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "LoginSessionID",  LoginStateSession }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, result =>
        {
            Debug.Log($"Player data ID updated successfully. {LoginStateSession} ");
        }, error =>
        {
            UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
            Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
        });
    }
    public IEnumerator IExecuteWithSessionCheck(System.Action action)
    {
        // Start session check
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        PlayFabManager.Instance.GetLoginSession();
        // Wait for session status to update
        while (PlayFabManager.Instance.LoginSessionCheckStatus == "")
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        Destroy(loading);
        // Handle session status
        if (PlayFabManager.Instance.LoginSessionCheckStatus == "1")
        {
            Debug.Log("Your login session is not expired.");
            action?.Invoke(); // Execute the passed action
        }
        else if (PlayFabManager.Instance.LoginSessionCheckStatus == "-1")
        {
            UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Your login session has now expired, please login again.");
            Debug.Log("Your login session has now expired, please login again.");
            // Handle expired session (e.g., redirect to login)
        }
    }
    public void ExecuteWithSessionCheck(System.Action action) => StartCoroutine(IExecuteWithSessionCheck(action));

}
