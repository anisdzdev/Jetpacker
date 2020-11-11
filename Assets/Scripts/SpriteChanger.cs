using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{

	public Sprite initial, noFire, lowFire, medFire, fullFire1, fullFire2, dead;
	public float timeBetweenSprites, timeBetweenFullFires;
	public bool testMode;
	string state;
	ParticleSystem[] ps;

	PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
		state = "initial";
		player = GetComponentInParent<PlayerMovement>();
		ps = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
		if(state == "fire"){
			FindObjectOfType<GameManager>().secondsClicked += Time.deltaTime;
			foreach(ParticleSystem p in ps){
				p.Play();
			}
			FindObjectOfType<AudioManager>().FireSound();
		}else{
			FindObjectOfType<GameManager>().secondsClicked=0;
			foreach(ParticleSystem p in ps){
				p.Stop();
			}
            if (player.isProtected) {
                player.isProtected = false;
                player.obstacles = 0;
            }
			FindObjectOfType<AudioManager>().StopFireSound();
		}
    }

	public void Play(string sp){
		switch (sp){
			case "dead":
				this.gameObject.GetComponent<SpriteRenderer>().sprite = dead;
				state = "dead";
				break;
			case "initial":
				this.gameObject.GetComponent<SpriteRenderer>().sprite = initial;
				state = "dead";
				break;
			case "Fire":
				if(state != "fire"){
					StartCoroutine(GoFire());
				}
				state = "fire";
				break;

			case "NoFire":
			state = "noFire";
				this.gameObject.GetComponent<SpriteRenderer>().sprite = noFire;
				break;

			default:
			this.gameObject.GetComponent<SpriteRenderer>().sprite = initial;
				break;

		}
	}

	IEnumerator GoFire(){
		this.gameObject.GetComponent<SpriteRenderer>().sprite = lowFire;
		yield return new WaitForSeconds(timeBetweenSprites);
		if(state == "fire"){
			this.gameObject.GetComponent<SpriteRenderer>().sprite = medFire;
			yield return new WaitForSeconds(timeBetweenSprites);
			if(state == "fire"){
				this.gameObject.GetComponent<SpriteRenderer>().sprite = fullFire1;
				yield return new WaitForSeconds(timeBetweenFullFires);
				StartCoroutine(toggleFullFires());
			}
		}
	}

	IEnumerator toggleFullFires(){
		if(state == "fire"){
			this.gameObject.GetComponent<SpriteRenderer>().sprite = fullFire2;
			yield return new WaitForSeconds(timeBetweenFullFires);
			if(state == "fire"){
				this.gameObject.GetComponent<SpriteRenderer>().sprite = fullFire1;
				yield return new WaitForSeconds(timeBetweenFullFires);
				StartCoroutine(toggleFullFires());
			}
		}
	}

    void OnCollisionEnter2D(Collision2D col) {
        if (!testMode) {
            if (player && !player.dead) {
                if (col.gameObject.name == "Left" || col.gameObject.name == "Right" || col.gameObject.name == "spike") {
                    if (!player.isProtected) {
                        Play("dead");
                        player.Died();
                        if (col.gameObject.name == "Left" || col.gameObject.name == "Right") {
                            col.gameObject.transform.parent.gameObject.GetComponent<Platforms>().enabled = false;
                            shakeGameObject(col.gameObject, 2, 1f, true);
                            FindObjectOfType<AudioManager>().DeathSound();

                        } else {
                            FindObjectOfType<AudioManager>().SpikeDeathSound();
                        }
                        Debug.Log("col : " + col.gameObject.name);
                    } else {
                        col.gameObject.transform.parent.gameObject.GetComponent<Platforms>().enabled = false;
                        fallGameObject(col);
                        col.gameObject.GetComponentInParent<Platforms>().OnTriggerEnter2D(this.GetComponent<Collider2D>());
                        col.gameObject.name = "NONE";
                        FindObjectOfType<AudioManager>().DeathSound();
                        player.isProtected = false;
                        player.obstacles = 0;
                    }
                }

            }
            if (state == "dead" && col.gameObject.name == "ground") {
                Play("initial");
                Debug.Log("PLAY INITIAL");
            }
        }
    }
	
	
	bool shaking = false;

	IEnumerator shakeGameObjectCOR(GameObject objectToShake, float totalShakeDuration, float decreasePoint, bool objectIs2D = false)
	{
		if (decreasePoint >= totalShakeDuration)
		{
			Debug.LogError("decreasePoint must be less than totalShakeDuration...Exiting");
			yield break; //Exit!
		}

		//Get Original Pos and rot
		Transform objTransform = objectToShake.transform;
		Vector3 defaultPos = objTransform.position;
		Quaternion defaultRot = objTransform.rotation;

		float counter = 0f;

		//Shake Speed
		const float speed = 0.1f;

		//Angle Rotation(Optional)
		const float angleRot = 4;

		//Do the actual shaking
		while (counter < totalShakeDuration)
		{
			counter += Time.deltaTime;
			float decreaseSpeed = speed;
			float decreaseAngle = angleRot;

			//Shake GameObject
			if (objectIs2D)
			{
				//Don't Translate the Z Axis if 2D Object
				Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
				tempPos.z = defaultPos.z;
				objTransform.position = tempPos;

				//Only Rotate the Z axis if 2D
				objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(0f, 0f, 1f));
			}
			else
			{
				objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
				objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(1f, 1f, 1f));
			}
			yield return null;


			//Check if we have reached the decreasePoint then start decreasing  decreaseSpeed value
			if (counter >= decreasePoint)
			{
				Debug.Log("Decreasing shake");

				//Reset counter to 0 
				counter = 0f;
				while (counter <= decreasePoint)
				{
					counter += Time.deltaTime;
					decreaseSpeed = Mathf.Lerp(speed, 0, counter / decreasePoint);
					decreaseAngle = Mathf.Lerp(angleRot, 0, counter / decreasePoint);

					Debug.Log("Decrease Value: " + decreaseSpeed);

					//Shake GameObject
					if (objectIs2D)
					{
						//Don't Translate the Z Axis if 2D Object
						Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
						tempPos.z = defaultPos.z;
						objTransform.position = tempPos;

						//Only Rotate the Z axis if 2D
						objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(0f, 0f, 1f));
					}
					else
					{
						objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
						objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(1f, 1f, 1f));
					}
					yield return null;
				}

				//Break from the outer loop
				break;
			}
		}
		objTransform.position = defaultPos; //Reset to original postion
		objTransform.rotation = defaultRot;//Reset to original rotation
		objectToShake.transform.parent.gameObject.GetComponent<Platforms>().enabled = true;
		shaking = false; //So that we can call this function next time
		Debug.Log("Done!");
	}

    void fallGameObject(Collision2D obj) {
        Rigidbody2D rb = obj.gameObject.AddComponent<Rigidbody2D>();
        obj.gameObject.GetComponent<Collider2D>().isTrigger = true;
        print(obj.relativeVelocity);
        rb.AddForce(-obj.relativeVelocity*0.8f, ForceMode2D.Impulse);
    }


	void shakeGameObject(GameObject objectToShake, float shakeDuration, float decreasePoint, bool objectIs2D = false)
	{
		if (shaking)
		{
			return;
		}
		shaking = true;
		StartCoroutine(shakeGameObjectCOR(objectToShake, shakeDuration, decreasePoint, objectIs2D));
	}
}
