using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using Newtonsoft.Json;
using System;
public class PlayFabManager : MonoBehaviour
{
    public string LoginSessionCheckStatus;
    public string PlayerName;
    public string LoginStateSession;
    public string LoginStateUpdatedSession;
    public string CustomUserIDAddress;
    public static PlayFabManager Instance;
    void Awake()
    {
        Instance = this;
        // PlayerBoosterPackProtected boosterPackProtected = new PlayerBoosterPackProtected{
        //     ID = 9999,
        //     DailyTimeExpire = 5464,
        //     BNBEarnPerClick = 0.06f,
        //     FinalTimeExpire = 89,
        //     BoosterPacksTypes = BoosterPacksTypes.C.ToString(),
        //     OriginalMultiplier = 5,
        //     ClickRate =  new ClickRateProtected{
        //         Win = 80,
        //         Lose = 20,
        //     }
        // };
        // GameManager.Instance._PlayerGameDataProtected = new PlayerGameDataProtected(){
        //     OwnedBoosterPacks = new List<PlayerBoosterPackProtected>(){
        //         boosterPackProtected
        //     },

        // };
        // PlayerGameDataUnProtected playerGameDataUnProtected = GameManager.Instance._PlayerGameDataProtected.ConvertToPlayerGameDataUnProtected();
        // string serPlayerGameDataUnProtected = JsonConvert.SerializeObject(playerGameDataUnProtected,Formatting.Indented);

        // Debug.Log($"serPlayerGameDataUnProtected {serPlayerGameDataUnProtected} ");


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
                    GameManager.Instance.FinalStartGame(playerBoosterPackProtected);
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

    void onLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        Debug.Log("PlayFab ID: " + result.PlayFabId);
        if (result.NewlyCreated) // new created
        {
            Debug.Log($"New account created!");
            PlayerGameDataUnProtected playerGameDataUnProtected = new PlayerGameDataUnProtected{
                OwnedBoosterPacksUnProtected = new List<PlayerBoosterPackUnProtected>()
            };

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
                GameManager.Instance._PlayerGameDataProtected = playerGameDataUnProtected.ConvertToPlayerGameDataProtected();
                Debug.Log($"Player GameData updated successfully. {LoginStateSession} ");
            }, error =>
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Restart("Server error, please restart the game.");
                Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
            });
            
            UIManager.Instance.NameContainerUI.SetActive(true);
        }
        else
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => {
                if(result.Data.ContainsKey("PlayerGameData"))
                {
                    PlayerGameDataUnProtected playerGameDataProtected = JsonConvert.DeserializeObject<PlayerGameDataUnProtected>(result.Data["PlayerGameData"].Value);
                    GameManager.Instance._PlayerGameDataProtected = playerGameDataProtected.ConvertToPlayerGameDataProtected();
                }
            },error => {Debug.Log("Error");});
            Debug.Log("Existing account logged in.");
            PlayerName = result.InfoResultPayload?.AccountInfo?.TitleInfo?.DisplayName;
            UIManager.Instance.ConnectedSceneUI();
        }
        StartCoroutine( UpdateLoginSession());
    }
    void onLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
        UIManager.Instance.DisconnectedSceneUI();

    }
    float getMultiplier(string mult)
    {
        float _mult = 0;
        float _max = 0;
        switch(mult)
        {
            case "C": //2.0
                _mult= 1.4f;
                _max = 2.0f;
            break;

            case "B": //3x
                _mult= 1.5f;
                _max = 3.0f;
            break;

            case "A": //4x
                _mult= 1.7f;
                _max = 4.0f;
            break;
        }
        _mult += GameManager.Instance._PlayerGameDataProtected.TotalReferralMultiplierPoints; 
        _mult = Mathf.Clamp(_mult,0,_max); 
        return _mult;
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
        float currentMultiplier =getMultiplier(boosterPackProtected.BoosterPacksTypes);
        int newID = 0;
        bool containsID = false;
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
        PlayerBoosterPackUnProtected playerBoosterPackProtected = new PlayerBoosterPackUnProtected{
            ID =  newID,
            DailyTimeExpire = 24, // like 24 hours, resets all avail clicks
            CurrentMultiplier = currentMultiplier,
            BNBEarnPerClick = ((boosterPackProtected.Price * currentMultiplier) / 30) / 50, //30 = days , 50 =  times
            AvailableClicks = 50,
            TotalBNBEarned = 0,

            ClickRate = new ClickRateUnProtected {
                Win = boosterPackProtected.ClickRate.Win,
                Lose = boosterPackProtected.ClickRate.Lose
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
            UIManager.Instance.NameContainerUI.SetActive(false);
            var nicknameRequest = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = UIManager.Instance.NameUI.text
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(nicknameRequest, result =>
            {
                Debug.Log("Nickname successfully set to: " + result.DisplayName);
                PlayerName = result.DisplayName;
                UIManager.Instance.ConnectedSceneUI();
            }, error =>
            {
                Debug.LogError("Failed to set nickname: " + error.GenerateErrorReport());
            });
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
