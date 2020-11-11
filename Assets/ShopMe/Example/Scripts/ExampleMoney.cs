using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleMoney : MonoBehaviour {

	public int money;
	public Text money_text;
	// Use this for initialization
	void Start () {
		money = PlayerPrefs.GetInt ("tokens", 0);

	}
	
	// Update is called once per frame
	void Update () {
		money_text.text = money.ToString();
		PlayerPrefs.SetInt ("tokens", money);                             //TODO ADD THIS SCRIPT TO YOUR CODE
	}

	//You can customize this script to have full control over the money (save with playerprefs, etc)
}
