using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpMessagerPop : MonoBehaviour
{
    public Text LevelUI;
    public Text TapTicketsUI;
    public Button Confirm;
    public void SetMessagerPop(int level, int tapTickets)
    {
        LevelUI.text = $"LV.{level}";
        TapTicketsUI.text = $"{tapTickets}";
        Confirm.onClick.AddListener(() =>
        {
            if (GameManager.Instance.CurrentMode == Modes.Adventure)
            {
                LevelManager.Instance.LeveledUpPanel = false;
                UIManager.Instance.MainMenuBTNGO.SetActive(true);
                GameManager.Instance.GotoMenu();
            }
            Destroy(gameObject);
        });
    }
}
