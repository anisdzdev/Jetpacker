using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopItem : MonoBehaviour {


	public string ItemName = "0";
	public string description;
	public Sprite image;
	public Sprite selected_img;
	public Sprite unselected_img;
	public Material grayScale;
	public Image item_image;
	public Image item_background;
	public bool unlocked;
	public bool using_item;
	public UnlockMethod methodToUnlock;
	public int numberForMethod;
    public bool buyable;
    public int price;
	public ShopItem thisItem;


	void Start(){
		if(unlocked){
			PlayerPrefs.SetInt (ItemName, 1);
		}

        
		item_image.sprite = image;
		if (unlocked) {
			if (PlayerPrefs.GetString ("active_item", "1") == ItemName) {
				using_item = true;
				FindObjectOfType<GameManager> ().FirstClickItem(thisItem);
				item_background.sprite = selected_img;
			} else {
				using_item = false;
				item_background.sprite = unselected_img;
			}
		}
	}

	void Update(){
		if (PlayerPrefs.GetInt (ItemName) == 1) {
			unlocked = true;
			item_image.material = null;
		} else {
			unlocked = false;
			item_image.material = grayScale;
		}
		if (unlocked) {
			if (PlayerPrefs.GetString ("active_item","1") == ItemName) {
				using_item = true;
				item_background.sprite = selected_img;
			} else {
				using_item = false;
				item_background.sprite = unselected_img;
			}
		}
        if (!buyable) {
            if (!unlocked) {
                if (methodToUnlock == UnlockMethod.DaysLogin) {
                    if (FindObjectOfType<GameManager>().consecutiveDaysPlaying >= numberForMethod && PlayerPrefs.GetInt(ItemName) != 1) {
                        PlayerPrefs.SetInt(ItemName, 1);
                        PlayerPrefs.SetInt("new_item", 1);
                    }
                } else if (methodToUnlock == UnlockMethod.ScoreReach) {
                    if (FindObjectOfType<GameManager>().score >= numberForMethod && PlayerPrefs.GetInt(ItemName) != 1) {
                        PlayerPrefs.SetInt(ItemName, 1);
                        PlayerPrefs.SetInt("new_item", 1);
                    }
                } else if (methodToUnlock == UnlockMethod.LevelReach) {
                    if (FindObjectOfType<LevelManager>() != null) {
                        if (FindObjectOfType<LevelManager>().activeLevel >= numberForMethod-1 && PlayerPrefs.GetInt(ItemName) != 1) {
                            PlayerPrefs.SetInt(ItemName, 1);
                            PlayerPrefs.SetInt("new_item", 1);
                        }
                    }
                } else {
                    if (FindObjectOfType<GameManager>().secondsClicked >= (float)numberForMethod && PlayerPrefs.GetInt(ItemName) != 1) {
                        PlayerPrefs.SetInt(ItemName, 1);
                        PlayerPrefs.SetInt("new_item", 1);
                    }
                }
            }
        }

	}

	public void ClickItem(ShopItem thisItem){
		FindObjectOfType<GameManager> ().ClickItem(thisItem);
	}

	public void FirstClickItem(ShopItem thisItem){
		FindObjectOfType<GameManager> ().FirstClickItem(thisItem);
	}


}
