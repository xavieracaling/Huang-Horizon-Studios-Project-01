using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text AddressPlayerUI;
    public Text NamePlayerUI;
    public InputField NameUI;
    public GameObject NameContainerUI;
    public GameObject MainSceneConnectedUI;
    public GameObject DisconnectedTransform; 

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
    // Update is called once per frame
    void Update()
    {
        
    }
}
