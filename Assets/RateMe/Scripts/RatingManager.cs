using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingManager : MonoBehaviour
{

	int death =0;
	bool isEmail;
	int rating;
	GameObject SpawnedPanel;

	[Header ("App Links")]
	string PlayStore = "http://play.google.com/store/apps/details?id={0}";
	string AppStore = "itms://itunes.apple.com/us/app/apple-store/{0}?mt=8";
	[Tooltip("iOS App ID (number), example: 1122334455")]
	public string iOSAppID="";
	[Tooltip ("Android Link is automatic")]
	public string AmazonStore;
	public string Email;
	public bool PreferAmazon;

	[Header ("Number Values")][Space(20)]
	[Range(1, 5)]
	public int MinimumRating;
	[Range(10, 200)]
	public int DeathsBeforeApparition = 10;

	[Header ("Layout")][Space(20)]
	public GameObject RateUsPanel;

	[Header ("Debug")][Space(20)]
	public bool DeleteRatingPlayerPrefs;



    // Start is called before the first frame update
    void Start()
    {
		if(DeleteRatingPlayerPrefs){
			PlayerPrefs.DeleteKey("Rated");
			PlayerPrefs.DeleteKey("RatingDeaths");
		}
		death = PlayerPrefs.GetInt("RatingDeaths", 0);
		int neverRate = PlayerPrefs.GetInt("Rated", 0);;
		if(neverRate == 1){
			Destroy(this.gameObject);
		}
		if(MinimumRating <1){
			Debug.LogError("You will allow every type of rating if you don't set Minimum Rating");
		}
		if(MinimumRating >0 && Email.Length < 1){
			Debug.LogError("You did not set any email so the user will not see the email option for low ratings");
			isEmail = false;
		}else{
			isEmail = true;
		}

		#if UNITY_IOS
		if (!string.IsNullOrEmpty (iOSAppID) || IsDigitsOnly(iOSAppID)) {
				AppStore = AppStore.Replace("{0}",iOSAppID);
			}
			else {
				Debug.LogError ("Please set iOSAppID variable correctly");
			}

		#elif UNITY_ANDROID
			PlayStore = PlayStore.Replace("{0}",Application.identifier);
		#endif

    }

    // Update is called once per frame
    void Update()
    {
		if(death >= DeathsBeforeApparition){
			RateUsPanel.SetActive(true);
		}
    }

	public void die(){
		death = PlayerPrefs.GetInt("RatingDeaths", 0) + 1;
		PlayerPrefs.SetInt("RatingDeaths", death);

	}

	public void Rate(int currentrating){
		rating = currentrating;
	}

	public void RateUs(){
		PlayerPrefs.SetInt("Rated", 1);
		if(Application.platform == RuntimePlatform.Android){
			if(PreferAmazon){
				Application.OpenURL(AmazonStore);
			}else{
				Application.OpenURL(PlayStore);
			}
		}else if (Application.platform == RuntimePlatform.IPhonePlayer){
			Application.OpenURL(AppStore);
		}else{
			Application.OpenURL("www.google.com");
		}
		RateUsPanel.SetActive(false);
		Destroy(this.gameObject);
	}

	public void EmailUs(){
		PlayerPrefs.SetInt("Rated", 1);
		Application.OpenURL("mailto:" + Email + "?subject=Unity Game Bad Review");
		RateUsPanel.SetActive(false);
		Destroy(this.gameObject);
	}

	public void Later(){
		RateUsPanel.SetActive(false);
		PlayerPrefs.SetInt("RatingDeaths", 0);
		death = 0;
	}

	public void Never(){
		RateUsPanel.SetActive(false);
		PlayerPrefs.SetInt("Rated", 1);
		Destroy (this.gameObject);
	}
	bool IsDigitsOnly(string str)
	{
		foreach (char c in str)
		{
			if (c < '0' || c > '9')
				return false;
		}

		return true;
	}
}
