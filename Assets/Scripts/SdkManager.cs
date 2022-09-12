using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using GameAnalyticsSDK;

public class SdkManager : MonoBehaviour
{
    void Awake() 
    {
        DontDestroyOnLoad(this.gameObject);
        FB.Init("1225142904727954");
        GameAnalytics.Initialize();
    }
}