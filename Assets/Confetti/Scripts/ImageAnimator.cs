using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour {
    
    public event EventHandler OnAnimationLoopedFirstTime;
    public event EventHandler OnAnimationLooped;

    [SerializeField] private Sprite[] frameArray;
    [SerializeField] private float framerate = .1f;
    [SerializeField] private bool loop;

    private int currentFrame;
    private float timer;
    private Image image;
    private bool isPlaying;
    private int loopCounter;

    private void Awake() {
        image = gameObject.GetComponent<Image>();

        if (frameArray != null) {
            PlayAnimation(frameArray, framerate);
        } else {
            isPlaying = false;
        }
    }

    private void Update() {
        if (!isPlaying) {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= framerate) {
            timer -= framerate;
            currentFrame = (currentFrame + 1) % frameArray.Length;
            if (!loop && currentFrame == 0) {
                StopPlaying();
            } else {
                image.sprite = frameArray[currentFrame];
            }

            if (currentFrame == 0) {
                loopCounter++;
                if (loopCounter == 1) {
                    if (OnAnimationLoopedFirstTime != null) OnAnimationLoopedFirstTime(this, EventArgs.Empty);
                }
                if (OnAnimationLooped != null) OnAnimationLooped(this, EventArgs.Empty);
            }
        }
    }

    public void StopPlaying() {
        isPlaying = false;
    }

    public void PlayAnimation(Sprite[] frameArray, float framerate, bool loop = true) {
        this.frameArray = frameArray;
        this.framerate = framerate;
        isPlaying = true;
        currentFrame = 0;
        timer = 0f;
        loopCounter = 0;
        image.sprite = frameArray[currentFrame];
    }

}
