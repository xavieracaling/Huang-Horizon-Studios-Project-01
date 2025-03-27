using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebglReferral : MonoBehaviour
{
    public string ReferralID { get; private set; }
    public Text TestRefferal;
    void Start()
    {
        Debug.Log("start url!!: ");

#if UNITY_WEBGL && !UNITY_EDITOR
        string url = Application.absoluteURL;
        Debug.Log("URL: " + url);

        ReferralID = GetQueryParam(url, "referral_id");

        if (!string.IsNullOrEmpty(ReferralID))
        {
            Debug.Log("Referral ID: " + ReferralID);
            TestRefferal.text = $"Referral ID: {ReferralID}";

            // 🔥 You can send it to your backend or use it in your game here
            OnReferralReceived(ReferralID);
        }
        else
        {
            TestRefferal.text = $"No referral ID found in URL.";
            Debug.Log("No referral ID found in URL.");
        }
#endif
    }

    private string GetQueryParam(string url, string key)
    {
        int index = url.IndexOf("?");
        if (index >= 0)
        {
            string queryString = url.Substring(index + 1);
            string[] parameters = queryString.Split('&');
            foreach (var param in parameters)
            {
                string[] kvp = param.Split('=');
                if (kvp.Length == 2 && kvp[0] == key)
                {
                    return kvp[1];
                }
            }
        }
        return null;
    }

    private void OnReferralReceived(string referral)
    {
        // 🎯 Do whatever you want here
        Debug.Log("Referral received: " + referral);
        // Example: Send to server, save to PlayerPrefs, etc.
    }
}
