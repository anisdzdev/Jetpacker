using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
	RatingManager ratingManager;
	int MinimumRating;

	public GameObject RateButton;
	public GameObject EmailButton;

    // Start is called before the first frame update
    void Start()
    {
		ratingManager = FindObjectOfType<RatingManager>();
		MinimumRating = ratingManager.MinimumRating;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Rate(int Rating){
		ratingManager.Rate(Rating);
		if(Rating >= MinimumRating){
			RateButton.SetActive(true);
			EmailButton.SetActive(false);
		}else{
			RateButton.SetActive(false);
			EmailButton.SetActive(true);
		}
	}

	public void RateUs(){
		ratingManager.RateUs();
	}

	public void EmailUs(){
		ratingManager.EmailUs();
	}

	public void Later(){
		ratingManager.Later();
	}

	public void Never(){
		ratingManager.Never();
	}
}
