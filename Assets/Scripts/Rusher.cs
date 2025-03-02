using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using GUPS.AntiCheat.Protected;
public class Rusher : MonoBehaviour
{
    public bool Explode;
    UnityEngine.UI.Button button;
    void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => 
        {
            // GameManager.Instance.TestAvailClicks -= 1;
            PlayerBoosterPackProtected playerBoosterPackProtected = GameManager.Instance.CurrentUsedPlayerBoosterPackProtected;
            GameManager.Instance.CurrentUsedPlayerBoosterPackProtected.AvailableClicks -= 1;
            
            playerBoosterPackProtected.AvailableClicks = new ProtectedFloat( Mathf.Clamp(playerBoosterPackProtected.AvailableClicks,0,50));

            if(Explode) // means no point / bnb
            {
                GameObject explode = Instantiate(GameManager.Instance.PrefabBomb,GameManager.Instance.MaskGameContainer) ;
                explode.transform.position = transform.position;
                Destroy(explode,0.6f);
            }
            else
            {
                GameObject effect = Instantiate( GameManager.Instance.ListEffectRush[Random.Range(0,GameManager.Instance.ListEffectRush.Count)],GameManager.Instance.MaskGameContainer) ;
                GameObject uiBNB = Instantiate( GameManager.Instance.PrefabBNBUIText,GameManager.Instance.MaskGameContainer.parent) ;
                effect.transform.position = transform.position;
                uiBNB.transform.position = transform.position;
                Text uiBNBText = uiBNB.GetComponent<Text>();
                uiBNBText.DOFade(0,1.5f).SetEase(Ease.Linear);


                float bnbEarned = playerBoosterPackProtected.BNBEarnPerClick;
                uiBNBText.text = $"{bnbEarned.ToString("0.0000000")} BNB";

                playerBoosterPackProtected.TotalBNBEarned += bnbEarned;
                uiBNB.transform.DOMoveY(uiBNB.transform.position.y * 1.5f,2f ).OnComplete(() =>
                {
                    uiBNBText.DOKill();
                    uiBNB.transform.DOKill();
                    Destroy(uiBNB);
                }).SetEase(Ease.Linear);;
                Destroy(effect,0.15f);
            }

            UIManager.Instance.UpdateUIClicks((int)playerBoosterPackProtected.AvailableClicks,(float)playerBoosterPackProtected.TotalBNBEarned );

            GameManager.Instance.ListOfOnSavePlayerData.Add( () => PlayFabManager.Instance.SavePlayerBoosterPackData());
            transform.DOKill();
            Destroy(gameObject);

        } );
    }
}
