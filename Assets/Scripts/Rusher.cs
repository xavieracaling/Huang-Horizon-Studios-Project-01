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
            GameManager.Instance.TestAvailClicks -= 1;

            if(Explode)
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
                
                float bnbEarned = Random.Range(0.1f,0.5f);
                uiBNBText.text = $"{bnbEarned.ToString("0.00000")} BNB";

                GameManager.Instance.TestTotalBNBEarned += bnbEarned;
                uiBNB.transform.DOMoveY(uiBNB.transform.position.y * 1.5f,2f ).OnComplete(() =>
                {
                    uiBNBText.DOKill();
                    uiBNB.transform.DOKill();
                    Destroy(uiBNB);
                }).SetEase(Ease.Linear);;
                Destroy(effect,0.15f);
            }
                UIManager.Instance.UpdateUIClicks((int)GameManager.Instance.TestAvailClicks,(float)GameManager.Instance.TestTotalBNBEarned );

            transform.DOKill();
            Destroy(gameObject);

        } );
    }
}
