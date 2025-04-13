using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ClipboardUtility : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CopyToClipboard(string str);
#endif

    public Text ToCopyUI1;
    public Text ToCopyUI2;
    public void Copy1()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CopyToClipboard(ToCopyUI1.text);
        
#else
        GUIUtility.systemCopyBuffer = ToCopyUI1.text;
#endif
    UIManager.Instance.InstantiateMessagerPopPrefab_Message("Copied successfully.");
    }
    public void Copy2()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CopyToClipboard(ToCopyUI2.text);
#else
        GUIUtility.systemCopyBuffer = ToCopyUI2.text;
#endif
    UIManager.Instance.InstantiateMessagerPopPrefab_Message("Copied successfully.");

    }
}