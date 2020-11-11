using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool isInLevelMode;

	[Header("Health Settings")]
	public float jumpForce = 10f;
	public float maxVerSpeed = 15f;
	public float maxHorSpeed = 10f;
	public float sideSpeed;
	public float rotSpeed = 10;
	public float angle = 15f;
	public float limitXMin;
	public float limitXMax;
	public float leftConstraint = 0.0f;
	public float rightConstraint = 0.0f;
	public float bottomConstraint = 0.0f;
	public float buffer = 1.0f; // set this so the spaceship disappears offscreen before re-appearing on other side
	public float distanceZ = 10.0f;
	public bool dead, succeeded, started = true;
    public bool isProtected;
    public float timer = 1f;
    public int obstacles;

    [Space(10)]
	[Header("Health Settings")]
	public bool infiniteLoop = true;
	public bool str8 = true;

	[Space(10)]
	[Header("Character Specific")]
	public Rigidbody2D rb;

	[HideInInspector]
	public SpriteChanger sc;

	void Start ()
	{
		sc = rb.gameObject.GetComponent<SpriteChanger>();
        isInLevelMode = (FindObjectOfType<LevelManager>() != null);
	}


	void Awake() 
	{
		leftConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, 0.0f, distanceZ) ).x;
		rightConstraint = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width, 0.0f, distanceZ) ).x;
		if(str8){
			maxVerSpeed *= 2;
		}
	}

	void FixedUpdate()
	{
		bottomConstraint = Camera.main.ScreenToWorldPoint( new Vector3(0.0f, 0.0f, distanceZ) ).y;
		ParticleSystem[] ps = rb.gameObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem p in ps){
			p.gameObject.SetActive(true);

		}
        timer -= Time.deltaTime;
        if(obstacles >= 3) {
            isProtected = true;
        }
#if UNITY_EDITOR
        
		if(started){
			if(!dead && !succeeded){
			if (Input.GetMouseButton(0))
			{
				sc.Play("Fire");
				Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
				if(!str8){
                        if(FindObjectOfType<LevelManager>() != null) {
                            if (FindObjectOfType<LevelManager>().boostSpeed != 0) {
                                jumpForce *= FindObjectOfType<LevelManager>().boostSpeed;
                            }
                        }
					if(mouse.x < Screen.width / 2)
					{
							rb.AddForce(new Vector2(-sideSpeed, 1.0f*jumpForce), ForceMode2D.Impulse);
							rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));
							rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation, Quaternion.Euler(new Vector3(0,0,angle)), rotSpeed * Time.deltaTime);
					}

					if(mouse.x > Screen.width / 2)
					{
							rb.AddForce(new Vector2(sideSpeed, 1.0f*jumpForce), ForceMode2D.Impulse);
							rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));
					rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation,Quaternion.Euler(new Vector3(0,0,-angle)), rotSpeed * Time.deltaTime);
					}
				}else{
					rb.AddForce(new Vector2(0, 1.0f*jumpForce), ForceMode2D.Impulse);
					rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));

				}
			}else{
				sc.Play("NoFire");
				rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation, Quaternion.Euler(new Vector3(0,0,0)), rotSpeed * Time.deltaTime);
			}
			}
		}
		#else
		if(started){
		if(!dead){
			if (Input.touchCount > 0)
			{
				sc.Play("Fire");
				var touch = Input.GetTouch(0);
				if(!str8){
					if (touch.position.x < Screen.width/2)
					{
							rb.AddForce(new Vector2(-sideSpeed, 1.0f*jumpForce), ForceMode2D.Impulse);
							rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));
							rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation, Quaternion.Euler(new Vector3(0,0,angle)), rotSpeed * Time.deltaTime);
							
					}
					else if (touch.position.x > Screen.width/2)
					{
							rb.AddForce(new Vector2(sideSpeed, 1.0f*jumpForce), ForceMode2D.Impulse);
							rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));
							rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation, Quaternion.Euler(new Vector3(0,0,-angle)), rotSpeed * Time.deltaTime);
						
					}
				}else{
					rb.AddForce(new Vector2(0, 1.0f*jumpForce), ForceMode2D.Impulse);
					rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorSpeed, maxHorSpeed), Mathf.Clamp(rb.velocity.y, -600, maxVerSpeed));

				}
			}else{
				sc.Play("NoFire");
				rb.gameObject.transform.rotation = Quaternion.RotateTowards(rb.gameObject.transform.rotation, Quaternion.Euler(new Vector3(0,0,0)), rotSpeed * Time.deltaTime);
			}
		}
		}
		#endif


	}



	void Update () {
		if (rb.gameObject.transform.position.x < leftConstraint - buffer) {
			float shipX = rightConstraint + buffer;
			rb.gameObject.transform.position = new Vector3(shipX, rb.gameObject.transform.position.y, rb.gameObject.transform.position.z);
			OutOfScreen();
		}
		if (rb.gameObject.transform.position.x > rightConstraint + buffer) {
			float shipX = leftConstraint - buffer;
			rb.gameObject.transform.position = new Vector3(shipX, rb.gameObject.transform.position.y, rb.gameObject.transform.position.z);
			OutOfScreen();
		}

		if (rb.gameObject.transform.position.y < bottomConstraint+buffer) {	
			BottomOut();
		}
	}
		
	void OutOfScreen() {
		//DIED
		if(!infiniteLoop){
            if (!isInLevelMode) {
                FindObjectOfType<GameManager>().Die();
            } else {
                FindObjectOfType<LevelManager>().Die();
            }
		}
	}

	void BottomOut() {
		Died();
		Debug.Log("bottomOut");
	}

	public void Died(){
		Debug.Log("DIED");
        if (!isInLevelMode) {

            if (!dead && FindObjectOfType<GameManager>().started) {
                FindObjectOfType<GameManager>().Die();
                dead = true;
                sc.Play("dead");
            }
        } else {
            if (!dead && FindObjectOfType<LevelManager>().started) {
                FindObjectOfType<LevelManager>().Die();
                dead = true;
                sc.Play("dead");
            }
        }
	}

}
