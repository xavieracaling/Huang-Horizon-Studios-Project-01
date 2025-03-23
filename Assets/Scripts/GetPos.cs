using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPos : MonoBehaviour
{
    [ContextMenu("GetPos")]
    public void GetPos_()
    {
        Debug.Log($"transf : {transform.localPosition}");
    }

    
}
