using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerExample : MonoBehaviour
{

	public GameObject storePanel;
	public GameObject showPanelBtn;
	public GameObject[] myChildren;
	public int active_item;
    // Start is called before the first frame update
    void Start()
    {
		active_item = PlayerPrefs.GetInt("active_item", 0);
		foreach (GameObject child in myChildren) {
			child.SetActive (false);
		}
		myChildren [active_item].SetActive (true);            // This will activate only the gameObject which is selected in the store
    }

    // Update is called once per frame
    void Update()
    {
		         // We add this to update if we want to check after each frame but it's always better to do it after store close

    }

	public void BackButton(){              //Function for closing the store
		storePanel.SetActive (false);
		showPanelBtn.SetActive (true);

		active_item = PlayerPrefs.GetInt("active_item", 0);
		foreach (GameObject child in myChildren) {
			child.SetActive (false);
		}
		myChildren [active_item].SetActive (true);    
	}

	public void ShowButton(){
		storePanel.SetActive (true);
		showPanelBtn.SetActive (false);
	}
}