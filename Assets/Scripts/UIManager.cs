using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text AddressPlayerUI;
    public Text NamePlayerUI;
    public Text BNBUI;
    public InputField NameUI;
    public GameObject NameContainerUI;
    public GameObject MainSceneConnectedUI;
    public GameObject DisconnectedTransform; 
    public GameObject LoadingPrefab; 
    public MessagerPop MessagerPopPrefab; 
    public Transform Container;
    public static UIManager Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }
    public void ConnectedSceneUI()
    {
        _EVM.Instance.GetBalance();
        NamePlayerUI.text = PlayFabManager.Instance.PlayerName;
        AddressPlayerUI.text = PlayFabManager.Instance.CustomUserIDAddress;
        MainSceneConnectedUI.SetActive(true);
        DisconnectedTransform.SetActive(false);
    } 
    public void DisconnectedSceneUI()
    {
        MainSceneConnectedUI.SetActive(false);
        DisconnectedTransform.SetActive(true);
    } 
    public void InstantiateMessagerPopPrefabFull(string msg, Action action,bool restart) 
    {
        MessagerPop messagerPop = Instantiate(MessagerPopPrefab,Container);
        messagerPop.SetMessagerPop(msg,action,restart);
        messagerPop.transform.SetAsLastSibling();
    }
    public void InstantiateMessagerPopPrefab_Restart(string msg) => InstantiateMessagerPopPrefabFull(msg,null,true);
    public void InstantiateMessagerPopPrefab_Message(string msg) => InstantiateMessagerPopPrefabFull(msg,null,false);
    public GameObject LoadingShow()
    {
        GameObject loading = Instantiate(LoadingPrefab,Container);
        return loading;
    } 
}
