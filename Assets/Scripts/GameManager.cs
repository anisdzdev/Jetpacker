using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms;
using System.Collections;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#if UNITY_IPHONE
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using UnityEngine.Analytics;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	[HideInInspector]
	public bool canStart, tapped, justStarted, started, dead, first_time_spawn, changedBG, waitBeforeSpawn, StopCam, authenticated;
	public bool enhancedPlatformMove;
	public float camMinPos, characterMaxPos, previousSpawn, topConstraint, lastTop, middle, diff, secondsClicked;
	public List<Platforms> all_platforms = new List<Platforms>();
	DateTime currentDate;
	DateTime oldDate;
	string destination;
	GameObject[] skins;
	private bool flash = false;
	public int BGNumber;
	int BGcount;
	int spawnNumber = 3;
    public int coins;


    string PlayStore = "http://play.google.com/store/apps/details?id={0}";
	string AppStore = "itms://itunes.apple.com/us/app/apple-store/{0}?mt=8";


	const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";


	ShopItem focusedItem;

	public string leaderBoardId;
	public string leaderboardAndroidId;
	public string policyURL;
	public Sprite soundOn;
	public Sprite soundOff;
	public bool isSoundOn;


	[Header("Numbers")]
	public string user_id;
	public string iOSAppID="";
	public int score;
	public int deathCount;
	public int consecutiveDaysPlaying;
	public float minPos;
	public float maxPos;
	public Vector3 offset;
	public float diePower;
	public float distanceZ = 10.0f;
	public bool ChangeBG;
	public int ChangeBgEvery;

	[Space(10)]
	[Header("GameObjects")]
	public GameObject StartScreen;
	public GameObject GameScreen;
	public GameObject EndScreen;
	public GameObject leaderboardBtn;
	public GameObject MusicButton;
	public GameObject Overlay;
	public GameObject MenuOverlay;
	public GameObject camera;
	public GameObject SkyCam;
	public GameObject ScreenBackground;
	public GameObject ScreenBackgroundParent;
	public TextMeshProUGUI ScoreText;
	public TextMeshProUGUI EndScoreText;
	public TextMeshProUGUI BestScoreText;
	public TextMeshProUGUI ItemDescription;
	public PlayerMovement player;
	public GameObject PlatformObj;
	public GameObject LastPlatform;
	public GameObject Spikes;
	public GameObject LastSpawnedSky;
	public CanvasGroup myCG;
	public Background[] backgrounds;
    public GameObject shopIndicator;



    [Space(10)]
	[Header("Animations")]

	public EasyTween tapToFly;
	public EasyTween tapToFlyImage;
	public EasyTween tapToFly2;
	public EasyTween GameName;
	public EasyTween ScoreAnim;
	public EasyTween store_btn;
	public EasyTween menu_btn;
	public EasyTween new_best;
	public EasyTween you_died;
	public EasyTween restart_btn;
	public EasyTween store_panel;
	public EasyTween store_overlay;
	public EasyTween menu_panel;
	public EasyTween menu_overlay, mode_btn;

    [Space(10)]
    public GameObject canvas;
    public GameObject coinText;
    public GameObject coinprefab;
    public GameObject coin_in_game_prefab;
    public Ease animationEase = Ease.InQuad;


    // Start is called before the first frame update
    void Start()
    {
		Application.targetFrameRate = 60;
		dead = false;
		player = FindObjectOfType<PlayerMovement>();
		player.started = false;
		tapToFly.OpenCloseObjectAnimation();
		tapToFlyImage.OpenCloseObjectAnimation();
		store_btn.OpenCloseObjectAnimation();
		menu_btn.OpenCloseObjectAnimation();
        mode_btn.OpenCloseObjectAnimation();
        StartCoroutine(SetCanStart());
		camMinPos = camera.transform.position.y;
		characterMaxPos = player.rb.gameObject.transform.position.y + 1;
		offset = Spikes.transform.position-Camera.main.gameObject.transform.position;
		consecutiveDaysPlaying = PlayerPrefs.GetInt("consecutiveDaysPlaying");
		CheckDate();
		string ItemName = PlayerPrefs.GetString ("active_item", "1");
		SetSkin(ItemName);
		BGcount = backgrounds.Length;
		deathCount = PlayerPrefs.GetInt("deathCount", 0);
		isSoundOn = (PlayerPrefs.GetInt("soundOn", 1) == 1) ? true : false;

        coins = PlayerPrefs.GetInt("coins", 0);
        CheckSound();
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
		
	void FixedUpdate(){
		//Spikes.transform.position = Camera.main.gameObject.transform.position + offset;

	}

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("new_item") == 1) {
            shopIndicator.SetActive(true);
        } else {
            shopIndicator.SetActive(false);
        }
        PlayerPrefs.SetInt("coins", coins);
        coinText.GetComponent<TextMeshProUGUI>().text = coins.ToString();

        if (!PlayerPrefs.HasKey("userid")){
			Debug.Log("here");
			for(int i=0; i<12; i++)
			{
				Debug.Log("nope");
				char Char = glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
				Debug.Log(Char);
				user_id += Char;
			}
			PlayerPrefs.SetString("userid", user_id);
		}else{
			user_id = PlayerPrefs.GetString("userid");
		}
		topConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, Screen.height, distanceZ) ).y;
		middle = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, Screen.height/2, distanceZ) ).y;
		diff = topConstraint - middle;
		if(ChangeBG){
			if(score != 0 && player.rb.gameObject.transform.position.y >= topConstraint && score % ChangeBgEvery == 0){
				if(!changedBG){
					lastTop = topConstraint;
					BGNumber++;
					ChangeBackground();
					changedBG = true;
				}
			}else{
				changedBG = false;
			}
		}

		CameraMovement();
		//if((!player.started && Input.touchCount > 0 && !justStarted) || (!player.started && Input.GetMouseButton(0) && !justStarted)){
		if((!player.started && tapped && !justStarted) || (!player.started && tapped && !justStarted)){
			if(canStart){
				//StartScreen.SetActive(false);
				store_btn.OpenCloseObjectAnimation();
				menu_btn.OpenCloseObjectAnimation();
				GameName.OpenCloseObjectAnimation();
				ScoreAnim.OpenCloseObjectAnimation();
				tapToFly2.OpenCloseObjectAnimation();
                mode_btn.OpenCloseObjectAnimation();
                tapToFlyImage.OpenCloseObjectAnimation();
				GetComponent<AdManager>().RemoveBanner();
				//GameScreen.SetActive(true);
				justStarted = true;
			}
		}else if(justStarted){
			player.started = true;
			justStarted = false;
			started = true;
		}

		if(started){
			ScoreText.text = score.ToString();
		}
		
		if (flash)
         {
             myCG.alpha = myCG.alpha - Time.deltaTime;
             if (myCG.alpha <= 0)
             {
                 myCG.alpha = 0;
                 flash = false;
             }
         }

        
    }

	public void Die(){
		if(player.started){
			dead = true;

			deathCount++;
			if(score >= ChangeBgEvery){
				player.rb.gameObject.GetComponent<Collider2D>().isTrigger = true;
			}
			PlayerPrefs.SetInt("deathCount", deathCount);

			//Platforms[] all_platforms = FindObjectsOfType<Platforms>();
			foreach(Platforms plat in all_platforms){
				if(plat !=null){
					plat.gameObject.SetActive(true);
					//Destroy(plat);
				}
			}
			BoxCollider2D[] Colliders = FindObjectsOfType<BoxCollider2D>();
			foreach (BoxCollider2D col in Colliders){
				if(col.gameObject.name == "Platform" || col.gameObject.name == "Right" || col.gameObject.name == "Left"){
					col.isTrigger = true;
				}
			}
			player.rb.constraints = RigidbodyConstraints2D.None;
			//player.rb.gameObject.transform.rotation = Quaternion.RotateTowards(player.rb.gameObject.transform.rotation,Quaternion.Euler(new Vector3(0,0,-99)), 500 * Time.deltaTime);
			player.rb.AddTorque(diePower, ForceMode2D.Impulse);
			EndScreen.SetActive(true);
			GameScreen.SetActive(false);
			EndScoreText.text = score.ToString();

			if(PlayerPrefs.GetInt("HScore", 0) < score){
				PlayerPrefs.SetInt("HScore", score);
				BestScoreText.text = "new best";
				AnalyticsEvent.Custom("new_best", new Dictionary<string, object>
				{
						{ "score_id", "score" },
						{ "value", score }
				});
				HighScoreBeaten();
			}else{
				BestScoreText.text = "best score: " + PlayerPrefs.GetInt("HScore", 0);
			}
			you_died.OpenCloseObjectAnimation();
			restart_btn.OpenCloseObjectAnimation();
			new_best.OpenCloseObjectAnimation();
			flash = true;
	        myCG.alpha = 1;
			if(deathCount !=0 && deathCount % 3 ==0){
				GetComponent<AdManager>().ShowInterstitial();
				Debug.Log("Interstial Now");
			}
			StartCoroutine(ShareScreenshot());
		}
	}

	public IEnumerator SetCanStart(){
		yield return new WaitForSeconds(1);
		canStart = true;
	}

	public void SpawnPlatform(){
		if(ChangeBG && waitBeforeSpawn){
			waitBeforeSpawn = false;
			return;
		}
		LastPlatform = Instantiate(PlatformObj, new Vector3((float)UnityEngine.Random.Range(minPos, maxPos), previousSpawn+3.5f, transform.position.z), transform.rotation);
		LastPlatform.name = "Platform";
        if (UnityEngine.Random.Range(0, 5) == 0) {
            GameObject coin = Instantiate(coin_in_game_prefab, new Vector3((float)UnityEngine.Random.Range(minPos - 2.53f, maxPos - 2.53f), previousSpawn + (2.75f), transform.position.z), transform.rotation);
        }
        int BGNumberCache = BGNumber;
		if(BGNumber >= BGcount){
			BGNumberCache = BGcount -1;
		}
		Sprite beam = backgrounds[BGNumberCache].beams;
		if(beam !=null){
			SpriteRenderer[] rends = LastPlatform.gameObject.GetComponentsInChildren<SpriteRenderer>();
			foreach(SpriteRenderer rend in rends){
				rend.sprite = beam;
			}
				
		}
		spawnNumber++;
		previousSpawn = LastPlatform.transform.position.y;
		Platforms[] all_active_platforms = FindObjectsOfType<Platforms>();
		foreach(Platforms plat in all_active_platforms){
			if(plat.gameObject.transform.position.y == previousSpawn-(4*3.5f)){
				//Destroy(plat.gameObject);
				all_platforms.Add(plat);
				plat.gameObject.SetActive(false);
			}
		}
	}

	public void SpawnSky(){
		GameObject NewSpawnedSky = Instantiate(LastSpawnedSky, new Vector3(LastSpawnedSky.transform.position.x, LastSpawnedSky.transform.position.y+9.828114f, LastSpawnedSky.transform.position.z), LastSpawnedSky.transform.rotation);
		NewSpawnedSky.name = "sky";
		LastSpawnedSky = NewSpawnedSky;
	}

	public void CameraMovement(){
		Camera cam = camera.GetComponent<Camera>();
		//characterMaxPos = player.rb.gameObject.transform.position.y + 1;
		if(!dead || score >= ChangeBgEvery){
			if(ChangeBG && spawnNumber % ChangeBgEvery == 0){
				StopCam= true;
				waitBeforeSpawn = true;
			}
			if((player.rb.gameObject.transform.position.y - characterMaxPos) > camMinPos){
				if (player.rb.gameObject.transform.position.y >= characterMaxPos){
					
					if(ChangeBG && StopCam && LastPlatform.transform.position.y + 2f  <= topConstraint){
						
						float y = LastPlatform.transform.position.y + 2f - diff;
						camera.transform.position = new Vector3(camera.transform.position.x, y, camera.transform.position.z);
						Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
					}else{
						camera.transform.position = new Vector3(camera.transform.position.x, player.rb.gameObject.transform.position.y - characterMaxPos, camera.transform.position.z);
						Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
					}
				}
			}
			camMinPos = camera.transform.position.y;
		}else{
			if(camera.transform.position.y > 0){
			camera.transform.position = new Vector3(camera.transform.position.x, player.rb.gameObject.transform.position.y - characterMaxPos, camera.transform.position.z);
				Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
			}else{
				camera.transform.position = new Vector3(camera.transform.position.x,0, camera.transform.position.z);
				Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
			}
		}
		float theChange = (BGNumber == 0) ? ((BGNumber+1)*(ChangeBgEvery-1)*3.5f -(1f -2f +diff))  : (BGNumber+1)*ChangeBgEvery*3.5f -(4f -3.5f*(BGNumber-1));
		float theInitialChange = (BGNumber == 0) ? 0f : lastTop-characterMaxPos;
		float mapped_val = map(camera.transform.position.y, theInitialChange, theChange, 0f, 9.5f);
		if(!dead){
			SkyCam.transform.position = new Vector3(SkyCam.transform.position.x, mapped_val, SkyCam.transform.position.z);
		}else{
			SkyCam.transform.position = new Vector3(SkyCam.transform.position.x, 0f, SkyCam.transform.position.z);
		}
		//float MoveBy = (camera.gameObject.transform.position.y- SkyCam.transform.position.y) * 0.01f;
		//SkyCam.transform.Translate(0, MoveBy, 0, Space.Self);
		//SkyCam.transform.position = Vector3.Lerp(SkyCam.transform.position, new Vector3(SkyCam.transform.position.x, camera.gameObject.transform.position.y *0.1f,SkyCam.transform.position.z), Time.deltaTime);
		//if(SkyCam.transform.position.y >= LastSpawnedSky.transform.position.y){
		//	SpawnSky();
		//}
	}

	public void Restart(){
		GetComponent<AdManager>().bannerView.Hide();
		GetComponent<AdManager>().bannerView.Destroy();
		GetComponent<AudioManager>().ButtonSound();
		AnalyticsEvent.Custom("restart", new Dictionary<string, object>
		{
				{ "user_id", user_id },
				{ "value", "restarted" }
		});
		SceneManager.LoadScene("MainScene");
	}

	public void CloseStore(){
		store_overlay.OpenCloseObjectAnimation();
		Overlay.SetActive(false);
		store_panel.OpenCloseObjectAnimation();
	}

	public void OpenStore(){
		Overlay.SetActive(true);
        PlayerPrefs.SetInt("new_item", 0);
		store_overlay.OpenCloseObjectAnimation();
		store_panel.OpenCloseObjectAnimation();
	}

	public void OpenMenu(){
		MenuOverlay.SetActive(true);
		menu_overlay.OpenCloseObjectAnimation();
		menu_panel.OpenCloseObjectAnimation();
	}

	public void CloseMenu(){
		menu_overlay.OpenCloseObjectAnimation();
		MenuOverlay.SetActive(false);
		menu_panel.OpenCloseObjectAnimation();
	}

	public void Tap(){
		tapped = true;
	}

	public void ClickItem(ShopItem item){
		if(!item.unlocked){
			GetComponent<AudioManager>().LockedSound();
            if (item.buyable) {
                ItemDescription.text = "click again to buy for " + item.price + " coins";
            } else {
                ItemDescription.text = item.description;
            }
		}else{
            GetComponent<AudioManager>().ButtonSound();
			if(item.using_item){
				ItemDescription.text = "active skin";
			}else{
				ItemDescription.text = "click again to activate this skin";
			}
		}

		if(focusedItem == item && item.unlocked){
			PlayerPrefs.SetString ("active_item", item.ItemName);
			ItemDescription.text = "active skin";
			SetSkin(item.ItemName);
			AnalyticsEvent.Custom("skin", new Dictionary<string, object>
				{
					{ "user_id", user_id },
					{ "value", item.ItemName }
				});
		}
        if (GetComponent<LevelManager>() != null) {
            if (focusedItem == item && item.buyable && GetComponent<LevelManager>().coins >= item.price && PlayerPrefs.GetInt(item.ItemName) != 1) {
                GetComponent<LevelManager>().coinText.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.4f)
                    .OnComplete(() => {
                        GetComponent<LevelManager>().coinText.GetComponent<RectTransform>().localScale = Vector3.one;
                    }).Play();
                GetComponent<LevelManager>().coins -= item.price;
                PlayerPrefs.SetInt(item.ItemName, 1);
                PlayerPrefs.SetString("active_item", item.ItemName);
                ItemDescription.text = "active skin";
                SetSkin(item.ItemName);
                AnalyticsEvent.Custom("skin", new Dictionary<string, object>
                    {
                    { "user_id", user_id },
                    { "value", item.ItemName }
                });
            }
        } else {
            if (focusedItem == item && coins >= item.price && PlayerPrefs.GetInt(item.ItemName) != 1) {
                coinText.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.4f)
                    .OnComplete(() => {
                        coinText.GetComponent<RectTransform>().localScale = Vector3.one;
                    }).Play();
                coins -= item.price;
                PlayerPrefs.SetInt(item.ItemName, 1);
                PlayerPrefs.SetString("active_item", item.ItemName);
                ItemDescription.text = "active skin";
                SetSkin(item.ItemName);
                AnalyticsEvent.Custom("skin", new Dictionary<string, object>
                    {
                    { "user_id", user_id },
                    { "value", item.ItemName }
                });
            }
        }
        focusedItem = item;
	}

	public void FirstClickItem(ShopItem item){
		if(!item.unlocked){
			ItemDescription.text = item.description;
		}else{
			if(item.using_item){
				ItemDescription.text = "active skin";
			}else{
				ItemDescription.text = "click again to activate this skin";
			}
		}

		if(focusedItem == item && item.unlocked){
			PlayerPrefs.SetString ("active_item", item.ItemName);
			ItemDescription.text = "active skin";
			SetSkin(item.ItemName);
		}
		focusedItem = item;
	}

	// IMPROVE THIS PART
	public void CheckDate(){
        currentDate = System.DateTime.Now;
 
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString", "0"));
        DateTime oldDate = DateTime.FromBinary(temp);
		string now = Convert.ToString(currentDate.Ticks);
		Debug.Log("now Long: " + currentDate.Day + " / " + oldDate.Day);
		if(oldDate.Day == currentDate.Day && oldDate.Month == currentDate.Month && oldDate.Year == currentDate.Year){
			Debug.Log("Same Day !");
			return;
		}

		else if ((oldDate.Day+1) == currentDate.Day && oldDate.Month == currentDate.Month && oldDate.Year == currentDate.Year){
			Debug.Log("Tomorrow !");
			consecutiveDaysPlaying++;
			PlayerPrefs.SetInt("consecutiveDaysPlaying", consecutiveDaysPlaying);
			PlayerPrefs.SetString("sysString", now);
		}else{
			consecutiveDaysPlaying = 0;
			PlayerPrefs.SetInt("consecutiveDaysPlaying", consecutiveDaysPlaying);
			PlayerPrefs.SetString("sysString", now);
		}

		if (PlayerPrefs.GetString("sysString", "0") == "0") {
			PlayerPrefs.SetString("sysString", now);
		}
	}

	public void SetSkin(string skin){
		Transform[] children = player.gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform child in children){
			if(child.gameObject !=player.gameObject && child.gameObject.name != "Particle System"){
				child.gameObject.SetActive(false);
			}
		}
		GameObject Player = player.gameObject.transform.Find(skin).gameObject;
		Player.SetActive(true);

		player.rb = Player.GetComponent<Rigidbody2D>();
		player.sc = player.rb.gameObject.GetComponent<SpriteChanger>();
	}

	public void ChangeBackground(){
		StopCam=false;
		waitBeforeSpawn= false;
		Debug.Log("Change Now");
		previousSpawn = previousSpawn +3.5f;
		SpawnPlatform();
		SpawnPlatform();
		SpawnPlatform();
		int BGNumberCache = BGNumber;
		if(BGNumber >= BGcount){
			BGNumberCache = BGcount -1;
		}
		Color m_color = backgrounds[BGNumberCache].color;
		Sprite m_i_sprite = backgrounds[BGNumberCache].i_sprite;
		Sprite m_sprite = backgrounds[BGNumberCache].sprite;
		Sprite beam = backgrounds[BGNumberCache].beams;
		SkyCam.GetComponent<Camera>().backgroundColor = m_color;
		GameObject[] skies = GameObject.FindGameObjectsWithTag("Background");
		if(beam !=null){
			foreach(Platforms plat in all_platforms){
				if(plat !=null){
					SpriteRenderer[] rends = plat.gameObject.GetComponentsInChildren<SpriteRenderer>();
					foreach(SpriteRenderer rend in rends){
						rend.sprite = beam;
					}
				}
			}
			Platforms[] all_active_platforms = FindObjectsOfType<Platforms>();
			foreach(Platforms plat in all_active_platforms){
				//plat.gameObject.GetComponentn<SpriteRenderer>().sprite = beam;
				if(plat !=null){
					SpriteRenderer[] rends = plat.gameObject.GetComponentsInChildren<SpriteRenderer>();
					foreach(SpriteRenderer rend in rends){
						rend.sprite = beam;
					}
				}
			}
		}
		foreach(GameObject sky in skies){
			if(sky.name == "i_sky" && m_i_sprite !=null){
				sky.GetComponent<SpriteRenderer>().sprite = m_i_sprite;
			}else{
				sky.GetComponent<SpriteRenderer>().sprite = m_sprite;
			}
		}
		//SkyCam.transform.position = new Vector3(LastSpawnedSky.transform.position.x, LastSpawnedSky.transform.position.y, LastSpawnedSky.transform.position.z);
	}

	void HighScoreBeaten(){
		#if UNITY_IPHONE
		Social.ReportScore((long)score, leaderBoardId, HighScoreCheck);
		#elif UNITY_ANDROID
        //Social.ReportScore((long)score, leaderboardAndroidId, HighScoreCheck);
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            // Note: make sure to add 'using GooglePlayGames'
            PlayGamesPlatform.Instance.ReportScore((int)score,
                        GPGSIds.leaderboard_highest_scores,
                        (bool success) => {
                            Debug.Log("Leaderboard update success: " + success);
                        });
        }
#endif
    }
    


    static void HighScoreCheck(bool result){
		if(result){
			Debug.Log("Submitted Score");
		}else{
			Debug.Log("Error Submitting Score");
		}
	}

	public void ShowLeaderboard(){
        GetComponent<AudioManager>().ButtonSound();
#if UNITY_ANDROID
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
#elif UNITY_IPHONE
        GameCenterPlatform.ShowLeaderboardUI(leaderBoardId, UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif
    }





    void OnApplicationQuit()
	{
		PlayerPrefs.DeleteKey("deathCount");
		PlayerPrefs.Save();
		Debug.Log("deleting Death Count: " + PlayerPrefs.GetInt("deathCount",0));
	}

	public void ToggleSound(){
		if(isSoundOn){
			PlayerPrefs.SetInt("soundOn", 0);
		}else{
			PlayerPrefs.SetInt("soundOn", 1);
		}
		isSoundOn = !isSoundOn;
		CheckSound();
	}

	void CheckSound(){
		if (isSoundOn)
		{
			MusicButton.GetComponent<Image>().sprite = soundOn;
			//MusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
			GetComponent<AudioManager>().soundIsOn = true;
			GetComponent<AudioManager>().PlayBackgroundMusic();
		}
		else
		{
            MusicButton.GetComponent<Image>().sprite = soundOff;
            //MusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
			GetComponent<AudioManager>().soundIsOn = false;
			GetComponent<AudioManager>().StopBackgroundMusic();
		}
	}
	public void OpenPolicy(){
		Application.OpenURL(policyURL);
	}

	public void RateUs(){
		PlayerPrefs.SetInt("Rated", 1);
		if(Application.platform == RuntimePlatform.Android){
			Application.OpenURL(PlayStore);
		}else if (Application.platform == RuntimePlatform.IPhonePlayer){
			Application.OpenURL(AppStore);
		}else{
			Application.OpenURL("www.google.com");
		}
	}

    public void ChangeScene() {
        GetComponent<AdManager>().bannerView.Hide();
        GetComponent<AdManager>().bannerView.Destroy();
        SceneManager.LoadScene("LevelsScene");

    }

    public void GetCoin(float x, float y, int value) {
        RectTransform clone = Instantiate(coinprefab, canvas.transform, false).GetComponent<RectTransform>();
        clone.anchorMin = Camera.main.WorldToViewportPoint(new Vector2(x, y));
        //clone.anchorMin = new Vector2(Screen.width / 2, Screen.height / 2);
        clone.sizeDelta = new Vector2(100, 100);
        clone.anchorMax = clone.anchorMin;

        clone.anchoredPosition = clone.anchorMin;

        clone.anchorMin = Camera.main.WorldToViewportPoint(new Vector2(x, y));
        //clone.anchorMin = GetComponent<GameManager>().camera.GetComponent<Camera>().WorldToViewportPoint(GetComponent<GameManager>().camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(x, y)));
        //clone.anchorMax = clone.anchorMin;

        clone.SetParent(coinText.transform);
        clone.DOAnchorPos(Vector3.zero, 0.6f)
            .SetEase(animationEase)
            .OnComplete(() => {
                Destroy(clone.gameObject); ;

                coinText.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.4f)
                .OnComplete(() => {
                    coinText.GetComponent<RectTransform>().localScale = Vector3.one;
                }).Play();

                coins += value;
            }
            ).Play();
    }

    float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}
	
	public IEnumerator ShareScreenshot() {
		yield return new WaitForSeconds(1);
		// wait for graphics to render
		yield return new WaitForEndOfFrame();
		
		// prepare texture with Screen and save it
		Texture2D texture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
		texture.Apply();
		
		Sprite screenshot = Sprite.Create(texture, new Rect(0f, 0f, Screen.width, Screen.height), new Vector2(0,0));
		ScreenBackground.GetComponent<Image>().sprite = screenshot;
		ScreenBackgroundParent.SetActive(true);	
		// save to persistentDataPath File
		byte[] data = texture.EncodeToPNG();        
		destination = Path.Combine(Application.persistentDataPath, 
										  System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");        
		File.WriteAllBytes(destination, data);
		RatingManager ratingManager = FindObjectOfType<RatingManager>();
		if(ratingManager != null){
			ratingManager.die();
		}
		
	}
	
	public void SendScreenshot(){
		AnalyticsEvent.Custom("share", new Dictionary<string, object>
			{
				{ "user_id", user_id },
				{ "value", "started sharing intent" }
			});
		if(!Application.isEditor)
		{
			new NativeShare().AddFile( destination ).SetSubject( "Check out my score" ).SetText( "What a time to be alive !" ).Share();

		}
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

[Serializable]
public struct Background {
	public Sprite i_sprite;
	public Sprite sprite;
	public Sprite beams;
	public Color color;
}
