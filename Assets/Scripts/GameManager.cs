using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject PrefabGoingLeftCharacter;
    public GameObject PrefabGoingRightCharacter;
    public GameObject GameContainer;
    public Transform MaskGameContainer;
    public GameObject StartGameGO;
    public GameObject MenuCenterBG;
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
            rusher.GetComponent<Button>().onClick.AddListener(() => Destroy(rusher));

            rusher.transform.localPosition = new UnityEngine.Vector2(randomXStartPos,yPosStarting);
            rusher.transform.DOLocalMoveX(targetXEndPos,2f).SetEase(Ease.Linear).OnComplete(() => {if(rusher!= null) Destroy(rusher);} );
            yield return new WaitForSeconds(Random.Range(0.1f,1.2f));
        }
        
    }
}
