using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake()
    {
        Instance = this;
    }
    public void StartGame() => PlayFabManager.Instance.ExecuteWithSessionCheck( () => 
    {
        Debug.Log("Game has started!");
    });
    
}
