using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleManager : MonoBehaviour
{

	int death = 0;
	public Text DeathsCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		DeathsCount.text = death.ToString();
    }

	public void KillOnce(){
		RatingManager ratingManager = FindObjectOfType<RatingManager>();
		if(ratingManager != null){
			ratingManager.die();
		}
		death++;
	}
}
