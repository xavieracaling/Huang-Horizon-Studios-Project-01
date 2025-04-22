using System.Collections;
using System.Collections.Generic;
using GUPS.AntiCheat.Protected;
using UnityEngine;
using UnityEngine.UI;
using System;
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
            TimeCountdownUI.text = timeCountdown.ToString();
        } 
    }
    
    void Awake()
    {
        Instance = this;
    }
    public void SetAdventureModeState(int minXP, int maxXP, int requiredClicks, int time)
    {
        CompletionXPGainUI.text = $"{minXP} XP - {maxXP} XP";
        ClicksRequiredUI.text = $"{requiredClicks}";
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        TimeCountdown = time;
    }
    [ContextMenu("GameCompleted")]
    public void GameCompleted()
    {
        LevelManager.Instance.XPGain();
    }
    [ContextMenu("GameStart")]
    public void GameStart()
    {
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
            yield return new WaitForSeconds(1);
        }
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
                SpriteRenderer  spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                spriteRenderer.color = ColorsRandomEnemy[UnityEngine.Random.RandomRange(0,ColorsRandomEnemy.Count)];
                currentSpawnTime = 0;
            }
            yield return null;
        }
    }
}
