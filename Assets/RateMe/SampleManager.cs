using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnDie(){
		//Do whatever you want here
		RatingManager ratingManager = FindObjectOfType<RatingManager>();
		if(ratingManager != null){
			ratingManager.die();
		}
	}
}
