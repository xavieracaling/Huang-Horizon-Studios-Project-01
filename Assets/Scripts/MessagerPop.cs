using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MessagerPop : MonoBehaviour
{
    public Text MessageUI;
    public Button Confirm;
    public void SetMessagerPop(string msg, Action action, bool restart = false)
    {
        MessageUI.text = msg;
        
        if(restart)
        {
            Confirm.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0,LoadSceneMode.Single);
            });
            return;
        }
        if(action != null)
        {
            Confirm.onClick.AddListener(() =>
            {
                action.Invoke();
                Destroy(gameObject);
            });
        }
        else if(action == null)
        {
            Confirm.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}
