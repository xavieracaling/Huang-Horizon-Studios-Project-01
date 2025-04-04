using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewbieNoReferral : MonoBehaviour
{
    public TMP_InputField ReferralCodeUI;
    public void ConfirmReferralCode()
    {
        if (ReferralCodeUI.text.Length > 0)
        {
            PlayFabManager.Instance.ValidateReferralCode(ReferralCodeUI.text, () => {Destroy();} );
        }
    }
    public void Destroy() => Destroy(gameObject);
}
