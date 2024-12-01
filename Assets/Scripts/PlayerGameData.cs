using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class PlayerGameData 
{
    //referralMultiplierPoints
    public float TotalReferralMultiplierPoints;
    public List<PlayerBoosterPackUnProtected> OwnedBoosterPacks = new List<PlayerBoosterPackUnProtected>();
}
