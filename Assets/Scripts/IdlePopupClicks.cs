using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class IdlePopupClicks : MonoBehaviour
{
    public Text PopupPrefab;
    public Color Green;
    public Color Red;
    public static IdlePopupClicks Instance;
    Vector2 currentSize ;
    Vector2 newSize ;

    void Awake()
    {
        Instance = this;
        currentSize = PopupPrefab.transform.localScale;
        newSize = PopupPrefab.transform.localScale *1.15f;
    } 
    public void ShowPopup(bool plus)
    {
        PopupPrefab.color = new Color(PopupPrefab.color.r, PopupPrefab.color.g, PopupPrefab.color.b, 1);
        PopupPrefab.color = plus == true ? Green : Red;
        // Reset alpha before fade
        PopupPrefab.text = plus == true ? $"+ {AdventureMode.Instance.CurrentClicks.ToString()}" : $"- {AdventureMode.Instance.CurrentClicks.ToString()}" ;
        // Fade and deactivate after
        PopupPrefab.DOKill();
        PopupPrefab.transform.DOScale(newSize, 0.3f).SetEase(Ease.InOutSine).OnComplete(() => 
        { 
            PopupPrefab.transform.DOScale(currentSize, 0.5f).SetEase(Ease.InOutSine);
        });
        PopupPrefab.DOFade(0f, 1f);

    }
    
    
}
