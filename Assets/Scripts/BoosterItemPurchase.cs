using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;

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
        float newPrice = _BoosterPackProtected.Price / 0.0001428571f;
        TapTicketPrice = (ProtectedUInt32)newPrice ; 
        ValueTap.text = TapTicketPrice.ToString();
        Get.onClick.AddListener(()=>
        {
            StartCoroutine(getBooster());
        });
        TitleUI.text = _BoosterPackProtected.Title;
        PriceUI.text = $"{_BoosterPackProtected.Price} BNB";
        DescriptionUI.text = $"BUY {_BoosterPackProtected.Price} BNB AND GET UP TO {_BoosterPackProtected.Price *_BoosterPackProtected.OriginalMultiplier } BNB IN ONLY {_BoosterPackProtected.FinalTimeExpire} DAYS BY CLICKING 50 TIMES PER DAY";
        MAXMultiplierUI.text = $"MAX MULTIPLIER : {_BoosterPackProtected.OriginalMultiplier}x";
        //0.0001428571


    }
    IEnumerator getBooster()
    {
        yield return _EVM.Instance.StartCoroutine(_EVM.Instance.IGetNativeBalanceOf());
        float playerBalanceBNB = float.Parse( UIManager.Instance.BNBUI.text);
        if(playerBalanceBNB >= _BoosterPackProtected.Price)
        {
            GameObject loading = UIManager.Instance.LoadingShow();
            loading.transform.SetAsLastSibling();
            
            var deposit = _EVM.Instance.DepositAmount(_BoosterPackProtected.Price, ()=> {
                _EVM.Instance.GetNativeBalanceOf();
                PlayFabManager.Instance.BoughtBoosterPack(_BoosterPackProtected);
                loading.SetActive(false);
            });
            yield return new WaitUntil(() => deposit.IsCompleted); 
            Debug.Log("Transaction complete!");
        }
        else 
        {
            Debug.Log("No money");

        }
    }
}
