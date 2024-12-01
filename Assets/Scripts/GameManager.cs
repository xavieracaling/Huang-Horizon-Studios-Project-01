using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using GUPS.AntiCheat.Protected;
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
    void Awake()
    {
     
        Instance = this;
    }
    
    public void StartGame() => PlayFabManager.Instance.ExecuteWithSessionCheck( () => 
    {
        GameContainer.SetActive(true);
        StartGameGO.SetActive(false);
        MenuCenterBG.SetActive(false);

        StartCoroutine(StartSpawnRushers());
        Debug.Log("Game has started!");
    });
    
    public IEnumerator StartSpawnRushers()
    {
        while(true)
        {
            bool goingLeftChar = false;
            int indexRand = Random.Range(0,2);
            int indexRandLeft = Random.Range(0,2);
            
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
                randomXStartPos = Random.Range(524,924);
                if(indexRandLeft == 0)
                    yPosStarting = 273;
                else
                    yPosStarting = -291;
            }
            else //right
            {
                rusher = Instantiate(PrefabGoingRightCharacter,MaskGameContainer);
                targetXEndPos = 1130f;
                randomXStartPos = Random.Range(-987,-605);
                yPosStarting = -48;
            }
            int rand = Random.Range(0,2);
            if(rand == 1)
            {
                Rusher _rusher = rusher.GetComponent<Rusher>();
                _rusher.Explode = true;
            }
            rusher.transform.localPosition = new UnityEngine.Vector2(randomXStartPos,yPosStarting);
            rusher.transform.DOLocalMoveX(targetXEndPos,Random.Range(3f,4.6f)).SetEase(Ease.Linear).OnComplete(() => {if(rusher!= null) Destroy(rusher);} );
            yield return new WaitForSeconds(Random.Range(0.1f,1.2f));
        }
        
    }
}
