using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;
    public GameObject YourBoosterGO;
    public GameObject NOBoosterGO;
    public GameObject PrefabYourBooster;
    public Transform ContentYourBooster;
    void Awake()
    {
        Instance = this;
    }
    public void NoBooster()
    {
        YourBoosterGO.SetActive(true);
        NOBoosterGO.SetActive(true);
        clearContentYourBooster();
    }
    public void BoosterShow()
    {
        NOBoosterGO.SetActive(false);
        YourBoosterGO.SetActive(true);
        clearContentYourBooster();
        foreach (var item in GameManager.Instance._PlayerGameDataProtected.OwnedBoosterPacks)
        {
            GameObject booster = Instantiate(PrefabYourBooster,ContentYourBooster);
            YourOwnedBooster yourOwnedBooster = booster.GetComponent<YourOwnedBooster>();
            yourOwnedBooster.InitializeBooster(item);
        }
    }
    void clearContentYourBooster()
    {
        if(ContentYourBooster.childCount > 0)
        {
            for (int i = ContentYourBooster.childCount - 1; i >= 0; i--)
            {
                Destroy(ContentYourBooster.GetChild(i).gameObject);
            }
        }
    }

    // Update is called once per frame
}
