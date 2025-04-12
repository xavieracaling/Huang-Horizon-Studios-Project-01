using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;
using ChainSafe.Gaming.UnityPackage;
using ChainSafe.Gaming.Web3;
using ChainSafe.Gaming.Evm.Providers;
using Nethereum.Hex.HexTypes;
using PlayFab.ClientModels;
using PlayFab;
using System;
public class BoosterItemPurchase : MonoBehaviour
{
    public Button Get;
    public Button Redeem;
    public Text DescriptionUI;
    public Text TitleUI;
    public Text PriceUI;
    public Text MAXMultiplierUI;
    public Text ValueTap;
    public BoosterPackProtected _BoosterPackProtected;
    public ProtectedUInt32 TapTicketPrice;
    void Awake()
    {
        Initialize() ;
    }
    public async void Initialize() 
    {
        ProtectedFloat newPrice = _BoosterPackProtected.Price / 0.0001428571f;
        TapTicketPrice = (ProtectedUInt32)(float)newPrice ; 
        ValueTap.text = TapTicketPrice.ToString();
        Get.onClick.AddListener(()=>
        {
            StartCoroutine(getBoosterVIABNB());
        });
        Redeem.onClick.AddListener(()=>
        {
            getBoosterVIATickets();
        });
        TitleUI.text = _BoosterPackProtected.Title;
        PriceUI.text = $"{_BoosterPackProtected.Price} BNB";
        DescriptionUI.text = $"BUY {_BoosterPackProtected.Price} BNB AND GET UP TO {_BoosterPackProtected.Price *_BoosterPackProtected.OriginalMultiplier } BNB IN ONLY {_BoosterPackProtected.FinalTimeExpire} DAYS BY CLICKING 50 TIMES PER DAY";
        MAXMultiplierUI.text = $"MAX MULTIPLIER : {_BoosterPackProtected.OriginalMultiplier}x";
        //0.0001428571


    }
    IEnumerator getBoosterVIABNB()
    {
        var getNativeBalanceOf = Web3Unity.Web3.RpcProvider.GetBalance(Web3Unity.Instance.Address);
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();

        yield return new WaitUntil(() => getNativeBalanceOf.IsCompleted);
        yield return new WaitForSeconds(1f);

        HexBigInteger balance =   getNativeBalanceOf.Result;
        ProtectedFloat wei = float.Parse(balance.ToString());
        ProtectedFloat decimals = 1000000000000000000; // 18 decimals
        ProtectedFloat eth = wei / decimals;
        ProtectedFloat playerBalanceBNB = eth;

        if(playerBalanceBNB >= _BoosterPackProtected.Price)
        {
            var deposit = _EVM.Instance.DepositAmount(_BoosterPackProtected.Price, ()=> {
                PlayFabManager.Instance.BoughtBoosterPack(_BoosterPackProtected);
            });
            yield return new WaitUntil(() => deposit.IsCompleted); 
            Destroy(loading);

            Debug.Log("Transaction complete!");
        }
        else 
        {
            Debug.Log("No money");
            UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Failed to buy booster. Not enough BNB") ;
            Destroy(loading);
            
        }
    }
    void getBoosterVIATickets()
    {
     
        GameObject loading = UIManager.Instance.LoadingShow();
        loading.transform.SetAsLastSibling();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "buyBoosterViaTicket",
            FunctionParameter = new {
                playerId = PlayFabManager.Instance.PlayFabID,
                itemPriceTicket = (Int64)TapTicketPrice,
            }

        };
        PlayFabClientAPI.ExecuteCloudScript(request,result   => {
            bool bought = Convert.ToBoolean(result.FunctionResult);
            Destroy(loading);
            if (bought)
            {
                PlayFabManager.Instance._TapTicketsInfo.CurrentTapTickets -= (int)(Int64)TapTicketPrice;
                if (PlayFabManager.Instance._TapTicketsInfo.CurrentTapTickets < 0)
                {
                    PlayFabManager.Instance._TapTicketsInfo.CurrentTapTickets = 0;
                }
                PlayFabManager.Instance.BoughtBoosterPack(_BoosterPackProtected);
                UIManager.Instance.UpdateUITapTickets();
                UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Bought success! ") ;
            }
            else
            {
                UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Failed to buy booster. Not enough tickets or something went wrong") ;
            }
            Destroy(loading);

        }, error   => {
            Destroy(loading);
            UIManager.Instance.InstantiateMessagerPopPrefab_Message($"Failed to buy booster. Not enough tickets or something went wrong") ;
        });
  
      
     
        
            
        
    }
   
}
