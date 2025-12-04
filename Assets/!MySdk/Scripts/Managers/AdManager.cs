using System;
using UnityEngine;

namespace mySdk
{
    public class AdManager : MonoBehaviour
    {
        public void Init()
        {
            Debug.Log("Ad manager init");
        }

        public void ShowInterstitial()
        {
            Debug.Log("Interstitial show");
        }

        public void ShowReward(Action rewardShowed)
        {     
            Debug.Log("Reward show");
        }
    }
}
