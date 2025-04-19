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
            sliderAnimationSync(oldRequiredXP,oldXP,true);
        }
    }
    public ProtectedInt32 RequiredXP {
        get => requiredXP;
        set
        {
            oldRequiredXP = requiredXP;
            requiredXP = value;
        }
    }
    public int oldRequiredXP;
    Coroutine CxpSliderAnimation;
    void Awake()
    {
        ExpProgress += updateXPProgress;
    }
    void OnEnable()
    {
        ExpProgress?.Invoke();
        oldRequiredXP = RequiredXP;
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
        Debug.Log($"newXP 1 {newXP}" );
        
        float target = (float)newXP / (float)currentRequiredXP;
        if (animate) // doing a lot of tempo
        {
            int tempoRequiredXP = currentRequiredXP;
            
            if (CurrentXP >= tempoRequiredXP)
            {
                newXP =  newXP - (newXP - tempoRequiredXP ) ;
                CurrentLevelUI.text = $"LEVEL: {CurrentLevel - 1}" ;
                target = (float)newXP / (float)currentRequiredXP;
            }

            //Debug.Log($"newXP 2 {newXP}" );

            float startValue = XPSlider.value;
            float elapsedTime = 0f;
            float currentXP = 0;
            //Debug.Log($"animate oldCurrentXP {oldCurrentXP}, currentRequiredXP {currentRequiredXP}, newXP {newXP}, target {target}" );

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                XPSlider.value = Mathf.Lerp(startValue, target, t);

                currentXP = Mathf.Lerp(oldCurrentXP, newXP, t);
                XPCurrentMaxUI.text = $"{(int)currentXP}/{tempoRequiredXP}";
          //      Debug.Log($"1 animate oldCurrentXP {oldCurrentXP}, currentRequiredXP {currentRequiredXP}, newXP {newXP}, target {target}" );

                yield return null;
            }
            XPSlider.value = target;
            
            if (CurrentXP >= tempoRequiredXP)
            {
                oldRequiredXP = RequiredXP;
                CurrentLevelUI.text = $"LEVEL: {CurrentLevel}" ;
                XPCurrentMaxUI.text = $"{CurrentXP}/{RequiredXP}";
                XPSlider.value = 0;
                
                int _oldXP = (int)currentXP;
                sliderAnimationSync(RequiredXP,_oldXP,true,0.5f); // level up

            }
           // Debug.Log("animate done");

        }
        else
        {
            XPSlider.value = target;
            Debug.Log("not animate");

        }
    }

}
