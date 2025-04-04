using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterItemPurchase : MonoBehaviour
{
    public Button Get;
    public Text DescriptionUI;
    public Text TitleUI;
    public Text PriceUI;
    public Text MAXMultiplierUI;
    public BoosterPackProtected _BoosterPackProtected;
    void Awake()
    {
        Initialize() ;
    }
    public async void Initialize() 
    {
        Get.onClick.AddListener(()=>
        {
            StartCoroutine(getBooster());
        });
        TitleUI.text = _BoosterPackProtected.Title;
        PriceUI.text = $"{_BoosterPackProtected.Price} BNB";
        DescriptionUI.text = $"BUY {_BoosterPackProtected.Price} BNB AND GET UP TO {_BoosterPackProtected.Price *_BoosterPackProtected.OriginalMultiplier } BNB IN ONLY {_BoosterPackProtected.FinalTimeExpire} DAYS BY CLICKING 50 TIMES PER DAY";
        MAXMultiplierUI.text = $"MAX MULTIPLIER : {_BoosterPackProtected.OriginalMultiplier}x";
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
