using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryShopDG : MonoBehaviour
{
    public GameObject TargetCategoryContent;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=> UIManager.Instance.CategoryChangeShop(TargetCategoryContent,transform));
    }
}
