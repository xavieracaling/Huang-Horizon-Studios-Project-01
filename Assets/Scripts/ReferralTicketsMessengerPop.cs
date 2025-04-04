using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferralTicketsMessengerPop : MonoBehaviour
{
    
    public Text ValueUI;
    public Button Confirm;
    public void SetMessagerPop(string msg)
    {
        ValueUI.text = msg;
        Confirm.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        
    }
}
