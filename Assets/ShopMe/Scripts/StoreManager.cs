using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StoreManager : MonoBehaviour {
    public bool unlockAll;
	public GameObject item_prefab;
	public GameObject item_holder;
	public Item[] items;

	public bool deletePrefs;


	// Use this for initialization
	void Start () {
		if(deletePrefs){
			PlayerPrefs.DeleteKey("active_item");
		}
		foreach (Item item in items) {
			if(deletePrefs){
				PlayerPrefs.DeleteKey(item.name);
			}
			if(!item.disabled){
				GameObject item_pref;
				item_pref = Instantiate (item_prefab, item_holder.transform)as GameObject;
				item_pref.transform.SetParent (item_holder.transform);
				item_pref.GetComponent<ShopItem> ().ItemName = item.name;
				item_pref.name = item.name;
				item_pref.GetComponent<ShopItem> ().image = item.Image;
				item_pref.GetComponent<ShopItem> ().unlocked = (unlockAll) ? true : item.unlocked;
				item_pref.GetComponent<ShopItem> ().description = item.description;
				item_pref.GetComponent<ShopItem> ().thisItem = item_pref.GetComponent<ShopItem> ();
				item_pref.GetComponent<ShopItem> ().methodToUnlock = item.methodToUnlock;
				item_pref.GetComponent<ShopItem> ().numberForMethod = item.numberForMethod;
                    item_pref.GetComponent<ShopItem>().buyable = item.buyable;
                item_pref.GetComponent<ShopItem>().price = item.price;

            }
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[Serializable]
public struct Item {
	public string name;
	public bool disabled;
	public Sprite Image;
	public bool unlocked;
	public string description;
	public UnlockMethod methodToUnlock;
	public int numberForMethod;
    public bool buyable;
    public int price;
}

public enum UnlockMethod {
	DaysLogin,
	ScoreReach,
    LevelReach,
	SecondsOfClick
}
