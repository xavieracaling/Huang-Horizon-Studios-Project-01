using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using GUPS.AntiCheat.Protected;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject PrefabGoingLeftCharacter;
    public GameObject PrefabGoingRightCharacter;
    public GameObject GameContainer;
    public Transform MaskGameContainer;
    public GameObject StartGameGO;
    public GameObject MenuCenterBG;
    public GameObject PrefabBomb;
    public GameObject PrefabBNBUIText;
    public List<GameObject>  ListEffectRush = new List<GameObject>();

    public PlayerGameDataProtected _PlayerGameDataProtected;
    public ProtectedInt64 TestAvailClicks = 50;
    public ProtectedFloat TestTotalBNBEarned;
    public List<Action> ListOfOnSavePlayerData = new List<Action>();
    public Coroutine ISavePlayerData;
    public bool AbleToSavePlayerData;
    void Awake()
    {
        AbleToSavePlayerData = true;
        Instance = this;
    }
    public void StartISavePlayerData()
    {
        if(ISavePlayerData != null)
            StopCoroutine(ISavePlayerData);
        ISavePlayerData = StartCoroutine(IESavePlayerData());
    }
    public void StopISavePlayerData()
    {
        if(ISavePlayerData != null)
            StopCoroutine(ISavePlayerData);
    }
    
    IEnumerator IESavePlayerData()
    {
        while (true)
        {
            if(ListOfOnSavePlayerData.Count > 0 && AbleToSavePlayerData)
            {
                ListOfOnSavePlayerData[ListOfOnSavePlayerData.Count - 1]?.Invoke();
                ListOfOnSavePlayerData.Remove(ListOfOnSavePlayerData[ListOfOnSavePlayerData.Count - 1]);
                yield return new WaitForEndOfFrame();
            }
                yield return new WaitForSeconds(1.5f);

        }
    }
    public void StartGame() => PlayFabManager.Instance.ExecuteWithSessionCheck( () => 
    {
        GameContainer.SetActive(true);
        StartGameGO.SetActive(false);
        MenuCenterBG.SetActive(false);
        StartISavePlayerData();
        StartCoroutine(StartSpawnRushers());
        Debug.Log("Game has started!");
    });
    
    public IEnumerator StartSpawnRushers()
    {
        while(true)
        {
            bool goingLeftChar = false;
            int indexRand =  UnityEngine.Random.Range(0,2);
            int indexRandLeft =  UnityEngine.Random.Range(0,2);
            
            float randomXStartPos = 0;
            float targetXEndPos = 0;
            float yPosStarting = 0;
            GameObject rusher = null;
            if(indexRand == 0)
                goingLeftChar = true;
            
            if(goingLeftChar) //left
            {
                rusher = Instantiate(PrefabGoingLeftCharacter,MaskGameContainer);
                targetXEndPos = -1120f;
                randomXStartPos = UnityEngine.Random.Range(524,924);
                if(indexRandLeft == 0)
                    yPosStarting = 273;
                else
                    yPosStarting = -291;
            }
            else //right
            {
                rusher = Instantiate(PrefabGoingRightCharacter,MaskGameContainer);
                targetXEndPos = 1130f;
                randomXStartPos =  UnityEngine.Random.Range(-987,-605);
                yPosStarting = -48;
            }
            int rand =  UnityEngine.Random.Range(0,2);
            if(rand == 1)
            {
                Rusher _rusher = rusher.GetComponent<Rusher>();
                _rusher.Explode = true;
            }
            rusher.transform.localPosition = new UnityEngine.Vector2(randomXStartPos,yPosStarting);
            rusher.transform.DOLocalMoveX(targetXEndPos, UnityEngine.Random.Range(3f,4.6f)).SetEase(Ease.Linear).OnComplete(() => {if(rusher!= null) Destroy(rusher);} );
            yield return new WaitForSeconds( UnityEngine.Random.Range(0.1f,1.2f));
        }
        
    }
}
