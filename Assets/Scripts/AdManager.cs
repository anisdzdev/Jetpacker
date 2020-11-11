using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
	public bool testing = true;

    [Space (5)]
	public string AndroidappId = "ca-app-pub-1471809630602015~9404537603";
	public string IOSappId = "ca-app-pub-1471809630602015~3819864979";
	public string TestAndroidappId = "ca-app-pub-3940256099942544~3347511713";
	public string TestIOSappId = "ca-app-pub-3940256099942544~1458002511";

    [Space(5)]
    public string AndroidBannerTopAdUnitId = "ca-app-pub-1471809630602015/6586802577";
	public string IOSBannerTopAdUnitId = "ca-app-pub-1471809630602015/4861483909";
	public string TestAndroidBannerTopAdUnitId = "ca-app-pub-3940256099942544/6300978111";
	public string TestIOSBannerTopAdUnitId = "ca-app-pub-3940256099942544/2934735716";

    [Space(5)]
    public string AndroidInterstitialAdUnitId = "ca-app-pub-1471809630602015/3959465776";
	public string IOSInterstitialAdUnitId = "ca-app-pub-1471809630602015/6158662828";
	public string TestAndroidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
	public string TestIOSInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";

    [Space(5)]
    public string AndroidRewardedAdUnitId = "ca-app-pub-1471809630602015/5226952314";
    public string IOSRewardedAdUnitId = "ca-app-pub-1471809630602015/6267397784";
    public string TestAndroidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    public string TestIOSRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";

    public BannerView bannerView;
	public InterstitialAd interstitial;
    public RewardedAd rewardedAd;

    // Start is called before the first frame update
    void Start()
    {
		#if UNITY_ANDROID
		string appId = (!testing) ? AndroidappId : TestAndroidappId;
        this.rewardedAd = (!testing) ? new RewardedAd(AndroidRewardedAdUnitId) : new RewardedAd(TestAndroidRewardedAdUnitId);
#elif UNITY_IPHONE
		string appId = (!testing) ? IOSappId : TestIOSappId;
        this.rewardedAd = (!testing) ? new RewardedAd(IOSRewardedAdUnitId) : new RewardedAd(TestIOSRewardedAdUnitId);
#else
		string appId = "unexpected_platform";
        this.rewardedAd = new RewardedAd(TestAndroidRewardedAdUnitId);
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
		this.RequestTopBanner();
		this.RequestInterstitial();
        this.RequestRewarded();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RequestRewarded() {

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

	private void RequestTopBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = (!testing) ? AndroidBannerTopAdUnitId : TestAndroidBannerTopAdUnitId;
		#elif UNITY_IPHONE
		string adUnitId = (!testing) ? IOSBannerTopAdUnitId : TestIOSBannerTopAdUnitId;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);
		AdRequest request;
		if(testing){
			request = new AdRequest.Builder()
				.AddTestDevice(AdRequest.TestDeviceSimulator)
				.AddTestDevice(SystemInfo.deviceUniqueIdentifier)
			.Build();
		}else{
			request = new AdRequest.Builder().Build();
		}

		bannerView.LoadAd(request); 
	}

	private void RequestInterstitial()
	{
		#if UNITY_ANDROID
		string adUnitId = (!testing) ? AndroidInterstitialAdUnitId : TestAndroidInterstitialAdUnitId;
		#elif UNITY_IPHONE
		string adUnitId = (!testing) ? IOSInterstitialAdUnitId : TestIOSInterstitialAdUnitId;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		interstitial = new InterstitialAd(adUnitId);
		AdRequest request;
		if(testing){
			request = new AdRequest.Builder()
				.AddTestDevice(AdRequest.TestDeviceSimulator)
				.AddTestDevice(SystemInfo.deviceUniqueIdentifier)
			.Build();
		}else{
			request = new AdRequest.Builder().Build();
		}
		interstitial.LoadAd(request);
	}

	public void ShowInterstitial()
	{
		Debug.Log("INTERSTITIAL NOW");
		if (interstitial.IsLoaded()) {
			interstitial.Show();
		}
		RequestInterstitial();
	}

    public void WatchRewardedAd() {
        if (this.rewardedAd.IsLoaded()) {
            this.rewardedAd.Show();
        }
    }

    public void RemoveBanner()
	{
		Debug.Log("HIDE BANNER");
		bannerView.Hide();
		bannerView.Destroy();
	}



    public void HandleRewardedAdLoaded(object sender, EventArgs args) {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args) {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
        RequestRewarded();
    }

    public void HandleUserEarnedReward(object sender, Reward args) {
        StartCoroutine(FindObjectOfType<LevelManager>().Reward3());
    }
}
