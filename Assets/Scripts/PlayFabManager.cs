using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
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
        if (result.NewlyCreated)
        {
            Debug.Log($"New account created!");
            UIManager.Instance.NameContainerUI.SetActive(true);
        }
        else
        {
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
            Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
        }); 
        
    }
    public IEnumerator UpdateLoginSession()
    {
        GetLoginSession();
        while(string.IsNullOrEmpty( LoginStateUpdatedSession))
            yield return new WaitForEndOfFrame();

        if(LoginStateUpdatedSession != "-1") //if LoginStateSession exists already
        {
            string newLoginID = LoginStateUpdatedSession;
            while(newLoginID == LoginStateUpdatedSession)
            {
                newLoginID = Random.Range(0,10000).ToString();
                yield return new WaitForEndOfFrame();
            }
            LoginStateSession = newLoginID;
        }
        else
        {
            LoginStateSession = Random.Range(0,10000).ToString();
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
            Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
        });
    }
    public IEnumerator IExecuteWithSessionCheck(System.Action action)
    {
        // Start session check
        PlayFabManager.Instance.GetLoginSession();

        // Wait for session status to update
        while (PlayFabManager.Instance.LoginSessionCheckStatus == "")
            yield return new WaitForEndOfFrame();

        // Handle session status
        if (PlayFabManager.Instance.LoginSessionCheckStatus == "1")
        {
            Debug.Log("Your login session is not expired.");
            action?.Invoke(); // Execute the passed action
        }
        else if (PlayFabManager.Instance.LoginSessionCheckStatus == "-1")
        {
            Debug.Log("Your login session has now expired, please login again.");
            // Handle expired session (e.g., redirect to login)
        }
    }
    public void ExecuteWithSessionCheck(System.Action action) => StartCoroutine(IExecuteWithSessionCheck(action));
    
}