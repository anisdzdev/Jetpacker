using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    [SerializeField]
	private AudioSource backgroundMusic, deathSound, spikeSound, scoreSound, fireSound, buttonSound, lockedSound;

    [HideInInspector]
    public bool soundIsOn = true;       //GameManager script might modify this value

    //Functions are called when it is necessary

    public void StopBackgroundMusic()
    {
		if(backgroundMusic !=null){
        	backgroundMusic.Stop();
		}
    }

    public void PlayBackgroundMusic()
    {
		if (soundIsOn){
			if(backgroundMusic !=null){
				backgroundMusic.Play();
			}
		}
    }

    public void ScoreSound()
    {
        if (soundIsOn)
            scoreSound.Play();
    }

    public void DeathSound()
    {
        if (soundIsOn)
            deathSound.Play();
    }
	
	public void ButtonSound()
    {
		Debug.Log("button_sound");
        if (soundIsOn)
            buttonSound.Play();
    }

	public void LockedSound(){
		if (soundIsOn)
			lockedSound.Play();
	}
	
	public void SpikeDeathSound()
    {
        if (soundIsOn)
            spikeSound.Play();
    }

    public void FireSound()
    {
        if (soundIsOn && !fireSound.isPlaying)
            fireSound.Play();
    }

	public void StopFireSound()
	{
		fireSound.Stop();
	}
}
