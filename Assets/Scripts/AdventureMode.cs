using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class AdventureMode : MonoBehaviour
{
    public static AdventureMode Instance;
    public Transform EnemyContainer;
    public Transform PointCenter;
    public List<GameObject>ListEnemy = new List<GameObject>();
    public List<Transform> ListEnemySpawnPoints = new List<Transform>();
    public Coroutine CTimeStarted;
    public Coroutine CSpawnEnemies;

    [Header("ColorsRandomEnemy")]
    public List<Color> ColorsRandomEnemy = new List<Color>();
    [Header("UI")]
    public Text CompletionXPGainUI;
    public Text ClicksRequiredUI;
    public Text CurrentClicksUI;
    public Text TimeCountdownUI;
    [Header("Values")]
    ProtectedInt32 clicksRequired;
    ProtectedInt32 currentClicks;
    ProtectedInt32 timeCountdown;
  
    public ProtectedInt32 ClicksRequired {
        get => clicksRequired;
        set 
        {
            clicksRequired = value;
            ClicksRequiredUI.text = clicksRequired.ToString();
        } 
    }
    public ProtectedInt32 CurrentClicks {
        get => currentClicks;
        set 
        {
            currentClicks = value;
            CurrentClicksUI.text = currentClicks.ToString();
        } 
    }
    public ProtectedInt32 TimeCountdown {
        get => timeCountdown;
        set 
        {
            timeCountdown = value;
            timeSpan = TimeSpan.FromSeconds(timeCountdown);
            TimeCountdownUI.text = $"{timeSpan.Minutes}:{timeSpan.Seconds}";
        } 
    }
    public float MaxTime;
    TimeSpan timeSpan;
    public Vector2 CurrentIdleSize;
    public Vector2 NewIdleSize;
    public Image ImageIdle;
    Animator animatorIdle;
    void Awake()
    {
        Instance = this;
        CurrentIdleSize = PointCenter.transform.localScale;
        NewIdleSize = PointCenter.transform.localScale * 1.3f;
        animatorIdle = PointCenter.GetComponent<Animator>();
        ImageIdle = PointCenter.GetComponent<Image>();
        PointCenter.GetComponent<Button>().onClick.AddListener(() => {
            IdleClick();
        });
    }
    public void IdleClick()
    {
        CurrentClicks++;
        if (!animatorIdle.GetBool("Jump"))
        {
            animatorIdle.SetTrigger("Jump");
        }
        PointCenter.DOScale(NewIdleSize,0.2f).SetEase(Ease.OutSine).OnComplete(() => {
            PointCenter.DOScale(CurrentIdleSize,0.15f).SetEase(Ease.OutSine);
        });
    }
    public void IdleGotDamaged()
    {
        CurrentClicks--;
        if (CurrentClicks <= 0)
        {
            CurrentClicks = 0;
        }
        ImageIdle.DOKill();
        ImageIdle.DOColor(Color.red, 0.5f)
        .SetLoops(2, LoopType.Yoyo)
        .OnComplete(() => ImageIdle.DOColor(Color.white, 0.25f));
    }
    public void SetAdventureModeState(int minXP, int maxXP, int requiredClicks, int time)
    {
        ImageIdle.color = Color.white;
        CompletionXPGainUI.text = $"{minXP} XP - {maxXP} XP";
        ClicksRequired = requiredClicks;
        timeSpan = TimeSpan.FromSeconds(time);
        MaxTime = time;
        TimeCountdown = time;
        TimeCountdownUI.DOKill();
        TimeCountdownUI.DOFade(1,1.2f);
        TimeCountdownUI.DOColor(Color.white,1.5f);
    }
    [ContextMenu("GameCompleted")]
    public void GameCompleted()
    {
        LevelManager.Instance.XPGain();
    }
    [ContextMenu("GameStart")]
    public void GameStart()
    {
        TimeCountdownUI.DOKill();
        TimeCountdownUI.DOFade(0,1.5f).SetLoops(-1,LoopType.Yoyo);
        TimeCountdownUI.DOColor(Color.red,1.2f).SetLoops(-1,LoopType.Yoyo);
        if (CTimeStarted != null)
        {
            StopCoroutine(CTimeStarted);
        }
        if (CSpawnEnemies != null)
        {
            StopCoroutine(CSpawnEnemies);
        }
        CTimeStarted = StartCoroutine(ITimeStarted());
        CSpawnEnemies = StartCoroutine(ISpawnEnemies(1));
    }
    public IEnumerator ITimeStarted()
    {
        while (TimeCountdown >= 0)
        {
            TimeCountdown --;
            if (TimeCountdown <= 0)
            {
                break;
            }
            yield return new WaitForSeconds(1);
        }
        TimeCountdownUI.DOKill();
        TimeCountdownUI.DOFade(1,0.5f);
        TimeCountdownUI.DOColor(Color.red,0.3f);
        TimeCountdown = 0;
    }
    public IEnumerator ISpawnEnemies(float spawnTime)
    {
        float currentSpawnTime = 0;
        while (TimeCountdown >= 0)
        {
            currentSpawnTime += 1f * Time.deltaTime;
            if (currentSpawnTime >= spawnTime)
            {
                GameObject enemy = Instantiate(ListEnemy[UnityEngine.Random.RandomRange(0,ListEnemy.Count)],ListEnemySpawnPoints[UnityEngine.Random.RandomRange(0,ListEnemySpawnPoints.Count)].position,Quaternion.identity,EnemyContainer);
                RusherAdventure rusherAdventure = enemy.GetComponent<RusherAdventure>();
                
                float timeInterp = (float )TimeCountdown / MaxTime;
                float newSpeed = Mathf.Lerp( 300,  198.8f,timeInterp);
                rusherAdventure.MoveSpeed = newSpeed;
                rusherAdventure.Image.color = ColorsRandomEnemy[UnityEngine.Random.RandomRange(0,ColorsRandomEnemy.Count)];
                currentSpawnTime = 0;
            }
            yield return null;
        }
    }
}
