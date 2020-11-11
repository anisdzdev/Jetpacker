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
using UnityEngine.Analytics;
using DG.Tweening;


public class LevelManager : MonoBehaviour {
    //[HideInInspector]
    public bool canStart, tapped, justStarted, started, dead, succeeded, first_time_spawn, changedBG, waitBeforeSpawn, StopCam, authenticated, shownSuccessMenu;
    public bool enhancedPlatformMove;
    public float camMinPos, characterMaxPos, previousSpawn, topConstraint, lastTop, middle, diff, secondsClicked;
    public List<Platforms> all_platforms = new List<Platforms>();
    string destination;
    private bool flash = false;
    public int BGNumber;
    public int BGcount;
    int spawnNumber = 3;
    public int coins, coinsToGet;

    string PlayStore = "http://play.google.com/store/apps/details?id={0}";
    string AppStore = "itms://itunes.apple.com/us/app/apple-store/{0}?mt=8";




    ShopItem focusedItem;

    public string leaderBoardId;
    public string leaderboardAndroidId;
    public string policyURL;
    public Color red;
    public Color green;
    public bool isSoundOn;

    Level level;
    public GameObject LastPlatform;


    [Header("Numbers")]
    public string user_id;
    public string iOSAppID = "";
    public int score, scoretoreach;
    public int deathCount;
    public int activeLevel;
    public float minPos;
    public float maxPos;
    public Vector3 offset;
    public float diePower;
    public float distanceZ = 10.0f;
    public bool ChangeBG;
    public int ChangeBgEvery;


    public Background[] backgrounds;

    public GameObject successScreenAnimations;
    public GameObject canvas;
    public GameObject coinPanel;
    public GameObject coinText;
    public GameObject finishcoins;
    public GameObject coinprefab;
    public GameObject coin_in_game_prefab;
    public GameObject confetti;
    public GameObject nextButton;
    public GameObject finishedGame;
    public TextMeshProUGUI title, description, youdied;
    public float beamSpeed, boostSpeed;
    public GameObject shopIndicator1, shopIndicator2;


    public List<SpriteRenderer> ground_containers;
    public List<Sprite> grounds;
    public Slider slider;
    public TextMeshProUGUI lvl1, lvl2;


    public Ease animationEase = Ease.InQuad;

    float initial_pos, final_pos;


    // Start is called before the first frame update
    void Start() {
        backgrounds = GetComponent<GameManager>().backgrounds;
        BGcount = backgrounds.Length;
        dead = false;
        GetComponent<GameManager>().player = FindObjectOfType<PlayerMovement>();
        GetComponent<GameManager>().player.started = false;
        GetComponent<GameManager>().tapToFly.OpenCloseObjectAnimation();
        GetComponent<GameManager>().tapToFlyImage.OpenCloseObjectAnimation();
        GetComponent<GameManager>().store_btn.OpenCloseObjectAnimation();
        GetComponent<GameManager>().menu_btn.OpenCloseObjectAnimation();
        GetComponent<GameManager>().mode_btn.OpenCloseObjectAnimation();
        coinPanel.SetActive(false);
        StartCoroutine(SetCanStart());
        camMinPos = GetComponent<GameManager>().camera.transform.position.y;
        characterMaxPos = GetComponent<GameManager>().player.rb.gameObject.transform.position.y + 1;
        offset = GetComponent<GameManager>().Spikes.transform.position - Camera.main.gameObject.transform.position;
        string ItemName = PlayerPrefs.GetString("active_item", "1");
        SetSkin(ItemName);
        activeLevel = PlayerPrefs.GetInt("Level", 0);
        GetComponent<LevelDecoder>().GetLevels();
        LoadLevel();
        
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
        PlayStore = PlayStore.Replace("{0}", Application.identifier);
#endif

    }

    void FixedUpdate() {
        //Spikes.transform.position = Camera.main.gameObject.transform.position + offset;

    }

    void LoadLevel() {
        level = GetComponent<LevelDecoder>().GetLevel(activeLevel);
        scoretoreach = level.valuetoreach;
        ChangeBgEvery = scoretoreach;
        if(level.backgroundchangeevery == 0) {
            ChangeBgEvery = scoretoreach;
        } else {
            ChangeBgEvery = level.backgroundchangeevery;
        }
        title.text = level.title;
        description.text = level.description;
        if (level.light_description_color) {
            description.color = Color.white;
            youdied.color = Color.white;
        }
        if(level.ground != 0) {
            foreach(SpriteRenderer rend in ground_containers) {
                rend.sprite = grounds[level.ground];
                rend.gameObject.GetComponent<Animator>().enabled = false;
            }
        }
        initial_pos = GetComponent<GameManager>().player.rb.transform.position.y;
        final_pos = -1 + scoretoreach * 3.5f;
        lvl1.text = (activeLevel+1).ToString();
        if (GetComponent<LevelDecoder>().LevelCount() == activeLevel + 1) {
            lvl2.text = "end";
            print("end");
        } else {
            lvl2.text = (activeLevel + 2).ToString();
            print("next");
        }
        if (level.enhanced) {
            foreach(Platforms plat in all_platforms) {
                plat.speed = level.beamspeed;
            }
        }
        BGNumber = level.background;
        ChangeBackground(true);
        coinsToGet = level.coins;
        finishcoins.GetComponent<TextMeshProUGUI>().text = "you earned " + coinsToGet + " coins";
        this.enhancedPlatformMove = level.enhanced;
        this.beamSpeed = level.beamspeed;
        this.boostSpeed = level.boostspeed;
        

    }

    // Update is called once per frame
    void Update() {
        if (PlayerPrefs.GetInt("new_item") == 1) {
            shopIndicator1.SetActive(true);
            shopIndicator2.SetActive(true);
        } else {
            shopIndicator1.SetActive(false);
            shopIndicator2.SetActive(false);
        }
        user_id = PlayerPrefs.GetString("userid");
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;
        middle = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height / 2, distanceZ)).y;
        diff = topConstraint - middle;

        PlayerPrefs.SetInt("coins", coins);
        coinText.GetComponent<TextMeshProUGUI>().text = coins.ToString();

        if (score >= scoretoreach) {
            succeeded = true;
            slider.value = 1;
            if (!shownSuccessMenu) {
                ShowSuccessMenu();
                shownSuccessMenu = true;
            }
        } else {
            slider.value = (GetComponent<GameManager>().player.rb.transform.position.y - initial_pos) / (final_pos - initial_pos);
        }
        if (ChangeBG) {
            if (score != 0 && GetComponent<GameManager>().player.rb.gameObject.transform.position.y >= topConstraint && score % ChangeBgEvery == 0 && !succeeded) {
                if (!changedBG) {
                    lastTop = topConstraint;
                    BGNumber++;
                    ChangeBackground(false);
                    changedBG = true;
                }
            } else {
                changedBG = false;
            }
        }

        CameraMovement();
        //if((!player.started && Input.touchCount > 0 && !justStarted) || (!player.started && Input.GetMouseButton(0) && !justStarted)){
        if ((!GetComponent<GameManager>().player.started && tapped && !justStarted) || (!GetComponent<GameManager>().player.started && tapped && !justStarted)) {
            if (canStart) {
                //StartScreen.SetActive(false);
                GetComponent<GameManager>().store_btn.OpenCloseObjectAnimation();
                GetComponent<GameManager>().menu_btn.OpenCloseObjectAnimation();
                GetComponent<GameManager>().mode_btn.OpenCloseObjectAnimation();
                GetComponent<GameManager>().GameName.OpenCloseObjectAnimation();
                GetComponent<GameManager>().ScoreAnim.OpenCloseObjectAnimation();
                GetComponent<GameManager>().tapToFly2.OpenCloseObjectAnimation();
                GetComponent<GameManager>().tapToFlyImage.OpenCloseObjectAnimation();
                GetComponent<AdManager>().RemoveBanner();
                //GameScreen.SetActive(true);
                justStarted = true;
            }
        } else if (justStarted) {
            GetComponent<GameManager>().player.started = true;
            justStarted = false;
            started = true;
        }

        if (started) {
            GetComponent<GameManager>().ScoreText.text = score.ToString();
        }

        if (flash) {
            GetComponent<GameManager>().myCG.alpha = GetComponent<GameManager>().myCG.alpha - Time.deltaTime;
            if (GetComponent<GameManager>().myCG.alpha <= 0) {
                GetComponent<GameManager>().myCG.alpha = 0;
                flash = false;
            }
        }


    }

    private void ShowSuccessMenu() {
        if (GetComponent<GameManager>().player.started) {
            GetComponent<GameManager>().player.succeeded = true;
            deathCount++;
            PlayerPrefs.SetInt("deathCount", deathCount);
            GetComponent<GameManager>().player.gameObject.SetActive(false);
            GetComponent<AudioManager>().StopFireSound();
            foreach (EasyTween tween in successScreenAnimations.GetComponents<EasyTween>()) {
                tween.OpenCloseObjectAnimation();
            }
            GetComponent<GameManager>().GameScreen.SetActive(false);
            confetti.SetActive(true);
            if (GetComponent<LevelDecoder>().LevelCount() == activeLevel + 1) {
                
                nextButton.SetActive(false);
                finishedGame.SetActive(true);
            } else {
                PlayerPrefs.SetInt("Level", activeLevel + 1);
                nextButton.SetActive(true);
                finishedGame.SetActive(false);
            }
            if (deathCount != 0 && deathCount % 3 == 0) {
                GetComponent<AdManager>().ShowInterstitial();
                Debug.Log("Interstial Now");
            }
            //coins += coinsToGet;                  //AFTER CLOSING PANEL
        }
    }

    public void Die() {
        if (GetComponent<GameManager>().player.started) {
            dead = true;

            deathCount++;
            if (score >= ChangeBgEvery) {
                GetComponent<GameManager>().player.rb.gameObject.GetComponent<Collider2D>().isTrigger = true;
            }
            PlayerPrefs.SetInt("deathCount", deathCount);

            //Platforms[] all_platforms = FindObjectsOfType<Platforms>();
            foreach (Platforms plat in all_platforms) {
                if (plat != null) {
                    plat.gameObject.SetActive(true);
                    //Destroy(plat);
                }
            }
            BoxCollider2D[] Colliders = FindObjectsOfType<BoxCollider2D>();
            foreach (BoxCollider2D col in Colliders) {
                if (col.gameObject.name == "Platform" || col.gameObject.name == "Right" || col.gameObject.name == "Left") {
                    col.isTrigger = true;
                }
            }
            GetComponent<GameManager>().player.rb.constraints = RigidbodyConstraints2D.None;
            //player.rb.gameObject.transform.rotation = Quaternion.RotateTowards(player.rb.gameObject.transform.rotation,Quaternion.Euler(new Vector3(0,0,-99)), 500 * Time.deltaTime);
            GetComponent<GameManager>().player.rb.AddTorque(diePower, ForceMode2D.Impulse);
            GetComponent<GameManager>().EndScreen.SetActive(true);
            GetComponent<GameManager>().GameScreen.SetActive(false);
            GetComponent<GameManager>().EndScoreText.text = score.ToString();

            if (PlayerPrefs.GetInt("HScore", 0) < score) {
                PlayerPrefs.SetInt("HScore", score);
                GetComponent<GameManager>().BestScoreText.text = "new best";
                AnalyticsEvent.Custom("new_best", new Dictionary<string, object>
                {
                        { "score_id", "score" },
                        { "value", score }
                });
                HighScoreBeaten();
            } else {
                GetComponent<GameManager>().BestScoreText.text = "best score: " + PlayerPrefs.GetInt("HScore", 0);
            }
            GetComponent<GameManager>().you_died.OpenCloseObjectAnimation();
            GetComponent<GameManager>().restart_btn.OpenCloseObjectAnimation();
            GetComponent<GameManager>().new_best.OpenCloseObjectAnimation();
            //#if UNITY_IPHONE
            /*if (Social.localUser.authenticated && authenticated) {
                GetComponent<GameManager>().leaderboardBtn.SetActive(true);
            }*/
            //#endif
            flash = true;
            GetComponent<GameManager>().myCG.alpha = 1;
            if (deathCount != 0 && deathCount % 3 == 0) {
                GetComponent<AdManager>().ShowInterstitial();
                Debug.Log("Interstial Now");
            }
            StartCoroutine(ShareScreenshot());
        }
    }

    public IEnumerator SetCanStart() {
        yield return new WaitForSeconds(1);
        canStart = true;
    }

    public void SpawnPlatform() {
        if (ChangeBG && waitBeforeSpawn) {
            waitBeforeSpawn = false;
            return;
        }
        LastPlatform = Instantiate(GetComponent<GameManager>().PlatformObj, new Vector3((float)UnityEngine.Random.Range(minPos, maxPos), previousSpawn + 3.5f, transform.position.z), transform.rotation);
        LastPlatform.name = "Platform";
        
        LastPlatform.GetComponent<Platforms>().speed = beamSpeed;
        if (UnityEngine.Random.Range(0, 5) == 0) {
            GameObject coin = Instantiate(coin_in_game_prefab, new Vector3((float)UnityEngine.Random.Range(minPos-2.53f, maxPos-2.53f), previousSpawn + (2.75f), transform.position.z), transform.rotation);
            
        }
        int BGNumberCache = BGNumber;
        if (BGNumber >= BGcount) {
            BGNumberCache = BGcount - 1;
        }
        Sprite beam = backgrounds[BGNumberCache].beams;
        if (beam != null) {
            SpriteRenderer[] rends = LastPlatform.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer rend in rends) {
                rend.sprite = beam;
            }

        }
        spawnNumber++;
        previousSpawn = LastPlatform.transform.position.y;
        Platforms[] all_active_platforms = FindObjectsOfType<Platforms>();
        foreach (Platforms plat in all_active_platforms) {
            if (plat.gameObject.transform.position.y == ((score-3) * 3.5f) -1) {
                //Destroy(plat.gameObject);
                all_platforms.Add(plat);
                plat.gameObject.SetActive(false);
            }
        }
    }

    public void SpawnSky() {
        GameObject NewSpawnedSky = Instantiate(GetComponent<GameManager>().LastSpawnedSky, new Vector3(GetComponent<GameManager>().LastSpawnedSky.transform.position.x, GetComponent<GameManager>().LastSpawnedSky.transform.position.y + 9.828114f, GetComponent<GameManager>().LastSpawnedSky.transform.position.z), GetComponent<GameManager>().LastSpawnedSky.transform.rotation);
        NewSpawnedSky.name = "sky";
        GetComponent<GameManager>().LastSpawnedSky = NewSpawnedSky;
    }

    public void CameraMovement() {
        Camera cam = GetComponent<GameManager>().camera.GetComponent<Camera>();
        //characterMaxPos = player.rb.gameObject.transform.position.y + 1;
        if (!dead || score >= ChangeBgEvery || !succeeded) {
            if (/*ChangeBG &&*/ spawnNumber % ChangeBgEvery == 0) {
                StopCam = true;
                waitBeforeSpawn = true;
            }
            if ((GetComponent<GameManager>().player.rb.gameObject.transform.position.y - characterMaxPos) > camMinPos) {
                if (GetComponent<GameManager>().player.rb.gameObject.transform.position.y >= characterMaxPos) {

                    if (/*ChangeBG &&*/ StopCam && Math.Round(LastPlatform.transform.position.y + 2f, 1) <= Math.Round(topConstraint, 1)) {
                        float y = LastPlatform.transform.position.y + 2f - diff;
                        GetComponent<GameManager>().camera.transform.position = new Vector3(GetComponent<GameManager>().camera.transform.position.x, y, GetComponent<GameManager>().camera.transform.position.z);
                        GetComponent<GameManager>().Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
                    } else {
                        GetComponent<GameManager>().camera.transform.position = new Vector3(GetComponent<GameManager>().camera.transform.position.x, GetComponent<GameManager>().player.rb.gameObject.transform.position.y - characterMaxPos, GetComponent<GameManager>().camera.transform.position.z);
                        GetComponent<GameManager>().Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
                    }
                }
            }
            camMinPos = GetComponent<GameManager>().camera.transform.position.y;
        } else {
            if (GetComponent<GameManager>().camera.transform.position.y > 0) {
                GetComponent<GameManager>().camera.transform.position = new Vector3(GetComponent<GameManager>().camera.transform.position.x, GetComponent<GameManager>().player.rb.gameObject.transform.position.y - characterMaxPos, GetComponent<GameManager>().camera.transform.position.z);
                GetComponent<GameManager>().Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
            } else {
                GetComponent<GameManager>().camera.transform.position = new Vector3(GetComponent<GameManager>().camera.transform.position.x, 0, GetComponent<GameManager>().camera.transform.position.z);
                GetComponent<GameManager>().Spikes.transform.position = Camera.main.gameObject.transform.position + offset;
            }
        }
        float theChange = (BGNumber == 0) ? ((BGNumber + 1) * (ChangeBgEvery - 1) * 3.5f - (1f - 2f + diff)) : (BGNumber + 1) * ChangeBgEvery * 3.5f - (4f - 3.5f * (BGNumber - 1));
        float theInitialChange = (BGNumber == 0) ? 0f : lastTop - characterMaxPos;
        float mapped_val = map(GetComponent<GameManager>().camera.transform.position.y, theInitialChange, theChange, 0f, 9.5f);
        if (!dead && !succeeded) {
            GetComponent<GameManager>().SkyCam.transform.position = new Vector3(GetComponent<GameManager>().SkyCam.transform.position.x, mapped_val, GetComponent<GameManager>().SkyCam.transform.position.z);
        } else {
            GetComponent<GameManager>().SkyCam.transform.position = new Vector3(GetComponent<GameManager>().SkyCam.transform.position.x, 0f, GetComponent<GameManager>().SkyCam.transform.position.z);
        }
        //float MoveBy = (camera.gameObject.transform.position.y- SkyCam.transform.position.y) * 0.01f;
        //SkyCam.transform.Translate(0, MoveBy, 0, Space.Self);
        //SkyCam.transform.position = Vector3.Lerp(SkyCam.transform.position, new Vector3(SkyCam.transform.position.x, camera.gameObject.transform.position.y *0.1f,SkyCam.transform.position.z), Time.deltaTime);
        //if(SkyCam.transform.position.y >= LastSpawnedSky.transform.position.y){
        //	SpawnSky();
        //}
    }

    public void Restart() {
        GetComponent<AdManager>().bannerView.Hide();
        GetComponent<AdManager>().bannerView.Destroy();
        GetComponent<AudioManager>().ButtonSound();
        AnalyticsEvent.Custom("restart", new Dictionary<string, object>
        {
                { "user_id", user_id },
                { "value", "restarted" }
        });
            SceneManager.LoadScene("LevelsScene");
        
    }

    public void RestartGame() {
        GetComponent<AdManager>().bannerView.Hide();
        GetComponent<AdManager>().bannerView.Destroy();
        GetComponent<AudioManager>().ButtonSound();
        AnalyticsEvent.Custom("restart", new Dictionary<string, object>
        {
                { "user_id", user_id },
                { "value", "restarted" }
        });
        PlayerPrefs.SetInt("Level", 0);
        SceneManager.LoadScene("LevelsScene");

    }

    public void CloseStore() {
        GetComponent<GameManager>().store_overlay.OpenCloseObjectAnimation();
        GetComponent<GameManager>().Overlay.SetActive(false);
        GetComponent<GameManager>().store_panel.OpenCloseObjectAnimation();
    }

    public void OpenStore() {
        GetComponent<GameManager>().Overlay.SetActive(true);
        GetComponent<GameManager>().store_overlay.OpenCloseObjectAnimation();
        GetComponent<GameManager>().store_panel.OpenCloseObjectAnimation();
    }

    public void OpenMenu() {
        GetComponent<GameManager>().MenuOverlay.SetActive(true);
        GetComponent<GameManager>().menu_overlay.OpenCloseObjectAnimation();
        GetComponent<GameManager>().menu_panel.OpenCloseObjectAnimation();
    }

    public void CloseMenu() {
        GetComponent<GameManager>().menu_overlay.OpenCloseObjectAnimation();
        GetComponent<GameManager>().MenuOverlay.SetActive(false);
        GetComponent<GameManager>().menu_panel.OpenCloseObjectAnimation();
    }

    public void Tap() {
        tapped = true;
    }

    public void ClickItem(ShopItem item) {
        if (!item.unlocked) {
            GetComponent<AudioManager>().LockedSound();
            GetComponent<GameManager>().ItemDescription.text = item.description;
        } else {
            GetComponent<AudioManager>().ButtonSound();
            if (item.using_item) {
                GetComponent<GameManager>().ItemDescription.text = "active skin";
            } else {
                GetComponent<GameManager>().ItemDescription.text = "click again to activate this skin";
            }
        }

        if (focusedItem == item && item.unlocked) {
            PlayerPrefs.SetString("active_item", item.ItemName);
            GetComponent<GameManager>().ItemDescription.text = "active skin";
            SetSkin(item.ItemName);
            AnalyticsEvent.Custom("skin", new Dictionary<string, object>
                {
                    { "user_id", user_id },
                    { "value", item.ItemName }
                });
        }
        focusedItem = item;
    }

    public void FirstClickItem(ShopItem item) {
        if (!item.unlocked) {
            GetComponent<GameManager>().ItemDescription.text = item.description;
        } else {
            if (item.using_item) {
                GetComponent<GameManager>().ItemDescription.text = "active skin";
            } else {
                GetComponent<GameManager>().ItemDescription.text = "click again to activate this skin";
            }
        }

        if (focusedItem == item && item.unlocked) {
            PlayerPrefs.SetString("active_item", item.ItemName);
            GetComponent<GameManager>().ItemDescription.text = "active skin";
            SetSkin(item.ItemName);
        }
        focusedItem = item;
    }


    public void CloseCoinsPanel(bool withAd) {
        if (withAd) {
            //SHOW AD HERE AND TRIPLE MONEY
            FindObjectOfType<AdManager>().WatchRewardedAd();
        } else {


            //Animation
            Vector2 position = GetComponent<GameManager>().camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
            GetCoin(position.x, position.y, coinsToGet);
        }
    }

    public IEnumerator Reward3() {
        Vector2 position = GetComponent<GameManager>().camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        GetCoin(position.x, position.y, coinsToGet);
        yield return new WaitForSeconds(0.2f);
        GetCoin(position.x, position.y, coinsToGet);
        yield return new WaitForSeconds(0.2f);
        GetCoin(position.x, position.y, coinsToGet);
    }

    public void GetCoin(float x, float y, int value) {
        coinPanel.SetActive(false);
        RectTransform clone = Instantiate(coinprefab, canvas.transform, false).GetComponent<RectTransform>();
        clone.anchorMin = Camera.main.WorldToViewportPoint(new Vector2(x,y));
        //clone.anchorMin = new Vector2(Screen.width / 2, Screen.height / 2);
        clone.sizeDelta = new Vector2(100, 100);
        clone.anchorMax = clone.anchorMin;

        clone.anchoredPosition = clone.anchorMin;

        clone.anchorMin = Camera.main.WorldToViewportPoint(new Vector2(x,y));
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
    
    public void ChangeScene() {
        GetComponent<AdManager>().bannerView.Hide();
        GetComponent<AdManager>().bannerView.Destroy();
        SceneManager.LoadScene("MainScene");
    }

    public void SetSkin(string skin) {
        Transform[] children = GetComponent<GameManager>().player.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.gameObject != GetComponent<GameManager>().player.gameObject && child.gameObject.name != "Particle System") {
                child.gameObject.SetActive(false);
            }
        }
        GameObject Player = GetComponent<GameManager>().player.gameObject.transform.Find(skin).gameObject;
        Player.SetActive(true);

        GetComponent<GameManager>().player.rb = Player.GetComponent<Rigidbody2D>();
        GetComponent<GameManager>().player.sc = GetComponent<GameManager>().player.rb.gameObject.GetComponent<SpriteChanger>();
    }

    public void ChangeBackground(bool fromstart) {
        StopCam = false;
        waitBeforeSpawn = false;
        Debug.Log("Change Now");


        if (!fromstart) {
            previousSpawn = previousSpawn + 3.5f;
            SpawnPlatform();
            SpawnPlatform();
            SpawnPlatform();
        }
        int BGNumberCache = BGNumber;
        if (BGNumber >= BGcount) {
            BGNumberCache = BGcount - 1;
        }
        Color m_color = backgrounds[BGNumberCache].color;
        Sprite m_i_sprite = backgrounds[BGNumberCache].i_sprite;
        Sprite m_sprite = backgrounds[BGNumberCache].sprite;
        Sprite beam = backgrounds[BGNumberCache].beams;
        GetComponent<GameManager>().SkyCam.GetComponent<Camera>().backgroundColor = m_color;
        GameObject[] skies = GameObject.FindGameObjectsWithTag("Background");
        if (beam != null) {
            foreach (Platforms plat in all_platforms) {
                if (plat != null) {
                    SpriteRenderer[] rends = plat.gameObject.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer rend in rends) {
                        rend.sprite = beam;
                    }
                }
            }
            Platforms[] all_active_platforms = FindObjectsOfType<Platforms>();
            foreach (Platforms plat in all_active_platforms) {
                //plat.gameObject.GetComponentn<SpriteRenderer>().sprite = beam;
                if (plat != null) {
                    SpriteRenderer[] rends = plat.gameObject.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer rend in rends) {
                        rend.sprite = beam;
                    }
                }
            }
        }
        foreach (GameObject sky in skies) {
            if (sky.name == "i_sky" && m_i_sprite != null) {
                sky.GetComponent<SpriteRenderer>().sprite = m_i_sprite;
            } else {
                sky.GetComponent<SpriteRenderer>().sprite = m_sprite;
            }
        }
        //SkyCam.transform.position = new Vector3(LastSpawnedSky.transform.position.x, LastSpawnedSky.transform.position.y, LastSpawnedSky.transform.position.z);
    }

    void HighScoreBeaten() {
#if UNITY_IPHONE
		Social.ReportScore((long)score, leaderBoardId, HighScoreCheck);
#elif UNITY_ANDROID
        Social.ReportScore((long)score, leaderboardAndroidId, HighScoreCheck);
#endif
    }

    static void HighScoreCheck(bool result) {
        if (result) {
            Debug.Log("Submitted Score");
        } else {
            Debug.Log("Error Submitting Score");
        }
    }

    public void ShowLeaderboard() {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardAndroidId);
#elif UNITY_IPHONE
		Social.ShowLeaderboardUI();
#endif
    }



    void OnApplicationQuit() {
        PlayerPrefs.DeleteKey("deathCount");
        PlayerPrefs.Save();
        Debug.Log("deleting Death Count: " + PlayerPrefs.GetInt("deathCount", 0));
    }

    public void ToggleSound() {
        if (isSoundOn) {
            PlayerPrefs.SetInt("soundOn", 0);
        } else {
            PlayerPrefs.SetInt("soundOn", 1);
        }
        isSoundOn = !isSoundOn;
        CheckSound();
    }

    void CheckSound() {
        if (isSoundOn) {
            GetComponent<GameManager>().MusicButton.GetComponent<Image>().sprite = GetComponent<GameManager>().soundOn;
            //GetComponent<GameManager>().MusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
            GetComponent<AudioManager>().soundIsOn = true;
            GetComponent<AudioManager>().PlayBackgroundMusic();
        } else {
            GetComponent<GameManager>().MusicButton.GetComponent<Image>().sprite = GetComponent<GameManager>().soundOff;
            //GetComponent<GameManager>().MusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
            GetComponent<AudioManager>().soundIsOn = false;
            GetComponent<AudioManager>().StopBackgroundMusic();
        }
    }
    public void OpenPolicy() {
        Application.OpenURL(policyURL);
    }

    public void RateUs() {
        PlayerPrefs.SetInt("Rated", 1);
        if (Application.platform == RuntimePlatform.Android) {
            Application.OpenURL(PlayStore);
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            Application.OpenURL(AppStore);
        } else {
            Application.OpenURL("www.google.com");
        }
    }

    float map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public IEnumerator ShareScreenshot() {
        yield return new WaitForSeconds(1);
        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // prepare texture with Screen and save it
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        Sprite screenshot = Sprite.Create(texture, new Rect(0f, 0f, Screen.width, Screen.height), new Vector2(0, 0));
        GetComponent<GameManager>().ScreenBackground.GetComponent<Image>().sprite = screenshot;
        GetComponent<GameManager>().ScreenBackgroundParent.SetActive(true);
        // save to persistentDataPath File
        byte[] data = texture.EncodeToPNG();
        destination = Path.Combine(Application.persistentDataPath,
                                          System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, data);
        RatingManager ratingManager = FindObjectOfType<RatingManager>();
        if (ratingManager != null) {
            ratingManager.die();
        }

    }

    public void SendScreenshot() {
        AnalyticsEvent.Custom("share", new Dictionary<string, object>
            {
                { "user_id", user_id },
                { "value", "started sharing intent" }
            });
        if (!Application.isEditor) {
            new NativeShare().AddFile(destination).SetSubject("Check out my score").SetText("What a time to be alive !").Share();

        }
    }

    bool IsDigitsOnly(string str) {
        foreach (char c in str) {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }



}
