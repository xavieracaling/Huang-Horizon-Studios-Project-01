using System;
using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelXPBar : MonoBehaviour
{
    [Header("UI")]
    public Slider XPSlider;
    public Text XPCurrentMaxUI;
    public Text CurrentLevelUI;
    [Header("Values")]
    ProtectedInt32 currentLevel;
    ProtectedInt32 baseXP;
    ProtectedInt32 currentXP;
    ProtectedInt32 requiredXP;
    public  Action ExpProgress;
    public ProtectedInt32 CurrentLevel {
        get => currentLevel;
        set
        {
            currentLevel = value;
            CurrentLevelUI.text = $"LEVEL: {currentLevel}" ;
        }
    }
    public ProtectedInt32 BaseXP {
        get => baseXP;
        set
        {
            baseXP = value;
        }
    }
    public ProtectedInt32 CurrentXP {
        get => currentXP;
        set
        {
            int oldXP = currentXP;
            currentXP = value;
            ExpProgress?.Invoke();
            sliderAnimationSync(OldRequiredXP,oldXP,true);
        }
    }
    public ProtectedInt32 RequiredXP {
        get => requiredXP;
        set
        {
            OldRequiredXP = requiredXP;
            requiredXP = value;
        }
    }
    public int OldRequiredXP;
    Coroutine CxpSliderAnimation;
    void Awake()
    {
        ExpProgress += updateXPProgress;
    }
    void OnEnable()
    {
        ExpProgress?.Invoke();
        OldRequiredXP = RequiredXP;
        sliderAnimationSync(RequiredXP,CurrentXP, false);
    }
    void Start()
    {
    }

    void updateXPProgress()
    {
        XPCurrentMaxUI.text = $"{CurrentXP}/{RequiredXP}";

    }
    void sliderAnimationSync(int currentRequiredXP,int oldXP,bool animate = false,float duration = 0.8f)
    {
        if (!gameObject.activeInHierarchy) return;
        if (CxpSliderAnimation != null)
        {
            StopCoroutine(CxpSliderAnimation);
        }
        CxpSliderAnimation = StartCoroutine (IxpSliderAnimation(currentRequiredXP,oldXP,animate,duration));
    }

    IEnumerator IxpSliderAnimation(int currentRequiredXP,int oldCurrentXP,bool animate = true,float duration = 0.8f)
    {
        float newXP = (float)CurrentXP;

        float target = (float)newXP / (float)currentRequiredXP;
        if (animate) // doing a lot of tempo
        {

            int tempoRequiredXP = currentRequiredXP;

            if (RequiredXP != OldRequiredXP)
            {
                newXP =  currentRequiredXP ;
                CurrentLevelUI.text = $"LEVEL: {CurrentLevel - 1}" ;
                target = (float)newXP / (float)currentRequiredXP;
            }


            float startValue = XPSlider.value;
            float elapsedTime = 0f;
            float currentXP = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                XPSlider.value = Mathf.Lerp(startValue, target, t);
                currentXP = Mathf.Lerp(oldCurrentXP, newXP, t);
                XPCurrentMaxUI.text = $"{(int)currentXP}/{tempoRequiredXP}";

                yield return null;
            }
            XPSlider.value = target;
            if (RequiredXP != OldRequiredXP) // leveled up
            {
                OldRequiredXP = RequiredXP;
                CurrentLevelUI.text = $"LEVEL: {CurrentLevel}" ;
                XPCurrentMaxUI.text = $"{CurrentXP}/{RequiredXP}";
                XPSlider.value = 0;
                if (!LevelManager.Instance.LeveledUpPanel)
                {
                    LevelManager.Instance.LeveledUpPanel = true;
                    UIManager.Instance.MainMenuBTNGO.SetActive(false);
                    UIManager.Instance.InstantiateLevelupMessagerPop(CurrentLevel,LevelManager.Instance.LatestGetXPResult.TapTicketsEarned);
                    PlayFabManager.Instance._TapTicketsInfo.CurrentTapTickets += LevelManager.Instance.LatestGetXPResult.TapTicketsEarned;
                    UIManager.Instance.UpdateUITapTickets();
                }
                int _oldXP = 0;
                if (CurrentXP > 0)
                {
                    sliderAnimationSync(RequiredXP,_oldXP,true,0.5f); // level up
                }

            }

        }
        else
        {
            XPSlider.value = target;

        }
    }

}
