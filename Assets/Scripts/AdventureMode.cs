using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Unity.Mathematics;
public class AdventureMode : MonoBehaviour
{
    public static AdventureMode Instance;
    public GameObject GameStartBTNGO;
    public Transform EnemyContainer;
    public Transform PointCenter;
    public List<GameObject> AllEnemiesSpawned = new List<GameObject>();
    public List<GameObject>ListEnemy = new List<GameObject>();
    public List<Transform> ListEnemySpawnPoints = new List<Transform>();
    public Coroutine CTimeStarted;
    public Coroutine CSpawnEnemies;
    public float MaxSpawnTest;
    public Image DarkBG;

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
            TimeCountdownUI.text = $"{timeSpan.Minutes.ToString("D2")}:{timeSpan.Seconds.ToString("D2")}";
        } 
    }
    public ProtectedFloat MaxTime;
    TimeSpan timeSpan;
    public Vector2 CurrentIdleSize;
    public Vector2 NewIdleSize;
    public Image ImageIdle;
    Animator animatorIdle;
    public ProtectedBool GameOver;
    public Text GameOverUI;
    public ProtectedBool GameStarted;
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
        if (!GameStarted)
        {
            return;
        }
        CurrentClicks++;
        IdlePopupClicks.Instance.ShowPopup(true);
        if (!animatorIdle.GetBool("Jump"))
        {
            animatorIdle.SetTrigger("Jump");
        }
        PointCenter.DOScale(NewIdleSize,0.2f).SetEase(Ease.OutSine).OnComplete(() => {
            PointCenter.DOScale(CurrentIdleSize,0.15f).SetEase(Ease.OutSine);
        });
    }
    public void StopCoroutines()
    {
        if (CTimeStarted != null)
        {
            StopCoroutine(CTimeStarted);
        }
        if (CSpawnEnemies != null)
        {
            StopCoroutine(CSpawnEnemies);
        }
    }
    public void IdleGotDamaged(int reduce = 1)
    {
        if (GameOver)
        {
            return;
        }
        CurrentClicks -= reduce;
        if (CurrentClicks <= 0)
        {
            CurrentClicks = 0;
        }
        IdlePopupClicks.Instance.ShowPopup(false);
        
        ImageIdle.DOKill();
        ImageIdle.DOColor(Color.red, 0.5f)
        .SetLoops(2, LoopType.Yoyo)
        .OnComplete(() => ImageIdle.DOColor(Color.white, 0.25f));
    }
    public void ClearEnemies()
    {
        for (int i = AllEnemiesSpawned.Count - 1; i >= 0; i--)
        {
            if (AllEnemiesSpawned[i] != null)
            {
                Destroy(AllEnemiesSpawned[i].gameObject);
            }
        }
        AllEnemiesSpawned.Clear();
    }
    public void SetAdventureModeState(int minXP, int maxXP, int requiredClicks, int time)
    {
        GameStarted = false;
        ClearEnemies();
        CurrentClicks = 0;
        UIManager.Instance.MainMenuBTNGO.SetActive(true);
        GameOver = false;
        GameStartBTNGO.SetActive(true);
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
        GameStarted = true;
        TimeCountdownUI.DOKill();
        TimeCountdownUI.DOFade(0,1.5f).SetLoops(-1,LoopType.Yoyo);
        TimeCountdownUI.DOColor(Color.red,1.2f).SetLoops(-1,LoopType.Yoyo);
        StopCoroutines();
        CTimeStarted = StartCoroutine(ITimeStarted());
        CSpawnEnemies = StartCoroutine(ISpawnEnemies(1.5f,0.2f));
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
        GameStarted = false;
        UIManager.Instance.MainMenuBTNGO.SetActive(false);
        TimeCountdownUI.DOKill();
        TimeCountdownUI.DOFade(1,0.5f);
        TimeCountdownUI.DOColor(Color.red,0.3f);
        TimeCountdown = 0;
        GameOver = true;
        if (CSpawnEnemies != null)
        {
            StopCoroutine(CSpawnEnemies);
        }
        DarkBG.DOFade(1,1f);
        GameOverUI.DOFade(1,1.5f);
        bool win = false;
        if (CurrentClicks >= ClicksRequired)
        {
            GameOverUI.color = Color.green;
            GameOverUI.text = "GAME COMPLETED";
            win = true;
        }
        else
        {
            GameOverUI.color = Color.red;
            GameOverUI.text = "GAME INCOMPLETE";
        }
        ClearEnemies();
        yield return new WaitForSeconds(2.5f);
        DarkBG.DOFade(0,1f);
        GameOverUI.DOFade(0,1.5f);
        if(win)
        {
            LevelManager.Instance.XPGain();
        }
        yield return new WaitForSeconds(2f);
        TimeCountdownUI.color = Color.white;

        GameManager.Instance.StartGameAdventure();
    }
    public IEnumerator ISpawnEnemies(float maxSpawnTime, float minSpawnTime)
    {
        float currentSpawnTime = 0;
        MaxSpawnTest = maxSpawnTime;
        while (TimeCountdown >= 0)
        {
            currentSpawnTime += 1f * Time.deltaTime;
            if (currentSpawnTime >= MaxSpawnTest)
            {
                GameObject enemy = Instantiate(ListEnemy[UnityEngine.Random.RandomRange(0,ListEnemy.Count)],ListEnemySpawnPoints[UnityEngine.Random.RandomRange(0,ListEnemySpawnPoints.Count)].position,Quaternion.identity,EnemyContainer);
                RusherAdventure rusherAdventure = enemy.GetComponent<RusherAdventure>();
                int rand = UnityEngine.Random.Range(0,2);
                rusherAdventure.Image.color = ColorsRandomEnemy[UnityEngine.Random.RandomRange(0,ColorsRandomEnemy.Count)];

                if (rand == 0)
                {
                    rusherAdventure.StarInit(1);
                }
                else if (rand == 1)
                {
                    rusherAdventure.StarInit(2);
                }


                AllEnemiesSpawned.Add(enemy);

                float timeInterp = (float )TimeCountdown / MaxTime;
                float newSpeed = Mathf.Lerp( 300,  198.8f,timeInterp);
                float newSpawn = Mathf.Lerp( minSpawnTime ,  maxSpawnTime,timeInterp);
                
                MaxSpawnTest = newSpawn;
                rusherAdventure.MoveSpeed = newSpeed;
                currentSpawnTime = 0;
            }
            yield return null;
        }
    }
}
